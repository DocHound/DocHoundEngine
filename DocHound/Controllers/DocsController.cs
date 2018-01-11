using System.Threading.Tasks;
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
    }
}