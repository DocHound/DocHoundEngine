﻿using System.Text.RegularExpressions;
using DocHound.Interfaces;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;

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
            var html = Markdig.Markdown.ToHtml(markdown, pipeline);

            if (settings.GetSetting<bool>(SettingsEnum.UseFontAwesomeInMarkdown))
                html = ParseFontAwesomeIcons(html);

            return html;
        }


        public static Regex FontAwesomeIconRegEx = new Regex(@"@icon-.*?[\s|\.|\,|\<]");


        /// <summary>
        /// Post processing routine that post-processes the HTML and 
        /// replaces @icon- with fontawesome icons
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected string ParseFontAwesomeIcons(string html)
        {
            var matches = FontAwesomeIconRegEx.Matches(html);
            foreach (Match match in matches)
            {
                var iconblock = match.Value.Substring(0, match.Value.Length - 1);
                var icon = iconblock.Replace("@icon-", "");
                html = html.Replace(iconblock, "<i class=\"fa fa-" + icon + "\"></i> ");
            }

            return html;
        }


        protected virtual MarkdownPipelineBuilder BuildPipeline(MarkdownPipelineBuilder builder, ISettingsProvider settings, string markdown)
        {
            if (settings.GetSetting<bool>(SettingsEnum.UseAbbreviations)) builder = builder.UseAbbreviations();
            if (settings.GetSetting<bool>(SettingsEnum.UseAutoIdentifiers)) builder = builder.UseAutoIdentifiers(AutoIdentifierOptions.GitHub);
            //if (settings.GetSetting<bool>(SettingsEnum.UseAutoLinks)) builder = builder.UseAutoLinks();
            if (settings.GetSetting<bool>(SettingsEnum.UseCitations)) builder = builder.UseCitations();
            if (settings.GetSetting<bool>(SettingsEnum.UseCustomContainers)) builder = builder.UseCustomContainers();
            if (settings.GetSetting<bool>(SettingsEnum.UseEmojiAndSmiley)) builder = builder.UseEmojiAndSmiley();
            if (settings.GetSetting<bool>(SettingsEnum.UseEmphasisExtras)) builder = builder.UseEmphasisExtras();
            if (settings.GetSetting<bool>(SettingsEnum.UseFigures)) builder = builder.UseFigures();
            if (settings.GetSetting<bool>(SettingsEnum.UseFootnotes)) builder = builder.UseFootnotes();
            if (settings.GetSetting<bool>(SettingsEnum.UseGenericAttributes) && !settings.GetSetting<bool>(SettingsEnum.UseMathematics)) builder = builder.UseGenericAttributes();
            if (settings.GetSetting<bool>(SettingsEnum.UseGridTables)) builder = builder.UseGridTables();
            if (settings.GetSetting<bool>(SettingsEnum.UseListExtras)) builder = builder.UseListExtras();
            
            //if (settings.GetSetting<bool>(SettingsEnum.UseMathematics)) builder = builder.UseMathematics();
            if (settings.GetSetting<bool>(SettingsEnum.UseMathematics)) builder = builder.UseMathJax();                 

            if (settings.GetSetting<bool>(SettingsEnum.UseMediaLinks)) builder = builder.UseMediaLinks();
            if (settings.GetSetting<bool>(SettingsEnum.UsePipeTables)) builder = builder.UsePipeTables();
            if (settings.GetSetting<bool>(SettingsEnum.UsePragmaLines)) builder = builder.UsePragmaLines();
            if (settings.GetSetting<bool>(SettingsEnum.UseSmartyPants)) builder = builder.UseSmartyPants();
            if (settings.GetSetting<bool>(SettingsEnum.UseTaskLists)) builder = builder.UseTaskLists();
            if (settings.GetSetting<bool>(SettingsEnum.UseYamlFrontMatter)) builder = builder.UseYamlFrontMatter();

            var containsMermaid = markdown.Contains("```mermaid");
            var containsNomnoml = markdown.Contains("```nomnoml");
            var useDiagrams = containsMermaid || containsNomnoml;
            if (useDiagrams)
            {
                // We need to check to make sure that it isn't specifically disabled
                if (settings.IsSettingSpecified(SettingsEnum.UseDiagramsNomnoml) && !settings.GetSetting<bool>(SettingsEnum.UseDiagramsNomnoml))
                    useDiagrams = false;
                if (settings.IsSettingSpecified(SettingsEnum.UseDiagramsMermaid) && !settings.GetSetting<bool>(SettingsEnum.UseDiagramsMermaid))
                    useDiagrams = false;
                if (useDiagrams) // If we auto-use, we need to set the diagram settings to true in this instance, so subsequent processing will work correctly
                {
                    if (containsMermaid)
                        settings.OverrideSetting(SettingsEnum.UseDiagramsMermaid, true);
                    if (containsNomnoml)
                        settings.OverrideSetting(SettingsEnum.UseDiagramsNomnoml, true);
                }

            }
            else
                useDiagrams = settings.GetSetting<bool>(SettingsEnum.UseDiagramsMermaid) || settings.GetSetting<bool>(SettingsEnum.UseDiagramsNomnoml);

            if (useDiagrams) builder = builder.UseDiagrams();

            return builder;
        }

        public string GetTemplateName(TopicInformation topic, string suggestedTemplateName, ISettingsProvider settings = null) => suggestedTemplateName;
        public string RenderToJson(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null) => string.Empty;
    }
}