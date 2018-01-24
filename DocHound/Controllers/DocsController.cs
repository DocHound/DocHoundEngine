using System;
using System.Threading.Tasks;
using DocHound.Classes;
using DocHound.Models.Docs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using DocHound.Interfaces;

namespace DocHound.Controllers
{
    public class DocsController : Controller
    {
        public async Task<IActionResult> Topic(string topicName)
        {
            var vm = new TopicViewModel(topicName, HttpContext);
            await vm.LoadData();
            return View(vm.ThemeFolder + "/" + vm.TemplateName + ".cshtml", vm);
        }

        // TODO: Need topics and toc to be individually accessible for AJAX calls
        //public async Task<IActionResult> TopicContentsOnly(string topicName)
        //{
        //    var vm = new TopicViewModel(topicName);
        //    await vm.LoadData();
        //    return Content(vm.Html);
        //}

        public async Task<IActionResult> FileProxy(string mode, string path)
        {
            if (path.ToLowerInvariant().StartsWith("http://") || path.ToLowerInvariant().StartsWith("https://"))
            {
                using (var client = new WebClientEx())
                {
                    var data = await client.DownloadDataTaskAsync(new Uri(path));
                    return File(data, GetContentTypeFromUrl(path), StringHelper.JustFileName(path));
                }
            }

            if (RepositoryTypeHelper.IsMatch(mode, RepositoryTypeNames.VstsGit))
            {
                var stream = await VstsHelper.GetFileStream(path, SettingsHelper.GetSetting<string>(Settings.VstsInstance), SettingsHelper.GetSetting<string>(Settings.VstsProjectName), SettingsHelper.GetSetting<string>(Settings.VstsDocsFolder), SettingsHelper.GetSetting<string>(Settings.VstsPat));
                return File(stream, GetContentTypeFromUrl(path), StringHelper.JustFileName(path));
            }

            if (RepositoryTypeHelper.IsMatch(mode, RepositoryTypeNames.VstsWorkItemTracking))
            {
                // TODO: No idea if there could be images in this and what to do with that...
            }
            return File((byte[])null, "image/jpeg", path);
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