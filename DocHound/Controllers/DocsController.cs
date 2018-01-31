using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using DocHound.Classes;
using DocHound.Models.Docs;
using Microsoft.AspNetCore.Mvc;
using DocHound.Interfaces;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Linq;

namespace DocHound.Controllers
{
    public class DocsController : Controller
    {
        //public async Task<IActionResult> Topic(string fragment1 = null, string fragment2 = null, string fragment3 = null, string fragment4 = null, string fragment5 = null, string fragment6 = null, string fragment7 = null, string fragment8 = null, string fragment9 = null, string fragment10 = null)
        public async Task<IActionResult> Topic()
        {
            var routeCollection = HttpContext.GetRouteData();
            var topic = routeCollection.Values.Values.FirstOrDefault()?.ToString();

            var vm = new TopicViewModel(topic, HttpContext);

            if (!string.IsNullOrEmpty(SettingsHelper.GetSetting<string>(Settings.SqlConnectionString)))
            {
                var settings = await GetSqlRepositorySettings();
                if (settings == null)
                    return NotFound($"Document repository {GetCurrentDomainPrefix()} does not exist.");
                vm.SetRootSettingsForRequest(settings);
            }

            await vm.LoadData();

            // TODO: Put this back in once we have our cert on Kavadocs.com
            //if (vm.RequireHttps && !HttpContext.Request.IsHttps && !HttpContext.Request.Host.Host.StartsWith("localhost"))
            //{
            //    var url = $"https://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
            //    return Redirect(url);
            //}
            return View(vm.ThemeFolder + "/" + vm.TemplateName + ".cshtml", vm);
        }

        private async Task<dynamic> GetSqlRepositorySettingsDynamic()
        {
            var settings = await GetSqlRepositorySettings();
            if (string.IsNullOrEmpty(settings)) return null;
            try
            {
                return JObject.Parse(settings);
            }
            catch
            {
                return null;
            }
        }

        private async Task<string> GetSqlRepositorySettings()
        {
            if (!string.IsNullOrEmpty(SettingsHelper.GetSetting<string>(Settings.SqlConnectionString)))
            {
                var prefix = GetCurrentDomainPrefix();
                return await GetRepositorySettings(prefix);
            }
            return string.Empty;
        }

        private string GetCurrentDomainPrefix()
        {
            if (Request.Host.Host.ToLowerInvariant() != "localhost")
            {
                var hostParts = Request.Host.Host.Split('.');
                if (hostParts.Length > 2)
                {
                    var usableHostParts = hostParts.Take(hostParts.Length - 2);
                    var prefix = string.Join('.', usableHostParts).ToLowerInvariant();
                    return prefix;
                }
            }
            var defaultPrefix = SettingsHelper.GetGlobalSetting("DefaultDomainPrefix");
            return string.IsNullOrEmpty(defaultPrefix) ? "docs" : defaultPrefix;
        }

        private static async Task<string> GetRepositorySettings(string prefix, string defaultPrefix = "docs")
        {
            if (string.IsNullOrEmpty(prefix)) prefix = defaultPrefix;

            using (var connection = new SqlConnection(SettingsHelper.GetSetting<string>(Settings.SqlConnectionString)))
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                    using (var command = new SqlCommand("SELECT Settings FROM Repositories WHERE Prefix = @Prefix"))
                    {
                        command.Connection = connection;
                        command.Parameters.Add(new SqlParameter("@Prefix", prefix.Trim().ToLower()));
                        return await command.ExecuteScalarAsync() as string;
                    }
                return null;
            }
        }

        // TODO: Need topics and toc to be individually accessible for AJAX calls
        //public async Task<IActionResult> TopicContentsOnly(string topicName)
        //{
        //    var vm = new TopicViewModel(topicName);
        //    await vm.LoadData();
        //    return Content(vm.Html);
        //}

