﻿using System.ComponentModel;

namespace DocHound.Interfaces
{
    /// <summary>All available settings supported by the system either as global configuration or as settings on a TOC or an invidual Topic</summary>
    /// <remarks>Settings in JSON files are usually set in camelCase, but our system can handle settings in a non-case-sensitive fashion.</remarks>
    public enum Settings
    {
        Unknown,
        RepositoryType,
        SqlConnectionString,

        // GitHub settings
        GitHubProject,
        GitHubMasterUrl,

        // VSTS settings
        VstsInstance,
        VstsPat,
        VstsDocsFolder,
        VstsProjectName,
        [DefaultValue("4.1-preview")]VstsApiVersion,

        // General topic toggle setting 
        [DefaultValue(false)] RequireAuthentication,
        [DefaultValue(true)] UseSyntaxHighlighting,
        [DefaultValue(false)] AllowThemeSwitching,
        [DefaultValue("")] AllowableThemeColors,
        [DefaultValue(true)] AllowThemeColorSwitching,
        [DefaultValue(true)] AllowSyntaxHighlightingThemeSwitching,
        [DefaultValue("")] AllowableSyntaxHighlightingThemes,
        [DefaultValue(true)] RequireHttps,

        // Content and theme settings
        [DefaultValue("~/Images/SiteIcon.png")] SiteIcon,
        [DefaultValue("_meta/_logo.png")] LogoPath,
        [DefaultValue("<p>Documentation engine provided by <a href=\"https://kavadocs.com\">Kava Docs</a>.<br /><i class=\"fa fa-heart\"></i> Made with Aloha in Maui, Hawaii.</p>")] FooterHtml,
        [DefaultValue("Default")] Theme,
        [DefaultValue("Default")] ThemeColors,
        [DefaultValue("")] CustomCssPath,
        [DefaultValue("KavaDocs")] SyntaxTheme,
        [DefaultValue(true)] ShowEstimatedReadingTime,

        // Markdown related settings
        [DefaultValue(true)] UseAbbreviations,
        [DefaultValue(true)] UseAutoIdentifiers,
        [DefaultValue(true)] UseAutoLinks,
        [DefaultValue(true)] UseCitations,
        [DefaultValue(true)] UseCustomContainers,
        [DefaultValue(false)] UseDiagramsMermaid,
        [DefaultValue(false)] UseDiagramsNomnoml,
        [DefaultValue(true)] UseEmojiAndSmiley,
        [DefaultValue(true)] UseEmphasisExtras,
        [DefaultValue(true)] UseFigures,
        [DefaultValue(true)] UseFootnotes,
        [DefaultValue(true)] UseGenericAttributes,
        [DefaultValue(true)] UseGridTables,
        [DefaultValue(true)] UseListExtras,
        [DefaultValue(false)] UseMathematics,
        [DefaultValue(true)] UseMediaLinks,
        [DefaultValue(true)] UsePipeTables,
        [DefaultValue(true)] UsePragmaLines,
        [DefaultValue(true)] UseSmartyPants,
        [DefaultValue(true)] UseTaskLists,
        [DefaultValue(true)] UseYamlFrontMatter,
        [DefaultValue(true)] UseFontAwesomeInMarkdown
    }
}
