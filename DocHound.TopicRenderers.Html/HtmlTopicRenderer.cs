using DocHound.Interfaces;

namespace DocHound.TopicRenderers.Html
{
    public class HtmlTopicRenderer : ITopicRenderer
    {
        public string RenderToHtml(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null)
        {
            // TODO: Make sure we only get the parts within the HTML <body> element, if this is more than an HTML fragment
            // TODO: Patch up the root URL of images that have a relative path
            return topic.OriginalContent;
        }
    }
}
