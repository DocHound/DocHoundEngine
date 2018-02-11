using DocHound.Interfaces;

namespace DocHound.TopicRenderers.Html
{
    public class HtmlTopicRenderer : ITopicRenderer
    {
        public string RenderToHtml(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null)
        {
            // TODO: Patch up the root URL of images that have a relative path
            var html = topic.OriginalContent;
            if (string.IsNullOrEmpty(html)) return string.Empty;

            return topic.OriginalContent;
        }

        public string GetTemplateName(TopicInformation topic, string suggestedTemplateName, ISettingsProvider settings = null) => suggestedTemplateName;
        public string RenderToJson(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null) => string.Empty;
    }
}
