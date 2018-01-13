using System;
using System.IO;
using Markdig;
using Markdig.Renderers;

namespace DocHound.Classes.TopicRenderers
{
    public class MarkdownTopicRenderer : ITopicRenderer
    {
        public string RenderToHtml(TopicRaw topic, string imageRootUrl = "")
        {
            if (string.IsNullOrEmpty(topic.OriginalContent)) return string.Empty;
            return MarkdownToHtml(topic, imageRootUrl);
        }

        private string MarkdownToHtml(TopicRaw topic, string imageRootUrl)
        {
            // TODO: This uses all images as external links. We may need to handle that differently
            var markdown = topic.OriginalContent;
            markdown = markdown.Replace("![](", "![](" + imageRootUrl);
            markdown = markdown.Replace("background: url('", "background: url('" + imageRootUrl);
            markdown = markdown.Replace("src=\"", "src=\"" + imageRootUrl);

            var builder = new MarkdownPipelineBuilder();
            BuildPipeline(builder);
            var pipeline = builder.Build();
            return Markdown.ToHtml(markdown, pipeline);
        }

        protected virtual MarkdownPipelineBuilder BuildPipeline(MarkdownPipelineBuilder builder)
        {
            builder = builder.UseYamlFrontMatter();
            builder = builder.UseAutoLinks();
            builder = builder.UseAutoIdentifiers();
            builder = builder.UseAbbreviations();
            builder = builder.UseEmojiAndSmiley();
            builder = builder.UseMediaLinks();
            builder = builder.UseListExtras();
            //builder = builder.UseFigures();
            builder = builder.UseTaskLists();
            //builder = builder.UseSmartyPants();
            //builder = builder.UsePragmaLines();
            builder = builder.UseGridTables();
            builder = builder.UsePipeTables();
            builder = builder.UseEmphasisExtras();
            builder = builder.UseFootnotes();
            builder = builder.UseCitations();
            builder = builder.UseGenericAttributes();

            return builder;
        }

        /// <summary>
        /// Create the entire Markdig pipeline and return the completed
        /// ready to process builder.
        /// </summary>
        /// <returns></returns>
        protected virtual MarkdownPipelineBuilder CreatePipelineBuilder()
        {
            var builder = new MarkdownPipelineBuilder();

            try
            {
                builder = BuildPipeline(builder);
            }
            catch (ArgumentException)
            {
            }

            return builder;
        }

        protected virtual IMarkdownRenderer CreateRenderer(TextWriter writer)
        {
            return new HtmlRenderer(writer);
        }
    }
}