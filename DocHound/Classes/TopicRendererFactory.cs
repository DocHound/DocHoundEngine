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
            if (!string.IsNullOrEmpty(topic.Type))
            {
                var normalizedType = topic.Type.ToLowerInvariant();
                if (normalizedType == "markdown") return GetRegisteredTopicRenderer("markdown");
                if (normalizedType == "html") return GetRegisteredTopicRenderer("html");
                if (normalizedType == "vsts-workitem") return GetRegisteredTopicRenderer("vsts-workitem");
            }

            // Since we couldn't use type information, we try to inspect the name (URL pattern) to see if we can do anything with it
            var normalizedName = topic.OriginalNameNormalized;

            if (normalizedName.EndsWith(".html") || normalizedName.EndsWith(".htm")) return GetRegisteredTopicRenderer("html");

            return GetRegisteredTopicRenderer("markdown"); // For anything else, we assume markdown
        }

        private static readonly Dictionary<string, ITopicRenderer> RegisteredRenderers = new Dictionary<string, ITopicRenderer>
        {
            {"markdown", new MarkdownTopicRenderer()},
            {"html", new HtmlTopicRenderer()},
            {"vsts-workitem", new WorkItemTopicRenderer()},
        };

        public static ITopicRenderer GetRegisteredTopicRenderer(string type) => RegisteredRenderers.ContainsKey(type) ? RegisteredRenderers[type] : null;

        public static void RegisterTopicRenderer(string type, ITopicRenderer renderer)
        {
            if (RegisteredRenderers.ContainsKey(type))
                RegisteredRenderers[type] = renderer;
            else
                RegisteredRenderers.Add(type, renderer);
        }
    }
}