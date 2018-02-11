using DocHound.Interfaces;
using Markdig;

namespace DocHound.TopicRenderers.Markdown
{
    public class MarkdownTopicRenderer : ITopicRenderer
    {
        public string RenderToHtml(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null)
        {
            if (string.IsNullOrEmpty(topic.OriginalContent)) return string.Empty;
            return MarkdownToHtml(topic, imageRootUrl, settings);
        }

        private string MarkdownToHtml(TopicInformation topic, string imageRootUrl, ISettingsProvider settings)
        {
            // TODO: This uses all images as external links. We may need to handle that differently

            var markdown = topic.OriginalContent;
            markdown = markdown.Replace("![](", "![](" + imageRootUrl);
            markdown = markdown.Replace("background: url('", "background: url('" + imageRootUrl);
            markdown = markdown.Replace("src=\"", "src=\"" + imageRootUrl);

            var builder = new MarkdownPipelineBuilder();
            BuildPipeline(builder, settings, markdown);
            var pipeline = builder.Build();
            return Markdig.Markdown.ToHtml(markdown, pipeline);
        }

        protected virtual MarkdownPipelineBuilder BuildPipeline(MarkdownPipelineBuilder builder, ISettingsProvider settings, string markdown)
        {
            if (settings.GetSetting<bool>(Settings.UseAbbreviations)) builder = builder.UseAbbreviations();
            if (settings.GetSetting<bool>(Settings.UseAutoIdentifiers)) builder = builder.UseAutoIdentifiers();
            if (settings.GetSetting<bool>(Settings.UseAutoLinks)) builder = builder.UseAutoLinks();
            if (settings.GetSetting<bool>(Settings.UseCitations)) builder = builder.UseCitations();
            if (settings.GetSetting<bool>(Settings.UseCustomContainers)) builder = builder.UseCustomContainers();
            if (settings.GetSetting<bool>(Settings.UseEmojiAndSmiley)) builder = builder.UseEmojiAndSmiley();
            if (settings.GetSetting<bool>(Settings.UseEmphasisExtras)) builder = builder.UseEmphasisExtras();
            if (settings.GetSetting<bool>(Settings.UseFigures)) builder = builder.UseFigures();
            if (settings.GetSetting<bool>(Settings.UseFootnotes)) builder = builder.UseFootnotes();
            if (settings.GetSetting<bool>(Settings.UseGenericAttributes)) builder = builder.UseGenericAttributes();
            if (settings.GetSetting<bool>(Settings.UseGridTables)) builder = builder.UseGridTables();
            if (settings.GetSetting<bool>(Settings.UseListExtras)) builder = builder.UseListExtras();
            if (settings.GetSetting<bool>(Settings.UseMathematics)) builder = builder.UseMathematics();
            if (settings.GetSetting<bool>(Settings.UseMediaLinks)) builder = builder.UseMediaLinks();
            if (settings.GetSetting<bool>(Settings.UsePipeTables)) builder = builder.UsePipeTables();
            if (settings.GetSetting<bool>(Settings.UsePragmaLines)) builder = builder.UsePragmaLines();
            if (settings.GetSetting<bool>(Settings.UseSmartyPants)) builder = builder.UseSmartyPants();
            if (settings.GetSetting<bool>(Settings.UseTaskLists)) builder = builder.UseTaskLists();
            if (settings.GetSetting<bool>(Settings.UseYamlFrontMatter)) builder = builder.UseYamlFrontMatter();

            var containsMermaid = markdown.Contains("```mermaid");
            var containsNomnoml = markdown.Contains("```nomnoml");
            var useDiagrams = containsMermaid || containsNomnoml;
            if (useDiagrams)
            {
                // We need to check to make sure that it isn't specifically disabled
                if (settings.IsSettingSpecified(Settings.UseDiagramsNomnoml) && !settings.GetSetting<bool>(Settings.UseDiagramsNomnoml))
                    useDiagrams = false;
                if (settings.IsSettingSpecified(Settings.UseDiagramsMermaid) && !settings.GetSetting<bool>(Settings.UseDiagramsMermaid))
                    useDiagrams = false;
                if (useDiagrams) // If we auto-use, we need to set the diagram settings to true in this instance, so subsequent processing will work correctly
                {
                    if (containsMermaid)
                        settings.OverrideSetting(Settings.UseDiagramsMermaid, true);
                    if (containsNomnoml)
                        settings.OverrideSetting(Settings.UseDiagramsNomnoml, true);
                }

            }
            else
                useDiagrams = settings.GetSetting<bool>(Settings.UseDiagramsMermaid) || settings.GetSetting<bool>(Settings.UseDiagramsNomnoml);

            if (useDiagrams) builder = builder.UseDiagrams();

            return builder;
        }

        public string GetTemplateName(TopicInformation topic, string suggestedTemplateName, ISettingsProvider settings = null) => suggestedTemplateName;
        public string RenderToJson(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null) => string.Empty;
    }
}