using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocHound.Classes;
using DocHound.ContentLoaders;
using DocHound.Interfaces;
using HtmlAgilityPack;
using Markdig;
using Markdig.Renderers;
using Microsoft.Extensions.Configuration;

namespace DocHound.Models.Docs
{
    // TODO: This whole class was just started but never completed
    public class ReindexViewModel
    {
        public async Task LoadData()
        {
            await BuildToc();
        }

        private async Task BuildToc()
        {
            string tocJson = null;

            switch (RepositoryType)
            {
                case RepositoryTypes.GitHubRaw:
                    // TODO: tocJson = await TableOfContentsHelper.GetTocJsonFromGitHubRaw(GitHubMasterUrlRaw);
                    break;
                case RepositoryTypes.VstsGit:
                    tocJson = await VstsHelper.GetTocJson(VstsInstance, VstsProjectName, VstsDocsFolder, VstsPat, VstsApiVersion);
                    break;
            }
            if (string.IsNullOrEmpty(tocJson)) return;

            var dynamicToc = TableOfContentsHelper.GetDynamicTocFromJson(tocJson);

            Topics = TableOfContentsHelper.BuildTocFromDynamicToc(dynamicToc, null, string.Empty, out List<TableOfContentsItem> flatTopicsList);
        }

        public List<TableOfContentsItem> Topics { get; set; } = new List<TableOfContentsItem>();

        private RepositoryTypes _repositoryTypes = RepositoryTypes.Undefined;
        public RepositoryTypes RepositoryType 
        {
            get
            {
                if (_repositoryTypes == RepositoryTypes.Undefined)
                    _repositoryTypes = RepositoryTypeHelper.GetTypeFromTypeName(SettingsHelper.GetSetting<string>(Settings.RepositoryType));
                return _repositoryTypes;
            }
        }

        private string _gitHubProject = null;
        public string GitHubProject
        {
            get
            {
                if (_gitHubProject == null)
                    _gitHubProject = SettingsHelper.GetSetting<string>(Settings.GitHubProject);
                return _gitHubProject;
            }
        }

        // GitHub Raw Settings
        private string _gitHubMasterUrlRaw = null;
        public string GitHubMasterUrlRaw 
        { 
            get
            {
                if (RepositoryType != RepositoryTypes.GitHubRaw) return string.Empty;

                if (_gitHubMasterUrlRaw == null)
                {
                    if (string.IsNullOrEmpty(GitHubProject))
                    {
                        var gitHubMasterUrlRaw = GitHubMasterUrl.Replace("https://github.com", "https://raw.githubusercontent.com/");
                        if (!GitHubMasterUrl.Contains("/master/")) gitHubMasterUrlRaw += "/master/";
                        _gitHubMasterUrlRaw = gitHubMasterUrlRaw;
                    }
                    else
                        _gitHubMasterUrlRaw = "https://raw.githubusercontent.com/" + GitHubProject + "/master/";
                }

                return _gitHubMasterUrlRaw;
            } 
        }
    
        private string _gitHubMasterUrl = null;
        public string GitHubMasterUrl
        {
            get
            {
                if (RepositoryType != RepositoryTypes.GitHubRaw) return string.Empty;

                if (_gitHubMasterUrl == null)
                {
                    if (string.IsNullOrEmpty(GitHubProject))
                        _gitHubMasterUrl = SettingsHelper.GetSetting<string>(Settings.GitHubMasterUrl);
                    else
                        _gitHubMasterUrl = "https://github.com/" + GitHubProject;
                }

                return _gitHubMasterUrl;
            }
        }

        // VSTS Settings
        public string _vstsPat = null;
        public string VstsPat 
        {
            get
            {
                if (_vstsPat == null)
                    _vstsPat = SettingsHelper.GetSetting<string>(Settings.VstsPat);
                return _vstsPat;
            }
        }

        public string _vstsInstance;
        public string VstsInstance
        {
            get
            {
                if (string.IsNullOrEmpty(_vstsInstance))
                    _vstsInstance = SettingsHelper.GetSetting<string>(Settings.VstsInstance);
                return _vstsInstance;
            }
        }

        public string _vstsDocsFolder = null;
        public string VstsDocsFolder
        {
            get
            {
                if (_vstsDocsFolder == null)
                    _vstsDocsFolder = SettingsHelper.GetSetting<string>(Settings.VstsDocsFolder);
                return _vstsDocsFolder;
            }
        }

        public string _vstsDocsProjectName = null;
        public string VstsProjectName
        {
            get
            {
                if (_vstsDocsProjectName == null)
                    _vstsDocsProjectName = SettingsHelper.GetSetting<string>(Settings.VstsProjectName);
                return _vstsDocsProjectName;
            }
        }

        public string _vstsApiVersion = null;
        public string VstsApiVersion
        {
            get
            {
                if (_vstsApiVersion == null)
                    _vstsApiVersion = SettingsHelper.GetSetting<string>(Settings.VstsApiVersion);
                return _vstsApiVersion;
            }
        }

        public async Task Reindex()
        {
            if (Topics == null || Topics.Count < 1) return;

            var nextTopic = Topics.FirstOrDefault();
            while (nextTopic != null)
            {
                var normalizedLink = nextTopic.LinkPure.ToLowerInvariant();
                var content = string.Empty;
                if (normalizedLink.StartsWith("https://") || normalizedLink.StartsWith("http://"))
                {
                    // This is an absolute link, so we can just try to load it
                    content = await WebClientEx.GetStringAsync(nextTopic.Link);
                }
                else
                {
                    switch (RepositoryType)
                    {
                        case RepositoryTypes.GitHubRaw:
                            content = await WebClientEx.GetStringAsync(GetFullExternalLink(nextTopic.Title));
                            break;
                        case RepositoryTypes.VstsGit:
                            if (!string.IsNullOrEmpty(nextTopic.LinkPure))
                                content = await VstsHelper.GetFileContents(nextTopic.LinkPure, VstsInstance, VstsProjectName, VstsDocsFolder, VstsPat, VstsApiVersion);
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(content))
                    await AddToIndex(nextTopic, content, nextTopic.LinkPure.ToLowerInvariant());
            }
            nextTopic = nextTopic.NextTopic;
        }

        public async Task AddToIndex(TableOfContentsItem topic, string content, string id)
        {

        }

        private string GetFullExternalLink(string link)
        {
            var realLink = GetRealLink(Topics, link);
            if (!string.IsNullOrEmpty(realLink))
            {
                var realLinkLower = realLink.ToLowerInvariant();
                if (!realLinkLower.StartsWith("http://") || !realLinkLower.StartsWith("https://"))
                    realLink = GitHubMasterUrlRaw + realLink;
                return realLink;
            }
            return GitHubMasterUrlRaw + link.Replace(" ", "%20");
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
    }
}