        public async Task<IActionResult> FileProxy(string mode, string path, string topic = "", string fileName = "")
        {
            // Special processing for file retrieval of attachments to TFS work items. This is mainly used to return images in item descriptions.
            if (RepositoryTypeHelper.IsMatch(mode, RepositoryTypeNames.VstsWorkItemTracking))
            {
                var model = new TopicViewModel(topic, HttpContext);
                if (!string.IsNullOrEmpty(SettingsHelper.GetSetting<string>(Settings.SqlConnectionString)))
                {
                    var settings = await GetSqlRepositorySettingsDynamic();
                    if (settings == null)
                        return NotFound($"Document repository {GetCurrentDomainPrefix()} does not exist.");
                    model.SetRootSettingsForRequest(settings);
                }
                await model.LoadData(buildHtml: false, buildToc: true);
                var instance = model.GetSetting<string>(Settings.VstsInstance);
                var pat = model.GetSetting<string>(Settings.VstsPat);
                var stream = await VstsHelper.GetWorkItemAttachmentStream(path, instance, pat);
                return File(stream, GetContentTypeFromUrl(fileName), fileName);
            }

            // If we got this far, and the path is a fully qualified URL, then we just retrieve it
            if (path.ToLowerInvariant().StartsWith("http://") || path.ToLowerInvariant().StartsWith("https://"))
            {
                using (var client = new WebClientEx())
                {
                    var data = await client.DownloadDataTaskAsync(new Uri(path));
                    return File(data, GetContentTypeFromUrl(path), StringHelper.JustFileName(path));
                }
            }

            // If it is in a VSTS Git repository, we use the API to retrieve it
            if (RepositoryTypeHelper.IsMatch(mode, RepositoryTypeNames.VstsGit))
            {
                if (!string.IsNullOrEmpty(SettingsHelper.GetSetting<string>(Settings.SqlConnectionString)))
                {
                    var settings = await GetSqlRepositorySettingsDynamic();
                    if (settings == null)
                        return NotFound($"Document repository {GetCurrentDomainPrefix()} does not exist.");
                    var stream = await VstsHelper.GetFileStream(path,
                        SettingsHelper.GetSetting<string>(Settings.VstsInstance, requestRootSettings: settings),
                        SettingsHelper.GetSetting<string>(Settings.VstsProjectName, requestRootSettings: settings),
                        SettingsHelper.GetSetting<string>(Settings.VstsDocsFolder, requestRootSettings: settings),
                        SettingsHelper.GetSetting<string>(Settings.VstsPat, requestRootSettings: settings));
                    return File(stream, GetContentTypeFromUrl(path), StringHelper.JustFileName(path));
                }
                else
                {
                    var stream = await VstsHelper.GetFileStream(path, SettingsHelper.GetSetting<string>(Settings.VstsInstance), SettingsHelper.GetSetting<string>(Settings.VstsProjectName), SettingsHelper.GetSetting<string>(Settings.VstsDocsFolder), SettingsHelper.GetSetting<string>(Settings.VstsPat));
                    return File(stream, GetContentTypeFromUrl(path), StringHelper.JustFileName(path));
                }
            }

            // Otherwise, we got nothing :-)
            return null;
        }

        public string GetContentTypeFromUrl(string path)
        {
            var lowerPath = path.ToLowerInvariant();

            if (lowerPath.EndsWith(".jpg") || lowerPath.EndsWith(".jpeg")) return "image/jpeg";
            if (lowerPath.EndsWith(".png")) return "image/png";
            if (lowerPath.EndsWith(".gif")) return "image/gif";
            if (lowerPath.EndsWith(".tif") || lowerPath.EndsWith(".tiff")) return "image/tiff";
            if (lowerPath.EndsWith(".css")) return "text/css";

            return "text/plain";
        }

        public async Task<IActionResult> ReindexAllTocFiles()
        {
            var vm = new ReindexViewModel();
            await vm.LoadData();
            await vm.Reindex();
            return Content("Done.");
        }
    }
}