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

        public string Theme { get; set; } = "~/Themes/Default/Docs.css";

        private async Task GetHtmlContent()
        {
            try
            {
                var fullExternalLink = GetFullExternalLink(SelectedTopic.Title);
                var externalContent = await WebClientEx.GetStringAsync(fullExternalLink);

                var lowerExternalLink = fullExternalLink.ToLowerInvariant();

                string html;
                if (lowerExternalLink.EndsWith(".md"))
                    html = MarkdownToHtml(externalContent);
                else if (lowerExternalLink.EndsWith(".html") || lowerExternalLink.EndsWith(".htm"))
                    html = externalContent; // TODO: Extract everything within the body tag
                else
                    html = externalContent;


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

                using (var stream = new MemoryStream())
                {
                    htmlDoc.Save(stream);
                    Html = StreamHelper.ToString(stream);
                }
            }
            catch
            {
                Html = string.Empty; //"<p>Topic " + topicName + " not found on GitHub";
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

        private string MarkdownToHtml(string markdown)
        {
            // TODO: This uses all images as external links. We may need to handle that differently
            markdown = markdown.Replace("![](", "![](" + MasterUrlRaw);
            markdown = markdown.Replace("background: url('", "background: url('" + MasterUrlRaw);
            markdown = markdown.Replace("src=\"", "src=\"" + MasterUrlRaw);

            var builder = new MarkdownPipelineBuilder();
            BuildPipeline(builder);
            var pipeline = builder.Build();
            return Markdown.ToHtml(markdown, pipeline);
        }

        protected virtual MarkdownPipelineBuilder BuildPipeline(MarkdownPipelineBuilder builder)
        {
            builder = builder.UseYamlFrontMatter();
            builder = builder.UseAutoLinks();
            builder = builder.UseAutoIdentifiers();
            builder = builder.UseAbbreviations();
            builder = builder.UseEmojiAndSmiley();
            builder = builder.UseMediaLinks();
            builder = builder.UseListExtras();
            //builder = builder.UseFigures();
            builder = builder.UseTaskLists();
            //builder = builder.UseSmartyPants();
            //builder = builder.UsePragmaLines();
            builder = builder.UseGridTables();
            builder = builder.UsePipeTables();
            builder = builder.UseEmphasisExtras();
            builder = builder.UseFootnotes();
            builder = builder.UseCitations();
            builder = builder.UseGenericAttributes();

            return builder;
        }

        /// <summary>
        /// Create the entire Markdig pipeline and return the completed
        /// ready to process builder.
        /// </summary>
        /// <returns></returns>
        protected virtual MarkdownPipelineBuilder CreatePipelineBuilder()
        {
            var builder = new MarkdownPipelineBuilder();

            try
            {
                builder = BuildPipeline(builder);
            }
            catch (ArgumentException)
            {
            }

            return builder;
        }

        protected virtual IMarkdownRenderer CreateRenderer(TextWriter writer)
        {
            return new HtmlRenderer(writer);
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