﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DocHound.Classes;
using DocHound.Interfaces;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;

namespace DocHound.Models.Docs
{
    public class TopicViewModel : IHaveTopics, IHaveSelectedTopic
    {
        private TableOfContentsItem _selectedTopic;
        private string _syntaxTheme;

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
            SyntaxTheme = TableOfContentsHelper.GetSyntaxThemeNameFromDynamicToc(dynamicToc);
            CustomCss = TableOfContentsHelper.GetCustomCssFromDynamicToc(dynamicToc);

            // TODO: Check for HTTPS if the TOC is configured to only accept https

            if (SelectedTopic == null && Topics.Count > 0)
            {
                SelectedTopic = Topics[0];
                if (SelectedTopicTitle == "index")
                    SelectedTopicTitle = SelectedTopic.Title;
            }

            var dynamicSettings = dynamicToc.settings;
            var dynamicSettings2 = SelectedTopic?.SettingsDynamic;

            Settings = new TocSettings(dynamicSettings, dynamicSettings2);
        }

        public TocSettings Settings { get; set; }

        public string CustomCss { get; set; }

        public string CustomCssFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(CustomCss)) return string.Empty;

                if (CustomCss.ToLowerInvariant().StartsWith("http://") || CustomCss.ToLowerInvariant().StartsWith("https://"))
                    return CustomCss;

                switch (RepositoryType)
                {
                    case RepositoryTypes.GitHubRaw:
                        return "/___FileProxy___?path=" + MasterUrlRaw + CustomCss;
                    case RepositoryTypes.VisualStudioTeamSystemGit:
                        return "/___FileProxy___?mode=vstsgit&path=" + CustomCss;
                }
                return String.Empty;
            }
        }

        public string SyntaxTheme
        {
            get
            {
                if (string.IsNullOrEmpty(_syntaxTheme)) return "kavadocs";
                return _syntaxTheme.ToLowerInvariant();
            }
            set { _syntaxTheme = value; }
        }

        public string ThemeFolder { get; set; }

        public List<MainMenuItem> MainMenu { get; set; }

        private async Task GetHtmlContent()
        {
            var rawTopic = new TopicInformation {OriginalName = SelectedTopic.Title, Type = SelectedTopic.Type};

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
                var repositoryType = RepositoryType;
                if (rawTopic.Type.ToLowerInvariant() == "vsts-workitem")
                    repositoryType = RepositoryTypes.VisualStudioTeamSystemWorkItems;

                switch (repositoryType)
                {
                    case RepositoryTypes.GitHubRaw:
                        var fullGitHubRawUrl = MasterUrlRaw + SelectedTopic.Link;
                        rawTopic.OriginalContent = await WebClientEx.GetStringAsync(fullGitHubRawUrl);
                        imageRootUrl = StringHelper.JustPath(fullGitHubRawUrl);
                        if (!string.IsNullOrEmpty(imageRootUrl) && !imageRootUrl.EndsWith("/")) imageRootUrl += "/";
                        break;
                    case RepositoryTypes.VisualStudioTeamSystemGit:
                        if (!string.IsNullOrEmpty(SelectedTopic.LinkPure))
                            rawTopic.OriginalContent = await VstsHelper.GetFileContents(SelectedTopic.LinkPure, VstsInstance, VstsProjectName, VstsDocsFolder, VstsPat);
                        imageRootUrl = "/___FileProxy___?mode=vstsgit&path=";
                        if (SelectedTopic.LinkPure.Contains("/"))
                            imageRootUrl += StringHelper.JustPath(SelectedTopic.LinkPure) + "/";
                        break;
                    case RepositoryTypes.VisualStudioTeamSystemWorkItems:
                        var itemNumber = int.Parse(SelectedTopic.Link);
                        rawTopic.OriginalContent = await VstsHelper.GetWorkItemJson(itemNumber, VstsInstance, VstsPat);
                        imageRootUrl = "/___FileProxy___?mode=vstswit&path=";
                        if (SelectedTopic.LinkPure.Contains("/"))
                            imageRootUrl += StringHelper.JustPath(SelectedTopic.LinkPure) + "/";
                        break;
                }
            }

            Html = TopicRendererFactory.GetTopicRenderer(rawTopic).RenderToHtml(rawTopic, imageRootUrl, Settings);

            if (string.IsNullOrEmpty(Html) && SelectedTopic != null)
            {
                var sb = new StringBuilder();
                sb.Append("<h1>" + SelectedTopic.Title + "</h1>");

                if (SelectedTopic.Topics.Count > 0)
                {
                    sb.Append("<ul>");
                    foreach (var topic in SelectedTopic.Topics)
                    {
                        sb.Append("<li class=\"kava-auto-link\">");
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
        VisualStudioTeamSystemGit,
        VisualStudioTeamSystemWorkItems
    }

    public class OutlineItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Tag { get; set; }
        public HtmlNode Node { get; set; }
    }

}