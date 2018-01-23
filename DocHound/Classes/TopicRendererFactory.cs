using System.Collections.Generic;
using DocHound.Interfaces;
using DocHound.TopicRenderers.Html;
using DocHound.TopicRenderers.Markdown;
using DocHound.TopicRenderers.VisualStudioTeamSystem;

namespace DocHound.Classes
{
    public static class TopicRendererFactory
    {
        public static ITopicRenderer GetTopicRenderer(TopicInformation topic)
        {
            // If we recognize the topic type, we use it to determine the renderer
            var renderer = GetRegisteredTopicRenderer(topic.Type);
            if (renderer != null) return renderer;

            // Since we couldn't use type information, we try to inspect the name (URL pattern) to see if we can do anything with it
            var normalizedName = topic.OriginalNameNormalized;
            if (normalizedName.EndsWith(".html") || normalizedName.EndsWith(".htm")) return GetRegisteredTopicRenderer("html");

            // For anything else, we assume markdown
            return GetRegisteredTopicRenderer("markdown"); 
        }

        private static readonly Dictionary<string, ITopicRenderer> RegisteredRenderers = new Dictionary<string, ITopicRenderer>
        {
            {TopicTypeNames.Markdown, new MarkdownTopicRenderer()},
            {TopicTypeNames.Html, new HtmlTopicRenderer()},
            {TopicTypeNames.VstsWorkItem, new WorkItemTopicRenderer()},
            {TopicTypeNames.VstsWorkItemQueries, new WorkItemTopicRenderer()},
            {TopicTypeNames.VstsWorkItemQuery, new WorkItemTopicRenderer()},
        };

        public static ITopicRenderer GetRegisteredTopicRenderer(string typeName) 
        {
            typeName = TopicTypeHelper.GetNormalizeName(typeName);
            return RegisteredRenderers.ContainsKey(typeName) ? RegisteredRenderers[typeName] : null;
        }

        public static void RegisterTopicRenderer(string typeName, ITopicRenderer renderer)
        {
            typeName = TopicTypeHelper.GetNormalizeName(typeName);
            if (RegisteredRenderers.ContainsKey(typeName))
                RegisteredRenderers[typeName] = renderer;
            else
                RegisteredRenderers.Add(typeName, renderer);
        }
    }
}