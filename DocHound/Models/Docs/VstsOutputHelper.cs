using DocHound.Classes;
using DocHound.Interfaces;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;

namespace DocHound.Models.Docs
{
    public class VstsOutputHelper
    {
        private readonly TopicViewModel _parent;

        public VstsOutputHelper(TopicViewModel parent)
        {
            _parent = parent;
        }

        private string _workItemStatesLockDummy = "";
        private bool _workItemStatesIsPopulated;
        private readonly Dictionary<string, string> _workItemStates = new Dictionary<string, string>();
        public string ImageLink { get; set; }

        public string GetStateHtmlTag(string state)
        {
            lock (_workItemStatesLockDummy)
                if (!_workItemStatesIsPopulated)
                {
                    var instance = _parent.GetSetting<string>(SettingsEnum.VstsInstance);
                    var projectName = _parent.GetSetting<string>(SettingsEnum.VstsProjectName);
                    var personalAccessToken = _parent.GetSetting<string>(SettingsEnum.VstsPat);
                    var apiVersion = _parent.GetSetting<string>(SettingsEnum.VstsApiVersion);
                    var workItemTypesJson = VstsHelper.GetWorkItemTypes(instance, projectName, personalAccessToken, apiVersion).Result;
                    if (!string.IsNullOrEmpty(workItemTypesJson))
                    {
                        dynamic workItemTypes = JObject.Parse(workItemTypesJson);
                        foreach (var workItemType in workItemTypes.value)
                        {
                            var workItemTypeName = (string) workItemType.name;
                            var workItemStateJson = VstsHelper.GetWorkItemTypeStates(workItemTypeName, instance, projectName, personalAccessToken, apiVersion).Result;
                            if (!string.IsNullOrEmpty(workItemStateJson))
                            {
                                dynamic workItemStates = JObject.Parse(workItemStateJson);
                                foreach (dynamic workItemState in workItemStates.value)
                                {
                                    var name = (string) workItemState.name;
                                    name = name.ToLowerInvariant();
                                    var color = (string) workItemState.color;
                                    if (!_workItemStates.ContainsKey(name))
                                        _workItemStates.Add(name, color);
                                }
                            }
                        }
                    }
                    _workItemStatesIsPopulated = true;
                }

            //var stateClass = state.ToLowerInvariant().Replace(" ", "-").Replace("--","-").Replace("--","-");
            //stateClass = "vsts-state vsts-state-" + stateClass;

            var style = string.Empty;
            var stateKey = state.ToLowerInvariant();
            if (_workItemStates.ContainsKey(stateKey))
            {
                var color = _workItemStates[stateKey];
                style = "border-color: #" + color + "; background-color: #" + color + ";";
            } 

            return "<div class=\"vsts-state\" style=\"" + style + "\"> </div>";
        }

        public string FixHtml(string html)
        {
            var instance = _parent.GetSetting<string>(SettingsEnum.VstsInstance);
            var instanceNormalized = instance.ToLowerInvariant();

            if (html.Contains("src=\"" + instance + "/"))
            {
                var changesMade = false;
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var images = doc.QuerySelectorAll("img");
                foreach (var image in images)
                {
                    var sourceAttribute = image.GetAttributeValue("src", string.Empty);
                    if (sourceAttribute.ToLowerInvariant().StartsWith(instanceNormalized))
                    {
                        image.SetAttributeValue("src", ImageLink + sourceAttribute);
                        changesMade = true;
                    }
                }

                if (changesMade)
                {
                    var outStream = new MemoryStream();
                    doc.Save(outStream);
                    html = StreamHelper.ToString(outStream);
                }
            }
            // TODO: Need to check if there are any broken image links or similar stuff in the HTML
            return html;
        }

        public string FixHtml(JToken html)
        {
            return FixHtml(html.ToString());
        }
    }
}