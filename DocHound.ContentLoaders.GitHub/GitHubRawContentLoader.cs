using DocHound.Interfaces;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocHound.ContentLoaders.GitHub
{
    public class GitHubRawContentLoader : ContentLoaderBase
    {
        public override async Task<string> GetTocJsonAsync()
        {
            var uri = GitHubMasterUrlRaw;
            var content = await ContentSniffer.DownloadContent(DownloadMode.HttpGet, uri + "_toc.json");
            if (string.IsNullOrEmpty(content)) content = await ContentSniffer.DownloadContent(DownloadMode.HttpGet, uri + "_meta/_toc.json");
            if (!string.IsNullOrEmpty(content)) return content;

            // The TOC file didn't exist, so we try to scrape the repository
            var gitHubRegularUrl = uri.Replace("raw.githubusercontent.com", "github.com");
            if (gitHubRegularUrl.EndsWith("/master/")) gitHubRegularUrl = gitHubRegularUrl.Substring(0, gitHubRegularUrl.Length - 7);
            return await CrawlGitHubForToc(gitHubRegularUrl);
        }

        public override async Task<string> GetContentStringAsync(string link, string topicTypeName)
        {
            if (TopicTypeHelper.IsMatch(topicTypeName, TopicTypeNames.Markdown) || TopicTypeHelper.IsMatch(topicTypeName, TopicTypeNames.Html))
            {
                var fullGitHubRawUrl = GitHubMasterUrlRaw + link;
                return await WebGet(fullGitHubRawUrl);
            }
            else if (TopicTypeHelper.IsMatch(topicTypeName, TopicTypeNames.ImageUrl))
                return GitHubMasterUrlRaw + link;

            return string.Empty;
        }

        public override string GetImageRootUrl(string contentUri)
        {
            var imageRootUrl = JustPath(contentUri);
            if (!string.IsNullOrEmpty(imageRootUrl) && !imageRootUrl.EndsWith("/")) imageRootUrl += "/";
            return imageRootUrl;
        }

        public override string GetLogoUrl()
        {
            var logoUrl = GetSetting<string>(Settings.LogoPath);
            var logoUrlLower = logoUrl.ToLowerInvariant();
            var logoUrlIsAbsolute = true;
            if (!logoUrl.StartsWith("http://") && !logoUrl.StartsWith("https://")) logoUrlIsAbsolute = false;
            if (!logoUrlIsAbsolute)
                logoUrl = GitHubMasterUrlRaw + logoUrl;
            return logoUrl;
        }

        private string _gitHubMasterUrlRaw = null;

        public string GitHubMasterUrlRaw
        {
            get
            {
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

        private static readonly Dictionary<string, string> CrawledGitHubRepositories = new Dictionary<string, string>();

        private async Task<string> CrawlGitHubForToc(string url, StringBuilder sb = null, string rootUrl = null)
        {
            lock (CrawledGitHubRepositories)
                if (CrawledGitHubRepositories.ContainsKey(url)) return CrawledGitHubRepositories[url];

            if (rootUrl == null) rootUrl = url;

            var sbWasNull = false;
            if (sb == null)
            {
                sb = new StringBuilder();
                sb.Append($"{{ \"title\": \"{url}\", \"topics\": [");
                sbWasNull = true;
            }

            try
            {
                var html = await WebClientEx.GetStringAsync(url);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var navigationItems = htmlDoc.QuerySelectorAll("tr.js-navigation-item");
                foreach (var navigationItem in navigationItems)
                {
                    // First, we look for all folders
                    var icon = navigationItem.QuerySelector("td.icon");
                    if (icon != null)
                    {
                        var svg = icon.QuerySelector("svg.octicon-file-directory");
                        if (svg != null)
                        {
                            // This is a folder
                            var content = navigationItem.QuerySelector("td.content span a");
                            if (content != null)
                            {
                                var slug = GetSlugFromLink(url, rootUrl, content.InnerText);
                                sb.Append("{");
                                sb.Append($"\"title\": \"{content.InnerText}\", \"slug\": \"{slug}\"");
                                sb.Append(", \"topics\": [");

                                var linkUrl = "https://github.com" + content.GetAttributeValue("href", string.Empty);
                                await CrawlGitHubForToc(linkUrl, sb, rootUrl);
                                sb.Append("]},");
                            }
                        }
                    }

                    // Now, we look for all the local documents
                    var icon2 = navigationItem.QuerySelector("td.icon");
                    if (icon2 != null)
                    {
                        var svg = icon2.QuerySelector("svg.octicon-file");
                        if (svg != null)
                        {
                            // This is a file
                            var content = navigationItem.QuerySelector("td.content span a");
                            if (content != null)
                            {
                                var text = content.InnerText;
                                if (ShouldFileBeIncludedInCrawl(text))
                                {
                                    var title = JustFileName(text);
                                    var slug = GetSlugFromLink(url, rootUrl, title);
                                    var link = content.GetAttributeValue("href", string.Empty);
                                    var blobMasterIndex = link.IndexOf("/blob/master/", StringComparison.InvariantCultureIgnoreCase);
                                    if (blobMasterIndex > -1)
                                        link = link.Substring(blobMasterIndex + 13);
                                    sb.Append("{");
                                    sb.Append($"\"title\": \"{title}\", \"link\": \"{link}\", \"slug\": \"{slug}\"");
                                    sb.Append("},");
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Oh well! Nothing we can do
            }

            if (sbWasNull)
            {
                sb.Append("]}");
                var resultToc = sb.ToString();
                lock (CrawledGitHubRepositories)
                    if (CrawledGitHubRepositories.ContainsKey(url))
                        CrawledGitHubRepositories[url] = resultToc;
                    else
                        CrawledGitHubRepositories.Add(url, resultToc);
                return resultToc;
            }

            return string.Empty;
        }

        private static string GetSlugFromLink(string url, string rootUrl, string title)
        {
            var slug = url.Substring(rootUrl.Length) + "/" + GetNormalizedName(title);
            if (slug.ToLowerInvariant().StartsWith("tree/")) slug = slug.Substring(5);
            if (slug.ToLowerInvariant().StartsWith("master/")) slug = slug.Substring(7);
            slug = TrimSupportedCrawlExtensionFromSlug(slug);
            while (slug.Contains("//")) slug = slug.Replace("//", "/");
            return slug;
        }

        private static readonly string[] SupportedCrawlExtensions = { "md", "html", "htm", "txt", "jpeg", "jpg", "png", "gif", "tiff", "tif" };

        private static bool ShouldFileBeIncludedInCrawl(string fileName)
        {
            fileName = fileName.ToLowerInvariant();
            return SupportedCrawlExtensions.Any(supportedExtension => fileName.EndsWith("." + supportedExtension));
        }

        private static string TrimSupportedCrawlExtensionFromSlug(string slug)
        {
            var slugLower = slug.ToLowerInvariant();
            foreach (var supportedExtension in SupportedCrawlExtensions)
                if (slugLower.EndsWith("." + supportedExtension))
                    slugLower = slugLower.Substring(0, slug.Length - (supportedExtension.Length + 1));
            return slug;
        }
    }
}
