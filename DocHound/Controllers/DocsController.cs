using System;
using System.Linq;
using System.Threading.Tasks;
using DocHound.Classes;
using DocHound.Models.Docs;
using Microsoft.AspNetCore.Mvc;
using DocHound.Interfaces;
using Microsoft.AspNetCore.Routing;
using DocHound.Models;
using Westwind.AspNetCore.Extensions;
using Westwind.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace DocHound.Controllers
{
    public class DocsController : Controller
    {
        //public async Task<IActionResult> Topic(string fragment1 = null, string fragment2 = null, string fragment3 = null, string fragment4 = null, string fragment5 = null, string fragment6 = null, string fragment7 = null, string fragment8 = null, string fragment9 = null, string fragment10 = null)
        public async Task<IActionResult> Topic()
        {
            // TODO: How do we get the repository Auth Requirement from the db or local settings?
            bool requireAuth = false;   // how do we get the repo info here?
            if (requireAuth)
                CheckAuthentication();
            

            var routeCollection = HttpContext.GetRouteData();
            var topic = routeCollection.Values.Values.FirstOrDefault()?.ToString();

            var vm = new TopicViewModel(topic, HttpContext);

            if (SqlDataAccess.CanUseSql)
            {
                var prefix = GetCurrentDomainPrefix();
                var settings = await SqlDataAccess.GetSqlRepositorySettings(prefix);
                if (settings == null)
                    return NotFound($"Document repository {prefix} does not exist.");
                vm.SetRootSettingsForRequest(settings);
                vm.UseSqlServer = true;
                vm.CurrentPrefix = prefix;               
            }
           
            await vm.LoadData();

            return View(vm.ThemeFolder + "/" + vm.TemplateName + ".cshtml", vm);
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

        // TODO: Need topics and toc to be individually accessible for AJAX calls
        //public async Task<IActionResult> TopicContentsOnly(string topicName)
        //{
        //    var vm = new TopicViewModel(topicName);
        //    await vm.LoadData();
        //    return Content(vm.Html);
        //}

        private void CheckAuthentication()
        {
            var appUser = User.GetAppUser();            
            if (!appUser.IsAuthenticated())
                Response.Redirect(Url.Action("Signin", "Account",new { ReturnUrl=GetUrl(Request)}));
        }

        static string GetUrl(HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        }

        public async Task<IActionResult> FileProxy(string mode, string path, string topic = "", string fileName = "")
        {
          
            // Special processing for file retrieval of attachments to TFS work items. This is mainly used to return images in item descriptions.
            if (RepositoryTypeHelper.IsMatch(mode, RepositoryTypeNames.VstsWorkItemTracking))
            {
                var model = new TopicViewModel(topic, HttpContext);
                if (SqlDataAccess.CanUseSql)
                {
                    var prefix = GetCurrentDomainPrefix();
                    var settings = await SqlDataAccess.GetSqlRepositorySettingsDynamic(prefix);
                    if (settings == null)
                        return NotFound($"Document repository {prefix} does not exist.");
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
                if (SqlDataAccess.CanUseSql)
                {
                    var prefix = GetCurrentDomainPrefix();
                    var settings = await SqlDataAccess.GetSqlRepositorySettingsDynamic(prefix);
                    if (settings == null)
                        return NotFound($"Document repository {prefix} does not exist.");
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