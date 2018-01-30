using System.Text;
using DocHound.Interfaces;

namespace DocHound.TopicRenderers.Html
{
    public class ImageTopicRenderer : ITopicRenderer
    {
        public string RenderToHtml(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null)
        {
            var sb = new StringBuilder();
            sb.Append($"<h1>{topic.OriginalName}</h1>");
            sb.Append($"<p><img src=\"{topic.OriginalContent}\" /></p>");
            return sb.ToString();
        }

        public string GetTemplateName(TopicInformation topic, string suggestedTemplateName, ISettingsProvider settings = null) => suggestedTemplateName;
        public string RenderToJson(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null) => string.Empty;
    }
}
