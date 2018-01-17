using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DocHound.Classes;
using HtmlAgilityPack;
using Markdig;
using Markdig.Renderers;
using Microsoft.Extensions.Configuration;

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
            string tocJson = null;

            switch (RepositoryType)
            {
                case RepositoryTypes.GitHubRaw:
                    tocJson = await TableOfContentsHelper.GetTocJsonFromGitHubRaw(MasterUrlRaw);
                    LogoUrl = MasterUrlRaw + "_meta/_logo.png";
                    break;
                case RepositoryTypes.VisualStudioTeamSystemGit:
                    tocJson = await VstsHelper.GetTocJson(VstsInstance, VstsProjectName, VstsDocsFolder, VstsPat);
                    LogoUrl = "/___FileProxy___?mode=vstsgit&path=_meta/_logo.png";
                    break;
            }
            if (string.IsNullOrEmpty(tocJson)) return;

            var dynamicToc = TableOfContentsHelper.GetDynamicTocFromJson(tocJson);

            if (dynamicToc.title != null) RepositoryTitle = dynamicToc.title;
            if (dynamicToc.owner != null) Owner = dynamicToc.owner;

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
            var rawTopic = new TopicRaw {OriginalName = SelectedTopic.Title};

            var imageRootUrl = string.Empty;

            var normalizedLink = SelectedTopic.LinkPure.ToLowerInvariant();
            if (normalizedLink.StartsWith("https://") || normalizedLink.StartsWith("http://"))
            {
                // This is an absolute link, so we can just try to load it
                rawTopic.OriginalContent = await WebClientEx.GetStringAsync(SelectedTopic.Link);
                imageRootUrl = StringHelper.JustPath(SelectedTopic.Link) + "/";
            }
            else if (!string.IsNullOrEmpty(normalizedLink))
            {
                switch (RepositoryType)
                {
                    case RepositoryTypes.GitHubRaw:
                        var fullGitHubRawUrl = MasterUrlRaw + SelectedTopic.Link;
                        rawTopic.OriginalContent = await WebClientEx.GetStringAsync(fullGitHubRawUrl);
                        imageRootUrl = StringHelper.JustPath(fullGitHubRawUrl);
                        break;
                    case RepositoryTypes.VisualStudioTeamSystemGit:
                        if (!string.IsNullOrEmpty(SelectedTopic.LinkPure))
                            rawTopic.OriginalContent = await VstsHelper.GetFileContents(SelectedTopic.LinkPure, VstsInstance, VstsProjectName, VstsDocsFolder, VstsPat);
                        imageRootUrl = "/___FileProxy___?mode=vstsgit&path=";
                        if (SelectedTopic.LinkPure.Contains("/"))
                            imageRootUrl += StringHelper.JustPath(SelectedTopic.LinkPure) + "/";
                        break;
                }
            }

            var html = TopicRendererFactory.GetTopicRenderer(rawTopic).RenderToHtml(rawTopic, imageRootUrl);

            //// Post Processing of HTML
            //// TODO: This may either move to the client, or into a generic processor object
            //if (!string.IsNullOrEmpty(html))
            //{
            //    var htmlDoc = new HtmlDocument();
            //    htmlDoc.LoadHtml(html);
            //    var children = htmlDoc.DocumentNode.Descendants();
            //    foreach (var child in children)
            //        if (child.Name == "h1" || child.Name == "h2" || child.Name == "h3")
            //            Outline.Add(new OutlineItem
            //            {
            //                Title = child.InnerText,
            //                Link = TopicHelper.GetNormalizedName(child.InnerText),
            //                Tag = child.Name,
            //                Node = child
            //            });
            //    foreach (var item in Outline)
            //    {
            //        var anchor = HtmlNode.CreateNode("<a name=\"" + TopicHelper.GetNormalizedName(item.Title) + "\">");
            //        item.Node.ParentNode.InsertBefore(anchor, item.Node);
            //    }

            //    try 
            //    {
            //        using (var stream = new MemoryStream())
            //        {
            //            htmlDoc.Save(stream);
            //            Html = StreamHelper.ToString(stream);
            //        }
            //    }
            //    catch
            //    {
            //        Html = string.Empty;
            //    }
            //}
            Html = html;

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

        // private string GetFullExternalLink(string link)
        // {
        //     var realLink = GetRealLink(Topics, link);
        //     if (!string.IsNullOrEmpty(realLink))
        //     {
        //         var realLinkLower = realLink.ToLowerInvariant();
        //         if (!realLinkLower.StartsWith("http://") || !realLinkLower.StartsWith("https://"))
        //             realLink = MasterUrlRaw + realLink;
        //         return realLink;
        //     }
        //     return MasterUrlRaw + link.Replace(" ", "%20");
        // }

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
        public string RepositoryTitle { get; set; }
        public string Owner { get; set; }

        public string Html { get; set; }
        public string LogoUrl { get; set; }

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

        public static void SetStaticConfiguration(IConfiguration Configuration)
        {
            var repositoryType = Configuration["RepositoryType"].ToLowerInvariant();
            switch (repositoryType)
            {
                case "githubraw":
                    RepositoryType = RepositoryTypes.GitHubRaw;
                    var gitHubProject = Configuration["GitHubProject"];
                    if (string.IsNullOrEmpty(gitHubProject))
                    {
                        MasterUrl = Configuration["MasterUrl"];
                        MasterUrlRaw = MasterUrl.Replace("https://github.com", "https://raw.githubusercontent.com/");
                        if (!MasterUrl.Contains("/master/")) MasterUrl += "/master/";
                    }
                    else
                    {
                        MasterUrl = "https://github.com/" + gitHubProject;
                        MasterUrlRaw = "https://raw.githubusercontent.com/" + gitHubProject + "/master/";
                    }
                    break;

                case "vstsgit":
                    RepositoryType = RepositoryTypes.VisualStudioTeamSystemGit;
                    VstsInstance = Configuration["VSTSInstance"];
                    VstsProjectName = Configuration["VSTSProjectName"];
                    VstsDocsFolder = Configuration["VSTSDocsFolder"];
                    VstsPat = Configuration["VSTSPAT"];
                    break;
            }
        }

        public static string VstsPat { get; set; }

        public static string VstsDocsFolder { get; set; }

        public static string VstsProjectName { get; set; }

        public static string VstsInstance { get; set; }

        public static RepositoryTypes RepositoryType { get; set; }
    }

    public enum RepositoryTypes
    {
        GitHubRaw,
        VisualStudioTeamSystemGit
    }

    public class OutlineItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Tag { get; set; }
        public HtmlNode Node { get; set; }
    }

}