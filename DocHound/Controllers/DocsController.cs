using System.Threading.Tasks;
using DocHound.Classes;
using DocHound.Models.Docs;
using Microsoft.AspNetCore.Mvc;

namespace DocHound.Controllers
{
    public class DocsController : Controller
    {
        public async Task<IActionResult> Topic(string topicName)
        {
            var vm = new TopicViewModel(topicName);
            await vm.LoadData();
            return View(vm.ThemeFolder + "/Topic.cshtml", vm);
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
            if (mode == "vstsgit")
            {
                var stream = await VstsHelper.GetFileStream(path, TopicViewModel.VstsInstance, TopicViewModel.VstsProjectName, TopicViewModel.VstsDocsFolder, TopicViewModel.VstsPat);

                var contentType = "application/binary";
                var lowerPath = path.ToLowerInvariant();

                if (lowerPath.EndsWith(".jpg") || lowerPath.EndsWith(".jpeg")) contentType = "image/jpeg";
                else if (lowerPath.EndsWith(".png")) contentType = "image/png";
                else if (lowerPath.EndsWith(".gif")) contentType = "image/gif";
                else if (lowerPath.EndsWith(".tif") || lowerPath.EndsWith(".tiff")) contentType = "image/tiff";

                var fileName = StringHelper.JustFileName(path);

                return File(stream, contentType, fileName);
            }
            return File((byte[])null, "image/jpeg", path);
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