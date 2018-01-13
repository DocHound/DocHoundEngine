using System;
using System.IO;
using Markdig;
using Markdig.Renderers;

namespace DocHound.Classes.TopicRenderers
{
    public class HtmlTopicRenderer : ITopicRenderer
    {
        public string RenderToHtml(TopicRaw topic)
        {
            // TODO: Make sure we only get the parts within the HTML <body> element, if this is more than an HTML fragment
            return topic.OriginalContent;
        }
    }
}