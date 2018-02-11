using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DocHound.Classes
{
    public static class VstsHelper
    {
        private static string ApiVersion => "4.1-preview";

        public static async Task<string> GetWorkItemTypes(string instance, string projectName, string personalAccessToken)
        {
            using (var httpClient = CreateHttpClient(instance + ":", personalAccessToken))
            {
                var url = "/" + projectName + "/_apis/wit/workitemtypes";

                var urlParameters = "api-version=" + ApiVersion;

                var httpResponseMessage = await httpClient.GetAsync(url + "?" + urlParameters);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var json = await httpResponseMessage.Content.ReadAsStringAsync();
                    return json;
                }
                return string.Empty;
            }
        }

        public static async Task<string> GetWorkItemTypeStates(string workItemType, string instance, string projectName, string personalAccessToken)
        {
            using (var httpClient = CreateHttpClient(instance + ":", personalAccessToken))
            {
                var url = "/" + projectName + "/_apis/wit/workitemtypes/" + workItemType + "/states";

                var urlParameters = "api-version=" + ApiVersion;

                var httpResponseMessage = await httpClient.GetAsync(url + "?" + urlParameters);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var json = await httpResponseMessage.Content.ReadAsStringAsync();
                    return json;
                }
                return string.Empty;
            }
        }

        public static async Task<string> GetWorkItemQueriesJson(string folder, string instance, string projectName, string personalAccessToken)
        {
            using (var httpClient = CreateHttpClient(instance + ":", personalAccessToken))
            {
                var url = "/" + projectName + "/_apis/wit/queries";
                if (!string.IsNullOrEmpty(folder))
                {
                    if (!folder.StartsWith("/")) url += "/";
                    url += folder;
                }

                var urlParameters = "$depth=2&api-version=" + ApiVersion;

                var httpResponseMessage = await httpClient.GetAsync(url + "?" + urlParameters);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var json = await httpResponseMessage.Content.ReadAsStringAsync();
                    return json;
                }
                return string.Empty;
            }
        }

        public static async Task<string> GetWorkItemQueryJson(string queryName, string instance, string projectName, string personalAccessToken)
        {
            using (var httpClient = CreateHttpClient(instance + ":", personalAccessToken))
            {
                var url = "/" + projectName + "/_apis/wit/queries";
                if (!string.IsNullOrEmpty(queryName))
                {
                    if (!queryName.StartsWith("/")) url += "/";
                    url += queryName;
                }

                var urlParameters = "api-version=" + ApiVersion;

                var httpResponseMessage = await httpClient.GetAsync(url + "?" + urlParameters);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var json = await httpResponseMessage.Content.ReadAsStringAsync();
                    return json;
                }
                return string.Empty;
            }
        }

        public static async Task<string> RunWorkItemQueryJson(string queryId, string instance, string projectName, string personalAccessToken)
        {
            using (var httpClient = CreateHttpClient(instance + ":", personalAccessToken))
            {
                if (!string.IsNullOrEmpty(queryId))
                {
                    Guid guidResult;
                    if (!Guid.TryParse(queryId, out guidResult))
                    {
                        try
                        {
                            var queryInfoJson = await GetWorkItemQueryJson(queryId, instance, projectName, personalAccessToken);
                            dynamic queryInfo = JObject.Parse(queryInfoJson);
                            queryId = queryInfo.id;
                        }
                        catch
                        {
                            return string.Empty;
                        }
                    }
                    var url = "/" + projectName + "/_apis/wit/wiql";
                    var urlParameters = "api-version=" + ApiVersion;
                    if (!queryId.StartsWith("/")) url += "/";
                    url += queryId;

                    var httpResponseMessage = await httpClient.GetAsync(url + "?" + urlParameters);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        var json = await httpResponseMessage.Content.ReadAsStringAsync();

                        // We got a list of items included in the result, but now we must also get the actual items in the list
                        dynamic queryResult = JObject.Parse(json);

                        if (queryResult.queryType == "flat" && queryResult.workItems != null) // TODO: Should support other types
                        {
                            // We figure out the fields we need to display
                            var fieldList = new List<string>();
                            foreach (dynamic column in queryResult.columns)
                                fieldList.Add(column.referenceName.ToString());
                            var fieldList2 = string.Join(",", fieldList.Take(100)); // We can query a maximum of 100 fields

                            // We now need to figure out what items we need to retrieve
                            var workItemIds = new List<int>();
                            foreach (dynamic workItem in queryResult.workItems)
                                workItemIds.Add((int)workItem.id);

                            var resultSets = new List<dynamic>();
                            while (workItemIds.Count > 0)
                            {
                                // We can retrieve up to 20 items in a batch
                                var resultJson = await GetWorkItemsJson(workItemIds.Take(20), instance, personalAccessToken, fieldList2);
                                dynamic queryResultItems = JObject.Parse(resultJson);
                                foreach (var resultItem in queryResultItems.value)
                                    resultSets.Add(resultItem);
                                workItemIds.RemoveRange(0, Math.Min(20, workItemIds.Count));
                            }

                            var itemsJson = JsonConvert.SerializeObject(resultSets);

                            json = json.Trim();
                            while (json.EndsWith("}")) json = json.Substring(0, json.Length - 1);

                            json = json + ",\"resultSet\": " + itemsJson + "}";

                            return json;
                        }
                    }
                }

                return string.Empty;
            }
        }

        public static async Task<string> GetWorkItemJson(int workItemNumber, string instance, string personalAccessToken, string fieldList = "")
        {
            return await GetWorkItemsJson(new[] {workItemNumber}, instance, personalAccessToken, fieldList);
        }
        public static async Task<string> GetWorkItemsJson(IEnumerable<int> workItemNumbers, string instance, string personalAccessToken, string fieldList = "")
        {
            using (var httpClient = CreateHttpClient(instance + ":", personalAccessToken))
            {
                // https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/wit/workitems?ids=297,299,300&api-version=1.0
                var workItemIds = string.Join(',', workItemNumbers);
                var parameters = "&api-version=" + ApiVersion;
                if (!string.IsNullOrEmpty(fieldList)) parameters += "&fields=" + fieldList;
                var httpResponseMessage = await httpClient.GetAsync("/_apis/wit/workitems?ids=" + workItemIds + parameters);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var json = await httpResponseMessage.Content.ReadAsStringAsync();
                    return json;
                }
                return string.Empty;
            }
        }

        public static async Task<string> GetTocJson(string instance, string project, string docsFolder, string personalAccessToken)
        {
            var tocJson = string.Empty;

            using (var httpClient = CreateHttpClient(instance + ":", personalAccessToken))
            {
                var repository = await GetRepositoriesJson(httpClient, project);
                var fileList = await GetFileListJson(httpClient, repository.Id, docsFolder);

                var toc = fileList?.FirstOrDefault(f => f.Path.ToLowerInvariant().EndsWith("_meta/_toc.json"));
                if (toc == null) toc = fileList?.FirstOrDefault(f => f.Path.ToLowerInvariant().EndsWith("_toc.json"));

                if (toc != null)
                    tocJson = await GetFileContents(httpClient, repository.Id, toc.Id);
                return tocJson;
            }
        }

        public static async Task<string> GetFileContents(string filePath, string instance, string project, string docsFolder, string personalAccessToken)
        {
            var fileContents = string.Empty;

            using (var httpClient = CreateHttpClient(instance + ":", personalAccessToken))
            {
                var repository = await GetRepositoriesJson(httpClient, project);
                var fileList = await GetFileListJson(httpClient, repository.Id, docsFolder);

                var docsFolderNormalized = docsFolder;
                var fullPath = filePath;
                if (docsFolderNormalized != null)
                {
                    if (!docsFolderNormalized.StartsWith("/")) docsFolderNormalized = "/" + docsFolderNormalized;
                    if (!docsFolderNormalized.EndsWith("/")) docsFolderNormalized = docsFolderNormalized + "/";
                    fullPath = docsFolderNormalized + filePath;
                }
                if (!fullPath.StartsWith("/")) fullPath = "/" + fullPath;

                var file = fileList.FirstOrDefault(f => f.Path == fullPath);

                if (file != null)
                    fileContents = await GetFileContents(httpClient, repository.Id, file.Id);
                return fileContents;
            }
        }

        public static async Task<Stream> GetWorkItemAttachmentStream(string url, string instance, string personalAccessToken)
        {
            var httpClient = CreateHttpClient(instance + ":", personalAccessToken);
            if (url.StartsWith(instance)) url = url.Substring(instance.Length + 1);
            return await httpClient.GetStreamAsync(url);
        }

        public static async Task<Stream> GetFileStream(string filePath, string instance, string project, string docsFolder, string personalAccessToken)
        {
            Stream fileContents = null;

            var httpClient = CreateHttpClient(instance + ":", personalAccessToken);

            var repository = await GetRepositoriesJson(httpClient, project);
            var fileList = await GetFileListJson(httpClient, repository.Id, docsFolder);

            var docsFolderNormalized = docsFolder;
            var fullPath = filePath;
            if (docsFolderNormalized != null)
            {
                if (!docsFolderNormalized.StartsWith("/")) docsFolderNormalized = "/" + docsFolderNormalized;
                if (!docsFolderNormalized.EndsWith("/")) docsFolderNormalized = docsFolderNormalized + "/";
                fullPath = docsFolderNormalized + filePath;
            }
            if (!fullPath.StartsWith("/")) fullPath = "/" + fullPath;

            var file = fileList.FirstOrDefault(f => f.Path == fullPath);

            if (file != null)
                fileContents = await GetFileStream(httpClient, repository.Id, file.Id);

            // Note: Not disposing this so the stream remains open.
            // TODO: Is this really required?
            //httpClient.Dispose();
            return fileContents;
        }

        private static HttpClient CreateHttpClient(string rootUrl, string personalAccessToken)
        {
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));
            var client = new HttpClient { BaseAddress = new Uri(rootUrl) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            return client;
        }

        private static async Task<VstsProjectInfo> GetProjectsJson(HttpClient httpClient, string projectName)
        {
            var httpResponseMessage = await httpClient.GetAsync("/_apis/projects?stateFilter=WellFormed&api-version=" + ApiVersion);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var json = await httpResponseMessage.Content.ReadAsStringAsync();
                dynamic projects = JObject.Parse(json);
                foreach (var project in projects.value)
                    if (project.name == projectName)
                        return new VstsProjectInfo
                        {
                            Id = new Guid(project.id.Value),
                            Name = (string)project.name.Value
                        };
            }
            return null;
        }

        private static async Task<VstsRepositoryInfo> GetRepositoriesJson(HttpClient httpClient, string projectName, string repositoryName = "")
        {
            var httpResponseMessage = await httpClient.GetAsync("/" + projectName + "/_apis/git/repositories?api-version=" + ApiVersion);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var json = await httpResponseMessage.Content.ReadAsStringAsync();
                dynamic repositories = JObject.Parse(json);
                foreach (var repository in repositories.value)
                    if (string.IsNullOrEmpty(repositoryName) || repository.name == projectName)
                        return new VstsRepositoryInfo
                        {
                            Id = new Guid(repository.id.Value),
                            Name = (string)repository.name.Value
                        };
            }
            return null;
        }

        private static async Task<List<VstsFileInfo>> GetFileListJson(HttpClient httpClient, Guid repositoryId, string scopePath = "")
        {
            var url = "/_apis/git/repositories/" + repositoryId + "/items?api-version=" + ApiVersion + "&includeContentMetadata=true&recursionLevel=Full";
            if (!string.IsNullOrEmpty(scopePath)) url += "&scopePath=" + scopePath;

            var httpResponseMessage = await httpClient.GetAsync(url);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var json = await httpResponseMessage.Content.ReadAsStringAsync();
                var files = new List<VstsFileInfo>();
                dynamic fileList = JObject.Parse(json);
                foreach (var file in fileList.value)
                    if (file.gitObjectType == "blob")
                        files.Add(new VstsFileInfo
                        {
                            Id = (string)file.objectId.Value,
                            Path = (string)file.path.Value,
                            Url = (string)file.url.Value
                        });
                return files;
            }
            return null;
        }

        private static async Task<string> GetFileContents(HttpClient httpClient, Guid repositoryId, string objectId)
        {
            var httpResponseMessage = await httpClient.GetAsync("/_apis/git/repositories/" + repositoryId + "/blobs/" + objectId + "?api-version=" + ApiVersion + "&$format=text");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var text = await httpResponseMessage.Content.ReadAsStringAsync();
                return text;
            }
            return null;
        }
        private static async Task<Stream> GetFileStream(HttpClient httpClient, Guid repositoryId, string objectId)
        {
            var httpResponseMessage = await httpClient.GetAsync("/_apis/git/repositories/" + repositoryId + "/blobs/" + objectId + "?api-version=" + ApiVersion + "&$format=text");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
                return stream;
            }
            return null;
        }
    }

    public class VstsProjectInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public override string ToString() => Name;
    }

    public class VstsRepositoryInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public override string ToString() => Name;
    }

    public class VstsFileInfo
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }

        public override string ToString() => Path;
    }
}
