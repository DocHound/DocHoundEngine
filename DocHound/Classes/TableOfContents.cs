using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Newtonsoft.Json;

namespace DocHound.Classes
{
    public static class TableOfContentsHelper
    {
        public static dynamic GetDynamicTocFromJson(string tocJson)
        {
            return JsonConvert.DeserializeObject(tocJson);
        }

        public static List<MainMenuItem> BuildMainMenuStructureFromJson(string tocJson)
        {
            dynamic toc = GetDynamicTocFromJson(tocJson);
            return BuildMainMenuStructureFromDynamicToc(toc);
        }
        public static List<MainMenuItem> BuildMainMenuStructureFromDynamicToc(dynamic toc)
        {
            var menu = new List<MainMenuItem>();
            if (toc != null && toc.menu != null)
                foreach (var menuItem in toc.menu)
                {
                    var newItem = new MainMenuItem();
                    if (menuItem.title != null) newItem.Title = menuItem.title;
                    if (menuItem.link != null) newItem.Link = menuItem.link;
                    menu.Add(newItem);
                }
            return menu;
        }

        public static List<TableOfContentsItem> BuildTocFromDynamicToc(dynamic toc, IHaveTopics parent, string selectedTopicTitle)
        {
            var rootTopics = new List<TableOfContentsItem>();
            if (toc == null) return rootTopics;
            try
            {
                if (toc.topics != null)
                    AddTopics(toc.topics, rootTopics, parent, parent, selectedTopicTitle);
            }
            catch
            {
                // ignored
            }
            return rootTopics;
        }

        public static List<TableOfContentsItem> BuildTocFromJson(string tocJson, IHaveTopics parent, string selectedTopicTitle)
        {
            dynamic toc = GetDynamicTocFromJson(tocJson);
            return BuildTocFromDynamicToc(toc, parent, selectedTopicTitle);
        }

        private static void AddTopics(IEnumerable<dynamic> topics, ICollection<TableOfContentsItem> parentTopics, IHaveTopics parent, IHaveTopics root, string selectedTopicTitle)
        {
            foreach (var topic in topics)
            {
                var newTopic = new TableOfContentsItem(parent) {Title = topic.title ?? "Unknown topic"};
                if (topic.link != null) newTopic.Link = topic.link;
                if (topic.isExpanded != null) newTopic.Expanded = topic.isExpanded;
                if (topic.keywords != null) newTopic.KeywordsRaw = topic.keywords;

                if (topic.seeAlso != null)
                {
                    var seeAlsos = ((string) topic.seeAlso).Split('\n'); // Multiple see-also links separated by new-line
                    foreach (var seeAlso in seeAlsos)
                    {
                        var seeAlsoParts = seeAlso.Split('|'); // If title and link are different, they are separated by |
                        if (seeAlsoParts.Length > 1)
                            newTopic.SeeAlso.Add(new SeeAlsoTopic {Title = seeAlsoParts[0], Link = seeAlsoParts[1]});
                        else if (seeAlsoParts.Length > 0)
                            newTopic.SeeAlso.Add(new SeeAlsoTopic {Title = seeAlsoParts[0], Link = seeAlsoParts[0]});
                    }
                }

                parentTopics.Add(newTopic);

                if (TopicHelper.LinkMatchesTopic(selectedTopicTitle, newTopic))
                {
                    if (root is IHaveSelectedTopic selectedTopicParent)
                        selectedTopicParent.SelectedTopic = newTopic;
                    EnsureExpanded(newTopic);
                }

                if (topic.topics != null)
                    AddTopics(topic.topics, newTopic.Topics, newTopic, root, selectedTopicTitle);
            }
        }

        /// <summary>
        /// Gets the toc json from git hub using RAW URLs (rather than the Git API).
        /// </summary>
        /// <param name="gitHubRawUrl">The git hub raw URL.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        public static async Task<string> GetTocJsonFromGitHubRaw(string gitHubRawUrl)
        {
            var content = await ContentSniffer.DownloadContent(DownloadMode.HttpGet, gitHubRawUrl + "_toc.json");
            if (content == null) content = await ContentSniffer.DownloadContent(DownloadMode.HttpGet, gitHubRawUrl + "_meta/_toc.json");
            if (content != null) return content.ToString();

            // The TOC file didn't exist, so we try to scrape the repository
            // TODO: Finish this
            //var html = await WebClientEx.GetStringAsync(gitHubRawUrl);
            //var htmlDoc = new HtmlDocument();
            //htmlDoc.LoadHtml(html);
            //var children = htmlDoc.QuerySelectorAll("tr.js-navigation-item");

            return string.Empty;
        }

