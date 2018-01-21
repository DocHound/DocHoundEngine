using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DocHound.Classes
{
    public static class VstsHelper
    {
        public static async Task<string> GetWorkItemJson(int workItemNumber, string instance, string personalAccessToken)
        {
            return await GetWorkItemsJson(new[] {workItemNumber}, instance, personalAccessToken);
        }
        public static async Task<string> GetWorkItemsJson(IEnumerable<int> workItemNumbers, string instance, string personalAccessToken)
        {
            var httpClient = CreateHttpClient(instance + ":", personalAccessToken);
            // https://fabrikam-fiber-inc.visualstudio.com/DefaultCollection/_apis/wit/workitems?ids=297,299,300&api-version=1.0
            var workItemIds = string.Join(',', workItemNumbers);
            var httpResponseMessage = await httpClient.GetAsync("/DefaultCollection/_apis/wit/workitems?ids=" + workItemIds + "&api-version=1.0");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var json = await httpResponseMessage.Content.ReadAsStringAsync();
                return json;
            }
            return string.Empty;
        }

        public static async Task<string> GetTocJson(string instance, string project, string docsFolder, string personalAccessToken)
        {
            var tocJson = string.Empty;

            var httpClient = CreateHttpClient(instance + ":", personalAccessToken);

            var repository = await GetRepositoriesJson(httpClient, project);
            var fileList = await GetFileListJson(httpClient, repository.Id, docsFolder);

            var toc = fileList.FirstOrDefault(f => f.Path.ToLowerInvariant().EndsWith("_meta/_toc.json"));
            if (toc == null) toc = fileList.FirstOrDefault(f => f.Path.ToLowerInvariant().EndsWith("_toc.json"));

            if (toc != null)
                tocJson = await GetFileContents(httpClient, repository.Id, toc.Id);

            httpClient.Dispose();
            return tocJson;
        }

        public static async Task<string> GetFileContents(string filePath, string instance, string project, string docsFolder, string personalAccessToken)
        {
            var fileContents = string.Empty;

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
                fileContents = await GetFileContents(httpClient, repository.Id, file.Id);

            httpClient.Dispose();
            return fileContents;
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
            // TODO: This is fishy. Why is there an embedded access topken here?!?
            //var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", "zi6opdawlmm7mvd3xxaycfxgboz2enbxrdmitjj5nawmbg3ldvtq")));
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", personalAccessToken)));
            var client = new HttpClient { BaseAddress = new Uri(rootUrl) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            return client;

        }

        private static async Task<VstsProjectInfo> GetProjectsJson(HttpClient httpClient, string projectName)
        {
            var httpResponseMessage = await httpClient.GetAsync("/_apis/projects?stateFilter=WellFormed&api-version=1.0");
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
            var httpResponseMessage = await httpClient.GetAsync("/DefaultCollection/" + projectName + "/_apis/git/repositories?api-version=1.0");
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
            var url = "/DefaultCollection/_apis/git/repositories/" + repositoryId + "/items?api-version=1.0&includeContentMetadata=true&recursionLevel=Full";
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
            var httpResponseMessage = await httpClient.GetAsync("/DefaultCollection/_apis/git/repositories/" + repositoryId + "/blobs/" + objectId + "?api-version=1.0&$format=text");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var text = await httpResponseMessage.Content.ReadAsStringAsync();
                return text;
            }
            return null;
        }
        private static async Task<Stream> GetFileStream(HttpClient httpClient, Guid repositoryId, string objectId)
        {
            var httpResponseMessage = await httpClient.GetAsync("/DefaultCollection/_apis/git/repositories/" + repositoryId + "/blobs/" + objectId + "?api-version=1.0&$format=text");
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
