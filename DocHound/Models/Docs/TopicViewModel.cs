using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DocHound.Classes;
using DocHound.Interfaces;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
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
        public bool HideTableOfContents { get; set; }

        public TopicViewModel(string slug, HttpContext context)
        {
            HttpContext = context;
            Vsts = new VstsOutputHelper(this);
            CurrentSlug = slug;

            if (HttpContext.Request.Query.ContainsKey("contentonly"))
                RenderTopicOnly = context.Request.Query["contentonly"] == "true";
            if (HttpContext.Request.Query.ContainsKey("notoc"))
                HideTableOfContents = context.Request.Query["notoc"] == "true";
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
            if (dynamicToc.requireHttps != null) RequireHttps = dynamicToc.requireHttps;

            Topics = TableOfContentsHelper.BuildTocFromDynamicToc(dynamicToc, this, CurrentSlug, out List<TableOfContentsItem> flatTopicList);
            FlatTopics = flatTopicList;
            MainMenu = TableOfContentsHelper.BuildMainMenuStructureFromDynamicToc(dynamicToc);
            ThemeFolder = TableOfContentsHelper.GetThemeFolderFromDynamicToc(dynamicToc);
            ThemeColors = TableOfContentsHelper.GetThemeColorFromDynamicToc(dynamicToc);
            SyntaxTheme = TableOfContentsHelper.GetSyntaxThemeNameFromDynamicToc(dynamicToc);
            CustomCss = TableOfContentsHelper.GetCustomCssFromDynamicToc(dynamicToc);

            var matchingTopic = FlatTopics.FirstOrDefault(t => TopicHelper.SlugMatchesTopic(CurrentSlug, t));
            if (matchingTopic == null) matchingTopic = FlatTopics.FirstOrDefault(t => TopicHelper.SlugMatchesTopic(CurrentSlug, t, true));
            if (matchingTopic == null) matchingTopic = FlatTopics.FirstOrDefault(t => TopicHelper.LinkMatchesTopic(CurrentSlug, t));
            if (matchingTopic == null) matchingTopic = Topics.FirstOrDefault();

            SelectedTopic = matchingTopic;
            TableOfContentsHelper.EnsureExpanded(SelectedTopic);

            TocSettings = dynamicToc.settings;
            CurrentTopicSettings = SelectedTopic?.SettingsDynamic;
        }

        public bool RequireHttps { get; set; } = true;

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
                        if (string.IsNullOrEmpty(rawTopic.Type)) rawTopic.Type = TopicTypeHelper.GetTopicTypeFromLink(fullGitHubRawUrl);
                        if (TopicTypeHelper.IsMatch(rawTopic.Type, TopicTypeNames.Markdown) || TopicTypeHelper.IsMatch(rawTopic.Type, TopicTypeNames.Html))
                            rawTopic.OriginalContent = await WebClientEx.GetStringAsync(fullGitHubRawUrl);
                        else if (TopicTypeHelper.IsMatch(rawTopic.Type, TopicTypeNames.ImageUrl))
                            rawTopic.OriginalContent = fullGitHubRawUrl;
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
                        if ((TopicTypeHelper.IsMatch(rawTopic?.Type, TopicTypeNames.VstsWorkItemQuery) || TopicTypeHelper.IsMatch(rawTopic?.Type, TopicTypeNames.VstsWorkItemQueries)) && HttpContext.Request.Query.ContainsKey("workitemnumber"))
                        {
                            // The current node is a work item query, but we use it as a context to get the actual work item
                            var itemNumber = int.Parse(HttpContext.Request.Query["workitemnumber"]);
                            rawTopic.OriginalContent = await VstsHelper.GetWorkItemJson(itemNumber, GetSetting<string>(Settings.VstsInstance), GetSetting<string>(Settings.VstsPat));
                            rawTopic.Type = TopicTypeNames.VstsWorkItem;
                        }
                        else if (TopicTypeHelper.IsMatch(rawTopic?.Type, TopicTypeNames.VstsWorkItemQueries) && HttpContext.Request.Query.ContainsKey("queryid"))
                        {
                            // The current node is a list of work item queries, but we use it as a context to run the actual query
                            var queryId = HttpContext.Request.Query["queryid"];
                            var queryInfoJson = await VstsHelper.GetWorkItemQueriesJson(queryId, GetSetting<string>(Settings.VstsInstance), GetSetting<string>(Settings.VstsProjectName), GetSetting<string>(Settings.VstsPat));
                            dynamic queryInfo = JObject.Parse(queryInfoJson);
                            if (queryInfo != null)
                                Title = "Query: " + queryInfo.name;
                            rawTopic.OriginalContent = await VstsHelper.RunWorkItemQueryJson(queryId, GetSetting<string>(Settings.VstsInstance), GetSetting<string>(Settings.VstsProjectName), GetSetting<string>(Settings.VstsPat));
                            rawTopic.Type = TopicTypeNames.VstsWorkItemQuery;
                        }
                        else if (TopicTypeHelper.IsMatch(rawTopic?.Type, TopicTypeNames.VstsWorkItem))
                        {
                            // Plain work item node
                            var itemNumber = int.Parse(SelectedTopic.Link);
                            rawTopic.OriginalContent = await VstsHelper.GetWorkItemJson(itemNumber, GetSetting<string>(Settings.VstsInstance), GetSetting<string>(Settings.VstsPat));
                        }
                        else if (TopicTypeHelper.IsMatch(rawTopic?.Type, TopicTypeNames.VstsWorkItemQueries))
                        {
                            // Plain work item queries
                            rawTopic.OriginalContent = await VstsHelper.GetWorkItemQueriesJson(SelectedTopic.Link, GetSetting<string>(Settings.VstsInstance), GetSetting<string>(Settings.VstsProjectName), GetSetting<string>(Settings.VstsPat));
                            Title = SelectedTopic.Title;
                        }
                        else if (TopicTypeHelper.IsMatch(rawTopic?.Type, TopicTypeNames.VstsWorkItemQuery))
                        {
                            // Plain work item query
                            rawTopic.OriginalContent = await VstsHelper.RunWorkItemQueryJson(SelectedTopic.Link, GetSetting<string>(Settings.VstsInstance), GetSetting<string>(Settings.VstsProjectName), GetSetting<string>(Settings.VstsPat));
                            Title = SelectedTopic.Title;
                        }

                        Vsts.ImageLink = "/___FileProxy___?mode=" + RepositoryTypeNames.VstsWorkItemTracking + "&topic=" + CurrentSlug + "&path=";
                        break;
                }
            }

            var renderer = TopicRendererFactory.GetTopicRenderer(rawTopic);

            var intermediateHtml = renderer.RenderToHtml(rawTopic, imageRootUrl, this);
            intermediateHtml = await ProcessKavaTopic(intermediateHtml);
            intermediateHtml = ProcessBrokenImageLinks(intermediateHtml, imageRootUrl);
            Html = intermediateHtml;

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

        private string ProcessBrokenImageLinks(string html, string imageRootUrl)
        {
            if (!html.Contains("<img ")) return html;

            try
            {
                var changesMade = false;
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var images = doc.QuerySelectorAll("img");

                foreach (var image in images)
                {
                    var src = image.GetAttributeValue("src", string.Empty);
                    if (src.StartsWith("data:")) continue;
                    if (src.StartsWith("http://")) continue;
                    if (src.StartsWith("https://")) continue;
                    if (src.StartsWith("/___FileProxy___?")) continue;

                    changesMade = true;
                    image.SetAttributeValue("src", imageRootUrl + src);
                }

                if (changesMade)
                {
                    var stream = new MemoryStream();
                    doc.Save(stream);
                    html = StreamHelper.ToString(stream);
                }

                return html;
            }
            catch
            {
                return html;
            }
        }

        private async Task<string> ProcessKavaTopic(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var recs = new List<string>();

            if ((text.Contains("<kava-topic") || text.Contains("[kava-topic:")) && HttpContext.Request.Query.ContainsKey("recursion"))
            {
                var rec = StringHelper.Base64Decode(HttpContext.Request.Query["recursion"]);
                if (!string.IsNullOrEmpty(rec))
                    recs = rec.Split('&').ToList();
            }


            // Fixing up <kava-topic link="x" slug="x" /> syntax
            while (text.Contains("<kava-topic"))
            {
                if (recs.Contains(CurrentSlug))
                    text = await EmbeddKavaTopicTag(text, recs, true);
                else
                {
                    recs.Add(CurrentSlug);
                    text = await EmbeddKavaTopicTag(text, recs);
                }
            }

            // Fixing up [kava-topic:x] syntax
            while (text.Contains("[kava-topic:"))
            {
                if (recs.Contains(CurrentSlug))
                    text = await EmbeddKavaTopicPlaceholder(text, recs, true);
                else
                {
                    recs.Add(CurrentSlug);
                    text = await EmbeddKavaTopicPlaceholder(text, recs);
                }
            }

            return text;
        }

        private static readonly string[] AutoEncodeFileExtensions = {"css", "c", "cpp", "cs", "vb", "js", "ts", "py", "ps1", "fs", "cshtml", "vbhtml", "pl", "rb", "xml", "rss"};

        private static string AutoEncodeEmbedContent(string content, string url)
        {
            var urlLower = url.ToLowerInvariant();
            if (!urlLower.StartsWith("http://") && !urlLower.StartsWith("https://")) return content; // This was a local topic, so it should already be processed and encoded properly by DocHound

            var mustEncode = false;
            foreach (var fileExtension in AutoEncodeFileExtensions)
            {
                if (urlLower.EndsWith("." + fileExtension))
                {
                    mustEncode = true;
                    break;
                }
            }

            if (mustEncode)
                content = WebUtility.HtmlEncode(content);

            return content;
        }

        private async Task<string> EmbeddKavaTopicPlaceholder(string html, IEnumerable<string> recursions, bool killRecursion = false)
        {
            var recsEncoded = StringHelper.Base64Encode(string.Join('&', recursions));
            var sb = new StringBuilder();

            var tagStart = html.IndexOf("[kava-topic:", StringComparison.Ordinal);
            if (tagStart > -1)
            {
                sb.Append(html.Substring(0, tagStart));
                html = html.Substring(tagStart + 12);
                var slugEnd = html.IndexOf(']');
                if (slugEnd > -1)
                {
                    var slug = html.Substring(0, slugEnd);
                    html = html.Substring(slugEnd + 1);
                    if (!killRecursion)
                    {
                        if (!slug.ToLowerInvariant().StartsWith("http://") && !slug.ToLowerInvariant().StartsWith("https://"))
                            slug = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{slug}?contentonly=true&recursion={recsEncoded}";
                        var insertedHtml = await ContentSniffer.DownloadContent(DownloadMode.HttpGet, slug);
                        insertedHtml = AutoEncodeEmbedContent(insertedHtml, slug);
                        sb.Append(insertedHtml);
                    }
                    else
                        sb.Append($"<span class=\"kava-recursive-topic\">Recursive topic link detected: {slug}</span>");
                }
                sb.Append(html);
            }

            return sb.ToString();
        }

        private async Task<string> EmbeddKavaTopicTag(string html, IEnumerable<string> recursions, bool killRecursion = false)
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
                                    slug = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{slug}?contentonly=true&recursion={recsEncoded}";
                                var insertedHtml = await ContentSniffer.DownloadContent(DownloadMode.HttpGet, slug);
                                insertedHtml = AutoEncodeEmbedContent(insertedHtml, slug);
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
                        html = html.Substring(index2a + 6);
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
                                        slug = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{slug}?contentonly=true";
                                    var insertedHtml = await ContentSniffer.DownloadContent(DownloadMode.HttpGet, slug);
                                    insertedHtml = AutoEncodeEmbedContent(insertedHtml, slug);
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

            var value = SettingsHelper.GetSetting<T>(setting, TocSettings, CurrentTopicSettings, CurrentRequestRootSettings);

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
        public string ThemeCss => ThemeFolderRaw + "/Theme.css";

        public string ThemeColors { get; set; }

        public string ThemeColorsCss
        {
            get
            {
                var colors =  ThemeColors;
                if (colors.ToLowerInvariant().EndsWith(".css"))
                    return colors;
                return ThemeFolderRaw + "/" + colors + ".css";
            }
        }
    
        // TODO: This needs to be done more sophisticated by automatically figuring out what the theme supports
        public List<string> AvailableThemeColors
        {
            get
            {
                var allowableColors = GetSetting<string>(Settings.AllowableThemeColors);
                if (!string.IsNullOrEmpty(allowableColors))
                {
                    var list = allowableColors.Split(',').ToList();
                    for (var counter = 0; counter < list.Count; counter++)
                        list[counter] = list[counter].Trim();
                    return list;

                }
                return new List<string> {"Default", "Dark", "Sepia", "High Contrast"};
            }
        }

        // TODO: This needs to be done more sophisticated by automatically figuring out what the theme supports
        public string GetThemeColorCss(string colorLabel)
        {
            return ThemeFolderRaw + "/Theme-Colors-" + colorLabel.Replace(" ", "-") + ".css";
        }

        // TODO: This needs to be done more sophisticated by automatically figuring out what the theme supports
        public List<string> AvailableSyntaxThemeColors
        {
            get
            {
                var allowableSyntaxThemeColors = GetSetting<string>(Settings.AllowableSyntaxHighlightingThemes);
                if (!string.IsNullOrEmpty(allowableSyntaxThemeColors))
                {
                    var list = allowableSyntaxThemeColors.Split(',').ToList();
                    for (var counter = 0; counter < list.Count; counter++)
                        list[counter] = list[counter].Trim();
                    return list;

                }
                return new List<string>
                {
                    "Brown Paper", "Codepen Embed", "Color Brewer", "Dracula", "Dark", "Darkula", "Default",
                    "Dracula", "Far", "Foundation", "Github", "Github Gist", "GoogleCode", "Grayscale",
                    "Idea", "IR Black", "KavaDocs", "KavaDocsDark", "Kimbie.Dark", "Kimbie.Light", "Magula",
                    "Mono Blue", "Monokai", "Monokai Sublime", "Obsidian", "Paraiso Dark", "Paraiso Light",
                    "RailsCasts", "Rainbow", "Solarized Dark", "Solarized Light", "Sunburst", "Twilight",
                    "VS", "VS2015", "XCode", "ZenBurn"
                };
            }
        }

        // TODO: This needs to be done more sophisticated by automatically figuring out what the theme supports
        public string GetSyntaxThemeColorCss(string colorLabel)
        {
            return "/css/highlightjs/styles/" + colorLabel.Replace(" ", "-").ToLowerInvariant() + ".css";
        }


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

        public string GetMenu(MenuMode mode = MenuMode.Top)
        {
            if (MainMenu.Count < 1) return string.Empty;

            var sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var item in MainMenu)
            {
                sb.Append("<li>");
                sb.Append("<a class=\"area-link\" href=\"" + item.Link + "\">" + item.Title + "</a>");
                sb.Append("</li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        public string GetToCHtml() => HideTableOfContents ? string.Empty : TableOfContentsHelper.GetToCHtml(Topics, SelectedTopic);

        public List<TableOfContentsItem> Topics { get; set; } = new List<TableOfContentsItem>();
        public List<TableOfContentsItem> FlatTopics { get; set; }
        public List<OutlineItem> Outline { get; set; } = new List<OutlineItem>();

        private string _title;
        public string Title 
        {
            get
            {
                if (_title == null)
                    return SelectedTopic != null ? SelectedTopic.Title : CurrentSlug;
                return _title;
            }
            set{ _title = value; }
        }

        public string TitleAsId => TopicHelper.GetNormalizedName(Title.ToLowerInvariant());

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

        public string CurrentSlug { get; set; }
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

        public void SetRootSettingsForRequest(string settings)
        {
            try
            {
                dynamic dynamicSettings = JObject.Parse(settings);
                CurrentRequestRootSettings = dynamicSettings;
            }
            catch
            {
                CurrentRequestRootSettings = null;
            }
        }

        public dynamic CurrentRequestRootSettings { get; set; }
    }

    public class OutlineItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Tag { get; set; }
        public HtmlNode Node { get; set; }
    }

    public enum MenuMode
    {
        Top,
        Mobile
    }
}