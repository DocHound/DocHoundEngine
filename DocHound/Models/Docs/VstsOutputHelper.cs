using DocHound.Classes;
using DocHound.Interfaces;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace DocHound.Models.Docs
{
    public class VstsOutputHelper
    {
        private readonly TopicViewModel _parent;

        public VstsOutputHelper(TopicViewModel parent)
        {
            _parent = parent;
        }
        
        public string GetStateHtmlTag(string state) => GetStateHtmlTagAsync(state).Result;
        public async Task<string> GetStateHtmlTagAsync(string state)
        {
            var stateClass = state.ToLowerInvariant().Replace(" ", "-").Replace("--","-").Replace("--","-");
            stateClass = "vsts-state vsts-state-" + stateClass;
            return "<div class=\"" + stateClass + "\"> </div>";
        }
    }
}