using Markdig;

namespace DocHound.Classes.TopicRenderers
{
    public class MarkdownTopicRenderer : ITopicRenderer
    {
        public string RenderToHtml(TopicRaw topic, string imageRootUrl = "", TocSettings settings = null)
        {
            if (settings == null) settings = new TocSettings(null, null);
            if (string.IsNullOrEmpty(topic.OriginalContent)) return string.Empty;
            return MarkdownToHtml(topic, imageRootUrl, settings);
        }

        private string MarkdownToHtml(TopicRaw topic, string imageRootUrl, TocSettings settings)
        {
            // TODO: This uses all images as external links. We may need to handle that differently

            var markdown = topic.OriginalContent;
            markdown = markdown.Replace("![](", "![](" + imageRootUrl);
            markdown = markdown.Replace("background: url('", "background: url('" + imageRootUrl);
            markdown = markdown.Replace("src=\"", "src=\"" + imageRootUrl);

            var builder = new MarkdownPipelineBuilder();
            BuildPipeline(builder, settings);
            var pipeline = builder.Build();
            return Markdown.ToHtml(markdown, pipeline);
        }

        protected virtual MarkdownPipelineBuilder BuildPipeline(MarkdownPipelineBuilder builder, TocSettings settings)
        {
            // TODO: We should be able to drive all of this through settings that are defined either per-document or inherit from further up in the tree

            if (settings.UseAbbreviations) builder = builder.UseAbbreviations();
            if (settings.UseAutoIdentifiers) builder = builder.UseAutoIdentifiers();
            if (settings.UseAutoLinks) builder = builder.UseAutoLinks();
            if (settings.UseCitations) builder = builder.UseCitations();
            if (settings.UseCustomContainers) builder = builder.UseCustomContainers();
            if (settings.UseDiagramsMermaid || settings.UseDiagramsNomnoml) builder = builder.UseDiagrams();
            if (settings.UseEmojiAndSmiley) builder = builder.UseEmojiAndSmiley();
            if (settings.UseEmphasisExtras) builder = builder.UseEmphasisExtras();
            if (settings.UseFigures) builder = builder.UseFigures();
            if (settings.UseFootnotes) builder = builder.UseFootnotes();
            if (settings.UseGenericAttributes) builder = builder.UseGenericAttributes();
            if (settings.UseGridTables) builder = builder.UseGridTables();
            if (settings.UseListExtras) builder = builder.UseListExtras();
            if (settings.UseMathematics) builder = builder.UseMathematics();
            if (settings.UseMediaLinks) builder = builder.UseMediaLinks();
            if (settings.UsePipeTables) builder = builder.UsePipeTables();
            if (settings.UsePragmaLines) builder = builder.UsePragmaLines();
            if (settings.UseSmartyPants) builder = builder.UseSmartyPants();
            if (settings.UseTaskLists) builder = builder.UseTaskLists();
            if (settings.UseYamlFrontMatter) builder = builder.UseYamlFrontMatter();

            return builder;
        }
    }
}