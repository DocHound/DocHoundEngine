using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocHound.Classes;
using HtmlAgilityPack;
using Markdig;
using Markdig.Renderers;
using Microsoft.Extensions.Configuration;

namespace DocHound.Models.Docs
{
    public class ReindexViewModel
    {
        public async Task LoadData()
        {
            await BuildToc();
        }

        private async Task BuildToc()
        {
            string tocJson = null;

            switch (TopicViewModel.RepositoryType)
            {
                case RepositoryTypes.GitHubRaw:
                    tocJson = await TableOfContentsHelper.GetTocJsonFromGitHubRaw(TopicViewModel.MasterUrlRaw);
                    break;
                case RepositoryTypes.VisualStudioTeamSystemGit:
                    tocJson = await VstsHelper.GetTocJson(TopicViewModel.VstsInstance, TopicViewModel.VstsProjectName, TopicViewModel.VstsDocsFolder, TopicViewModel.VstsPat);
                    break;
            }
            if (string.IsNullOrEmpty(tocJson)) return;

            var dynamicToc = TableOfContentsHelper.GetDynamicTocFromJson(tocJson);

            Topics = TableOfContentsHelper.BuildTocFromDynamicToc(dynamicToc, null, string.Empty);
        }

        public List<TableOfContentsItem> Topics { get; set; } = new List<TableOfContentsItem>();

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
                    switch (TopicViewModel.RepositoryType)
                    {
                        case RepositoryTypes.GitHubRaw:
                            content = await WebClientEx.GetStringAsync(GetFullExternalLink(nextTopic.Title));
                            break;
                        case RepositoryTypes.VisualStudioTeamSystemGit:
                            if (!string.IsNullOrEmpty(nextTopic.LinkPure))
                                content = await VstsHelper.GetFileContents(nextTopic.LinkPure, TopicViewModel.VstsInstance, TopicViewModel.VstsProjectName, TopicViewModel.VstsDocsFolder, TopicViewModel.VstsPat);
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
                    realLink = TopicViewModel.MasterUrlRaw + realLink;
                return realLink;
            }
            return TopicViewModel.MasterUrlRaw + link.Replace(" ", "%20");
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