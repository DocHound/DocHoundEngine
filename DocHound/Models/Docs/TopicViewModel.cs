using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DocHound.Classes;
using HtmlAgilityPack;
using Markdig;
using Markdig.Renderers;

namespace DocHound.Models.Docs
{
    public class TopicViewModel : IHaveTopics, IHaveSelectedTopic
    {
        private TableOfContentsItem _selectedTopic;

        public TopicViewModel(string topicName)
        {
            SelectedTopicTitle = topicName;
        }

        public async Task LoadData()
        {
            await BuildToc();
            await GetHtmlContent();
        }

        private async Task BuildToc()
        {
            var tocJson = await TableOfContentsHelper.GetTocJsonFromGitHubRaw(MasterUrlRaw);
            if (string.IsNullOrEmpty(tocJson)) return;

            var dynamicToc = TableOfContentsHelper.GetDynamicTocFromJson(tocJson);

            Topics = TableOfContentsHelper.BuildTocFromDynamicToc(dynamicToc, this, SelectedTopicTitle);
            MainMenu = TableOfContentsHelper.BuildMainMenuStructureFromDynamicToc(dynamicToc);
            ThemeFolder = TableOfContentsHelper.GetThemeFolderFromDynamicToc(dynamicToc);

            // TODO: Check for HTTPS if the TOC is configured to only accept https

            if (SelectedTopic == null && Topics.Count > 0)
            {
                SelectedTopic = Topics[0];
                if (SelectedTopicTitle == "index")
                    SelectedTopicTitle = SelectedTopic.Title;
            }
        }

        public string ThemeFolder { get; set; }

        public List<MainMenuItem> MainMenu { get; set; }

        private async Task GetHtmlContent()
        {
            var rawTopic = new TopicRaw{OriginalName = GetFullExternalLink(SelectedTopic.Title)};

            try
            {
                rawTopic.OriginalContent = await WebClientEx.GetStringAsync(rawTopic.OriginalName);
            }
            catch
            {
                rawTopic.OriginalContent = null;
            }

            var html = TopicRendererFactory.GetTopicRenderer(rawTopic).RenderToHtml(rawTopic);

            // Post Processing of HTML
            // TODO: This may either move to the client, or into a generic processor object
            if (!string.IsNullOrEmpty(html))
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                var children = htmlDoc.DocumentNode.Descendants();
                foreach (var child in children)
                    if (child.Name == "h1" || child.Name == "h2" || child.Name == "h3")
                        Outline.Add(new OutlineItem
                        {
                            Title = child.InnerText,
                            Link = TopicHelper.GetNormalizedName(child.InnerText),
                            Tag = child.Name,
                            Node = child
                        });
                foreach (var item in Outline)
                {
                    var anchor = HtmlNode.CreateNode("<a name=\"" + TopicHelper.GetNormalizedName(item.Title) + "\">");
                    item.Node.ParentNode.InsertBefore(anchor, item.Node);

                }

                try 
                {
                    using (var stream = new MemoryStream())
                    {
                        htmlDoc.Save(stream);
                        Html = StreamHelper.ToString(stream);
                    }
                }
                catch
                {
                    Html = string.Empty;
                }
            }


            if (string.IsNullOrEmpty(Html) && SelectedTopic != null)
            {
                var sb = new StringBuilder();
                sb.Append("<h1>" + SelectedTopic.Title + "</h1>");

                if (SelectedTopic.Topics.Count > 0)
                {
                    sb.Append("<ul>");
                    foreach (var topic in SelectedTopic.Topics)
                    {
                        sb.Append("<li>");
                        sb.Append("<a href=\"" + TopicHelper.GetNormalizedName(topic.Title) + "\">");
                        sb.Append(topic.Title);
                        sb.Append("</a>");
                        sb.Append("</li>");
                    }
                    sb.Append("</ul>");
                }

                Html = sb.ToString();
            }
        }

        private string GetFullExternalLink(string link)
        {
            var realLink = GetRealLink(Topics, link);
            if (!string.IsNullOrEmpty(realLink))
            {
                var realLinkLower = realLink.ToLowerInvariant();
                if (!realLinkLower.StartsWith("http://") || !realLinkLower.StartsWith("https://"))
                    realLink = MasterUrlRaw + realLink;
                return realLink;
            }
            return MasterUrlRaw + link.Replace(" ", "%20");
        }

        private static string GetRealLink(IEnumerable<TableOfContentsItem> topics, string name)
        {
            foreach (var topic in topics)
            {
                if (TopicHelper.LinkMatchesTopic(name, topic)) return topic.Link;
                var childLink = GetRealLink(topic.Topics, name);
                if (!string.IsNullOrEmpty(childLink)) return childLink;
            }
            return string.Empty;
        }

        public List<TableOfContentsItem> Topics { get; set; } = new List<TableOfContentsItem>();
        public List<OutlineItem> Outline { get; set; } = new List<OutlineItem>();

        public string Title => SelectedTopicTitle;

        public string Html { get; set; }

        public TableOfContentsItem SelectedTopic
        {
            get { return _selectedTopic; }
            set
            {
                _selectedTopic = value;
                if (_selectedTopic != null)
                    _selectedTopic.Expanded = true;
            }
        }

        public string SelectedTopicTitle { get; set; }
        public static string MasterUrlRaw { get; set; }
        public static string MasterUrl { get; set; }
        public string Link => string.Empty;
        public IHaveTopics Parent => null;
    }

    public class OutlineItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Tag { get; set; }
        public HtmlNode Node { get; set; }
    }

}