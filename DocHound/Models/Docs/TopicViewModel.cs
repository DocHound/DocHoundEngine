using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocHound.Classes;
using DocHound.Interfaces;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace DocHound.Models.Docs
{
    public class TopicViewModel : IHaveTopics, IHaveSelectedTopic, ISettingsProvider
    {
        private TableOfContentsItem _selectedTopic;
        private string _syntaxTheme;
        public HttpContext HttpContext { get; }
        public VstsOutputHelper Vsts { get; }

        public bool RenderTopicOnly { get; set; }

        public TopicViewModel(string topicName, HttpContext context)
        {
            HttpContext = context;
            Vsts = new VstsOutputHelper(this);
            SelectedTopicName = topicName;

            if (HttpContext.Request.Query.ContainsKey("contentonly"))
                RenderTopicOnly = context.Request.Query["contentonly"] == "true";
        }

        public async Task LoadData(bool buildToc = true, bool buildHtml = true)
        {
            if (buildToc) await BuildToc();
            if (buildHtml) await GetHtmlContent();
        }

        private async Task BuildToc()
        {
            string tocJson = null;

            var repositoryType = RepositoryTypeHelper.GetTypeFromTypeName(GetSetting<string>(Settings.RepositoryType));
            switch (repositoryType)
            {
                case RepositoryTypes.GitHubRaw:
                    tocJson = await TableOfContentsHelper.GetTocJsonFromGitHubRaw(GitHubMasterUrlRaw);
                    LogoUrl = GitHubMasterUrlRaw + "_meta/_logo.png";
                    break;
                case RepositoryTypes.VstsGit:
                    tocJson = await VstsHelper.GetTocJson(GetSetting<string>(Settings.VstsInstance), GetSetting<string>(Settings.VstsProjectName), GetSetting<string>(Settings.VstsDocsFolder), GetSetting<string>(Settings.VstsPat));
                    LogoUrl = "/___FileProxy___?mode=vstsgit&path=_meta/_logo.png";
                    break;
            }
            if (string.IsNullOrEmpty(tocJson)) return;

            var dynamicToc = TableOfContentsHelper.GetDynamicTocFromJson(tocJson);

            if (dynamicToc.title != null) RepositoryTitle = dynamicToc.title;
            if (dynamicToc.owner != null) Owner = dynamicToc.owner;

            Topics = TableOfContentsHelper.BuildTocFromDynamicToc(dynamicToc, this, SelectedTopicName);
            MainMenu = TableOfContentsHelper.BuildMainMenuStructureFromDynamicToc(dynamicToc);
            ThemeFolder = TableOfContentsHelper.GetThemeFolderFromDynamicToc(dynamicToc);
            SyntaxTheme = TableOfContentsHelper.GetSyntaxThemeNameFromDynamicToc(dynamicToc);
            CustomCss = TableOfContentsHelper.GetCustomCssFromDynamicToc(dynamicToc);

            // TODO: Check for HTTPS if the TOC is configured to only accept https

            if (SelectedTopic == null && Topics.Count > 0)
            {
                SelectedTopic = Topics[0];
                if (SelectedTopicName == "index")
                    SelectedTopicName = SelectedTopic.Title;
            }

            TocSettings = dynamicToc.settings;
            CurrentTopicSettings = SelectedTopic?.SettingsDynamic;
        }

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
                var repositoryType = RepositoryTypeHelper.GetTypeFromTypeName(GetSetting<string>(Settings.RepositoryType));

                // Even if the overall repository type is something else, we will switch to different repository access for specific node types, 
                // as they may point to other repositories or require different APIs even within the same repository
                if (TopicTypeHelper.IsVstsWorkItemType(rawTopic?.Type))
                    repositoryType = RepositoryTypes.VstsWorkItemTracking;

                switch (repositoryType)
                {
                    case RepositoryTypes.GitHubRaw:
                        var fullGitHubRawUrl = GitHubMasterUrlRaw + SelectedTopic.Link;
                        rawTopic.OriginalContent = await WebClientEx.GetStringAsync(fullGitHubRawUrl);
                        imageRootUrl = StringHelper.JustPath(fullGitHubRawUrl);
                        if (!string.IsNullOrEmpty(imageRootUrl) && !imageRootUrl.EndsWith("/")) imageRootUrl += "/";
                        break;

                    case RepositoryTypes.VstsGit:
                        if (!string.IsNullOrEmpty(SelectedTopic.LinkPure))
                            rawTopic.OriginalContent = await VstsHelper.GetFileContents(SelectedTopic.LinkPure, GetSetting<string>(Settings.VstsInstance), GetSetting<string>(Settings.VstsProjectName), GetSetting<string>(Settings.VstsDocsFolder), GetSetting<string>(Settings.VstsPat));
                        imageRootUrl = "/___FileProxy___?mode=" + RepositoryTypeNames.VstsGit + "&path=";
                        if (SelectedTopic.LinkPure.Contains("/"))
                            imageRootUrl += StringHelper.JustPath(SelectedTopic.LinkPure) + "/";
                        break;

                    case RepositoryTypes.VstsWorkItemTracking:
                        if (TopicTypeHelper.IsMatch(rawTopic?.Type, TopicTypeNames.VstsWorkItemQuery) && HttpContext.Request.Query.ContainsKey("workitemnumber"))
                        {
                            // The current node is a work item query, but we use it as a context to get the actual work item
                            var itemNumber = int.Parse(HttpContext.Request.Query["workitemnumber"]);
                            rawTopic.OriginalContent = await VstsHelper.GetWorkItemJson(itemNumber, GetSetting<string>(Settings.VstsInstance), GetSetting<string>(Settings.VstsPat));
                            rawTopic.Type = TopicTypeNames.VstsWorkItem;
                        }
                        else if (TopicTypeHelper.IsMatch(rawTopic?.Type, TopicTypeNames.VstsWorkItem))
                        {
                            // Plain work item node
                            var itemNumber = int.Parse(SelectedTopic.Link);
                            rawTopic.OriginalContent = await VstsHelper.GetWorkItemJson(itemNumber, GetSetting<string>(Settings.VstsInstance), GetSetting<string>(Settings.VstsPat));
                        }
                        else if (TopicTypeHelper.IsMatch(rawTopic?.Type, TopicTypeNames.VstsWorkItemQueries))
                            // Plain work item queries
                            rawTopic.OriginalContent = await VstsHelper.GetWorkItemQueriesJson(SelectedTopic.Link, GetSetting<string>(Settings.VstsInstance), GetSetting<string>(Settings.VstsProjectName), GetSetting<string>(Settings.VstsPat));
                        else if (TopicTypeHelper.IsMatch(rawTopic?.Type, TopicTypeNames.VstsWorkItemQuery))
                            // Plain work item query
                            rawTopic.OriginalContent = await VstsHelper.RunWorkItemQueryJson(SelectedTopic.Link, GetSetting<string>(Settings.VstsInstance), GetSetting<string>(Settings.VstsProjectName), GetSetting<string>(Settings.VstsPat));

                        Vsts.ImageLink = "/___FileProxy___?mode=" + RepositoryTypeNames.VstsWorkItemTracking + "&topic=" + SelectedTopicName + "&path=";
                        break;
                }
            }

            var renderer = TopicRendererFactory.GetTopicRenderer(rawTopic);

            Html = renderer.RenderToHtml(rawTopic, imageRootUrl, this);
            while (Html.Contains("<kava-topic"))
            {
                var canEmbedTopic = true;
                var recs = new List<string>();
                if (HttpContext.Request.Query.ContainsKey("recursion"))
                {
                    var rec = StringHelper.Base64Decode(HttpContext.Request.Query["recursion"]);
                    if (!string.IsNullOrEmpty(rec))
                    {
                        recs = rec.Split('&').ToList();
                        if (recs.Contains(SelectedTopicName)) // We are recursive if this is true
                            canEmbedTopic = false;
                    }
                }

                if (!canEmbedTopic)
                {
                    Html = await EmbeddKavaTopic(Html, recs, true);
                }
                else
                {
                    recs.Add(SelectedTopicName);
                    Html = await EmbeddKavaTopic(Html, recs);
                }
            }

            Json = renderer.RenderToJson(rawTopic, imageRootUrl, this);
            TemplateName = renderer.GetTemplateName(rawTopic, TemplateName, this);

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

        private async Task<string> EmbeddKavaTopic(string html, IEnumerable<string> recursions, bool killRecursion = false)
        {
            var recsEncoded = StringHelper.Base64Encode(string.Join('&', recursions));
            var sb = new StringBuilder();
            var tagStart = html.IndexOf("<kava-topic", StringComparison.Ordinal);
            if (tagStart > -1)
            {
                sb.Append(html.Substring(0, tagStart - 1));
                html = html.Substring(tagStart + 11);
                var index2 = html.IndexOf("slug=\"", StringComparison.Ordinal);
                if (index2 > -1)
                {
                    html = html.Substring(index2 + 6);
                    var index3 = html.IndexOf("\"", StringComparison.Ordinal);
                    if (index3 > -1)
                    {
                        var slug = html.Substring(0, index3);
                        html = html.Substring(index3 + 1);
                        var index4 = html.IndexOf("/>", StringComparison.Ordinal);
                        if (index4 > -1)
                        {
                            html = html.Substring(index4 + 2);
                            if (!killRecursion)
                            {
                                if (!slug.ToLowerInvariant().StartsWith("http://") && !slug.ToLowerInvariant().StartsWith("https://"))
                                    slug = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/" + slug;
                                var insertedHtml = await ContentSniffer.DownloadContent(DownloadMode.HttpGet, $"{slug}?contentonly=true&recursion={recsEncoded}");
                                sb.Append(insertedHtml);
                            }
                            else
                                sb.Append($"<span class=\"kava-recursive-topic\">Recursive topic link detected: {slug}</span>");
                        }
                        sb.Append(html);
                    }
                }
                else
                {
                    var index2a = html.IndexOf("link=\"", StringComparison.Ordinal);
                    if (index2a > -1)
                    {
                        html = html.Substring(index2 + 6);
                        var index3 = html.IndexOf("\"", StringComparison.Ordinal);
                        if (index3 > -1)
                        {
                            var slug = html.Substring(0, index3);
                            html = html.Substring(index3 + 1);
                            var index4 = html.IndexOf("/>", StringComparison.Ordinal);
                            if (index4 > -1)
                            {
                                html = html.Substring(index4 + 2);
                                if (!killRecursion)
                                {
                                    if (!slug.ToLowerInvariant().StartsWith("http://") && !slug.ToLowerInvariant().StartsWith("https://"))
                                        slug = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/" + slug;
                                    var insertedHtml = await ContentSniffer.DownloadContent(DownloadMode.HttpGet, $"{slug}?contentonly=true");
                                    sb.Append(insertedHtml);
                                }
                                else
                                    sb.Append($"<span class=\"kava-recursive-topic\">Recursive topic link detected: {slug}</span>");
                            }
                            sb.Append(html);
                        }
                    }
                }
            }

            return sb.ToString();
        }

        private readonly Dictionary<Settings, object> _cachedSettings = new Dictionary<Settings, object>();

        public T GetSetting<T>(Settings setting)
        {
            if (_cachedSettings.ContainsKey(setting)) return (T) _cachedSettings[setting];

            var value = SettingsHelper.GetSetting<T>(setting, TocSettings, CurrentTopicSettings);

            if (CurrentTopicSettings != null)
                // We can only cache if we already have current topic settings, otherwise, those may just not have been loaded yet, and may thus still be loaded and needed later.
                _cachedSettings.Add(setting, value);

            return value;
        }

        public bool IsSettingSpecified(Settings setting) => SettingsHelper.IsSettingSet(setting, TocSettings, CurrentTopicSettings);

        public void OverrideSetting<T>(Settings setting, T value)
        {
            if (_cachedSettings.ContainsKey(setting))
                _cachedSettings[setting] = value;
            else
                _cachedSettings.Add(setting, value);
        }

        public string CustomCss { get; set; }

        public string CustomCssFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(CustomCss)) return string.Empty;

                if (CustomCss.ToLowerInvariant().StartsWith("http://") || CustomCss.ToLowerInvariant().StartsWith("https://"))
                    return CustomCss;

                var repositoryType = RepositoryTypeHelper.GetTypeFromTypeName(GetSetting<string>(Settings.RepositoryType));
                switch (repositoryType)
                {
                    case RepositoryTypes.GitHubRaw:
                        return "/___FileProxy___?path=" + GitHubMasterUrlRaw + CustomCss;
                    case RepositoryTypes.VstsGit:
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
        public string ThemeFolderRaw => ThemeFolder.Replace("~/wwwroot/", "/");

        private string _templateName = "TopicDefault";

        public string TemplateName
        {
            get { return _templateName; }
            set
            {
                if (value.ToLowerInvariant().EndsWith(".cshtml"))
                    throw new ArgumentException("Templates name should not include a file extension, such as '.cshtml'");
                _templateName = value;
            }
        }

        public List<MainMenuItem> MainMenu { get; set; }

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

        public string Title => SelectedTopicName;
        public string RepositoryTitle { get; set; }
        public string Owner { get; set; }

        public string Html { get; set; }
        public string Json { get; set; }

        public dynamic JsonObject
        {
            get
            {
                if (string.IsNullOrEmpty(Json)) return null;

                try
                {
                    return JObject.Parse(Json);
                }
                catch
                {
                    return null;
                }
            }
        }

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

        public string SelectedTopicName { get; set; }
        public string Link => string.Empty;
        public string Slug => string.Empty;
        public IHaveTopics Parent => null;

        public dynamic TocSettings { get; private set; }
        public dynamic CurrentTopicSettings { get; private set; }

        private string _gitHubMasterUrlRaw = null;

        public string GitHubMasterUrlRaw
        {
            get
            {
                var repositoryType = RepositoryTypeHelper.GetTypeFromTypeName(GetSetting<string>(Settings.RepositoryType));
                if (repositoryType != RepositoryTypes.GitHubRaw) return string.Empty;

                if (_gitHubMasterUrlRaw == null)
                {
                    if (string.IsNullOrEmpty(GetSetting<string>(Settings.GitHubProject)))
                    {
                        var gitHubMasterUrlRaw = GitHubMasterUrl.Replace("https://github.com", "https://raw.githubusercontent.com/");
                        if (!GitHubMasterUrl.Contains("/master/")) gitHubMasterUrlRaw += "/master/";
                        _gitHubMasterUrlRaw = gitHubMasterUrlRaw;
                    }
                    else
                        _gitHubMasterUrlRaw = "https://raw.githubusercontent.com/" + GetSetting<string>(Settings.GitHubProject) + "/master/";
                }

                return _gitHubMasterUrlRaw;
            }
        }

        private string _gitHubMasterUrl = null;

        public string GitHubMasterUrl
        {
            get
            {
                var repositoryType = RepositoryTypeHelper.GetTypeFromTypeName(GetSetting<string>(Settings.RepositoryType));
                if (repositoryType != RepositoryTypes.GitHubRaw) return string.Empty;

                if (_gitHubMasterUrl == null)
                {
                    if (string.IsNullOrEmpty(GetSetting<string>(Settings.GitHubProject)))
                        _gitHubMasterUrl = GetSetting<string>(Settings.GitHubMasterUrl);
                    else
                        _gitHubMasterUrl = "https://github.com/" + GetSetting<string>(Settings.GitHubProject);
                }

                return _gitHubMasterUrl;
            }
        }
    }

    public class OutlineItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Tag { get; set; }
        public HtmlNode Node { get; set; }
    }
}