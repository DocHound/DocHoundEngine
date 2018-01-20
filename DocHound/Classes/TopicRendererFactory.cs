using DocHound.Classes.TopicRenderers;
using DocHound.Models.Docs;

namespace DocHound.Classes
{
    public static class TopicRendererFactory
    {
        public static ITopicRenderer GetTopicRenderer(TopicRaw topic)
        {
            var normalizedName = topic.OriginalNameNormalized;

            if (normalizedName.EndsWith(".html") || normalizedName.EndsWith(".htm")) return HtmlTopicRenderer;

            return MarkdownTopicRenderer;
        }

        private static ITopicRenderer _markdownTopicRenderer;
        public static ITopicRenderer MarkdownTopicRenderer
        {
            get 
            {
                if (_markdownTopicRenderer == null)
                    _markdownTopicRenderer = new MarkdownTopicRenderer();
                return _markdownTopicRenderer;
            }
        }

        private static ITopicRenderer _htmlTopicRenderer;
        public static ITopicRenderer HtmlTopicRenderer
        {
            get 
            {
                if (_htmlTopicRenderer == null)
                    _htmlTopicRenderer = new HtmlTopicRenderer();
                return _htmlTopicRenderer;
            }
        }
    }

    public interface ITopicRenderer
    {
        string RenderToHtml(TopicRaw topic, string imageRootUrl = "", TocSettings settings = null);
    }

    public class TopicRaw
    {
        public string OriginalName { get; set; }
        public string OriginalNameNormalized => OriginalName.Trim().ToLowerInvariant();
        //public string MasterUrl => string.IsNullOrEmpty(TopicViewModel.MasterUrlRaw) ? TopicViewModel.MasterUrl : TopicViewModel.MasterUrlRaw;

        //private string _imageRootUrl;
        //public string ImageRootUrl
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_imageRootUrl))
        //            return MasterUrl;
        //        return _imageRootUrl;
        //    }
        //    set { _imageRootUrl = value; }
        //}

        public string OriginalContent { get; set; }
    }
}