        public static string GetToCHtml(List<TableOfContentsItem> toc, string selectedLink)
        {
            var sb = new StringBuilder();
            AddTocItems(sb, toc, 0, selectedLink);
            return sb.ToString();
        }

        private static void AddTocItems(StringBuilder sb, IEnumerable<TableOfContentsItem> topics, int indentLevel, string selectedLink, TableOfContentsItem parentTopic = null)
        {
            var ulClasses = "topicList topicListLevel" + indentLevel;
            if (parentTopic != null) ulClasses += parentTopic.Expanded ? " topicExpanded" : " topicCollapsed";
            sb.Append("<ul class=\"" + ulClasses + "\">");

            foreach (var topic in topics)
            {
                var className = "topicLink topicLevel" + indentLevel;

                if (TopicHelper.LinkMatchesTopic(selectedLink, topic)) className += " selectedTopic";

                sb.Append("<li class=\"" + className + "\">");

                if (topic.Topics.Count > 0)
                {
                    sb.Append("<a href=\"/" + TopicHelper.GetNormalizedName(topic.Title) + "\">" + topic.Title + "</a>");
                    var keywords = topic.Keywords;
                    if (!string.IsNullOrEmpty(keywords))
                        sb.Append("<span style=\"display: none;\">" + topic.Keywords + "</span>");
                    sb.Append("<span class=\"caret " + (topic.Expanded ? "caretExpanded" : "caretCollapsed") + "\"><svg xmlns=\"http://www.w3.org/2000/svg\" focusable=\"false\" viewBox=\"0 0 24 24\"><path fill=\"black\" stroke=\"white\" d=\"M8.59 16.34l4.58-4.59-4.58-4.59L10 5.75l6 6-6 6z\"></path></svg></span>");

                    AddTocItems(sb, topic.Topics, indentLevel + 1, selectedLink, topic);
                }
                else
                {
                    var keywords = topic.Keywords;
                    sb.Append("<a href=\"/" + TopicHelper.GetNormalizedName(topic.Title) + "\">" + topic.Title + "</a>");
                    if (!string.IsNullOrEmpty(keywords))
                        sb.Append("<span style=\"display: none;\">" + topic.Keywords + "</span>");
                }

                sb.Append("</li>");
            }


            sb.Append("</ul>");

        }

        //private static void CheckTocItems(IEnumerable<TableOfContentsItem> topics, string selectedLink)
        //{
        //    var tableOfContentsItems = topics as TableOfContentsItem[] ?? topics.ToArray();
        //    foreach (var topic in tableOfContentsItems)
        //    {
        //        if (TopicHelper.LinkMatchesTopic(selectedLink, topic))
        //        {
        //            var parentTableOfContentsItem = topic.Parent as TableOfContentsItem;
        //            if (parentTableOfContentsItem != null)
        //                EnsureExpanded(parentTableOfContentsItem);
        //        }
        //        CheckTocItems(topic.Topics, selectedLink);
        //    }
        //}

        private static void EnsureExpanded(TableOfContentsItem item)
        {
            if (item == null) return;
            item.Expanded = true;

            var parentTableOfContentsItem = item.Parent as TableOfContentsItem;
            if (parentTableOfContentsItem != null)
                EnsureExpanded(parentTableOfContentsItem);
        }

        public static string GetThemeFolderFromDynamicToc(dynamic dynamicToc)
        {
            if (dynamicToc.theme != null)
            {
                if (dynamicToc.theme.standardTheme != null)
                    return "~/wwwroot/Themes/" + ((string)dynamicToc.theme.standardTheme).Trim();

                // TODO: Allow for more theme settings...
            }

            return "~/wwwroot/Themes/Default";
        }

        public static string GetSyntaxThemeNameFromDynamicToc(dynamic dynamicToc)
        {
            if (dynamicToc.theme != null)
                if (dynamicToc.theme.syntaxTheme != null)
                    return ((string) dynamicToc.theme.syntaxTheme).Trim();

            return string.Empty;
        }

        public static string GetCustomCssFromDynamicToc(dynamic dynamicToc)
        {
            if (dynamicToc.theme != null)
                if (dynamicToc.theme.customCss != null)
                    return ((string)dynamicToc.theme.customCss).Trim();

            return string.Empty;
        }
    }

    public interface IHaveTopics
    {
        List<TableOfContentsItem> Topics { get; }
        string Title { get; }
        string Link { get; }
        IHaveTopics Parent { get; }
    }

    public class TableOfContentsItem : IHaveTopics
    {
        public TableOfContentsItem(IHaveTopics parent)
        {
            Parent = parent;
        }

        public IHaveTopics Parent { get; }

        public string Title { get; set; }

        private string _link = string.Empty;

        public string Link
        {
            get { return string.IsNullOrEmpty(_link) ? TopicHelper.GetNormalizedName(Title) : _link; }
            set { _link = value; }
        }
        public string LinkPure => _link;

        public bool Expanded { get; set; }

        public List<TableOfContentsItem> Topics { get; } = new List<TableOfContentsItem>();
        public List<SeeAlsoTopic> SeeAlso { get; } = new List<SeeAlsoTopic>();

        private TableOfContentsItem _previousTopic;
        public TableOfContentsItem PreviousTopic => _previousTopic ?? (_previousTopic = TopicHelper.GetPreviousTopic(this));

        private TableOfContentsItem _nextTopic;
        public TableOfContentsItem NextTopic => _nextTopic ?? (_nextTopic = TopicHelper.GetNextTopic(this, this));
        public string KeywordsRaw { get; set; }

        public string Keywords => KeywordsRaw?.Replace("\r", ", ").Replace("\n", ", ").Replace(", , ", ", ");

        public override string ToString() => Title + " [" + Link + "]";
    }

    public interface IHaveSelectedTopic
    {
        TableOfContentsItem SelectedTopic { get; set; }
    }

    public class SeeAlsoTopic
    {
        public string Link { get; set; }
        public string Title { get; set; }
    }

    public static class TopicHelper
    {
        public static string GetNormalizedName(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;

            var normalizedName = name;
            normalizedName = normalizedName.Replace(" ", "-");
            normalizedName = normalizedName.Replace("%20", "-");
            normalizedName = normalizedName.Replace("/", "-");
            normalizedName = normalizedName.Replace(",", "");
            normalizedName = normalizedName.Replace("(", "");
            normalizedName = normalizedName.Replace(")", "");
            normalizedName = normalizedName.Replace("?", "");
            normalizedName = normalizedName.Replace(":", "");
            return normalizedName;
        }

        public static bool LinkMatchesTopic(string link, IHaveTopics topic)
        {
            var normalizedName = GetNormalizedName(link);
            if (GetNormalizedName(topic.Title) == normalizedName) return true;

            var normalizedLink = GetNormalizedName(topic.Link);
            if (normalizedLink == normalizedName) return true;
            var normalizedLinkParts = normalizedLink.Split('.');
            if (normalizedLinkParts.Length > 0 && normalizedLinkParts[0] == normalizedName) return true;

            return false;
        }

        public static TableOfContentsItem GetPreviousTopic(IHaveTopics topic)
        {
            var parentItem = topic.Parent;
            if (parentItem == null) return null;

            for (var counter = 0; counter < parentItem.Topics.Count; counter++)
                if (parentItem.Topics[counter] == topic)
                {
                    if (counter > 0)
                    {
                        var previousTopic = parentItem.Topics[counter - 1];
                        while (previousTopic.Topics.Count > 0)
                            previousTopic = previousTopic.Topics.Last();
                        return previousTopic;
                    }
                    var tableOfContentsParentItem = parentItem as TableOfContentsItem;
                    if (tableOfContentsParentItem != null)
                        return tableOfContentsParentItem; // Since it is the first item at the current level, we return the parent node
                }

            return null;
        }

        public static TableOfContentsItem GetNextTopic(IHaveTopics topic, IHaveTopics originalTopic)
        {
            if (topic == originalTopic && topic.Topics.Count > 0) return topic.Topics[0];

            var parentItem = topic.Parent;
            if (parentItem == null) return null;

            for (var counter = 0; counter < parentItem.Topics.Count; counter++)
                if (parentItem.Topics[counter] == topic)
                {
                    if (counter < parentItem.Topics.Count - 1) return parentItem.Topics[counter + 1];
                    return GetNextTopic(parentItem, originalTopic); // Since it is the last topic in the list, we try to find the next node in the parent
                }

            return null;
        }
    }

    public class MainMenuItem
    {
        public string Title { get; set; } = "Home";
        public string Link { get; set; } = "/";
    }
}
