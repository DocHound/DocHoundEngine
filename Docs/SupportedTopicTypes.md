# Supported Topic Types

Kava Docs supports a range of vastly different content formats that are to be rendered as topics. This list is also constantly expanded upon.

> Note: We also offer our consulting services in case you need a new content type implemented. Please contact us for further information.

Typic types can be defined on a per-topic basis (see also: [Table of Contents File Structure](TOC-File-Structure)). Note that the Kava Docs engine will also try to automatically determine the topic type based on the link pattern. For instance, links that end in ".md" are assumed to be Markdown files. ".html" are HTML files, and so on.

## Current List of Officially Supported Content Types

| Type | Description | Auto Link Pattern | More Information |
|---|---|---|---|
| markdown | Topics written in the popular Markdown standard. We also support a variety of extensions. | Links ending in .md and also, Markdown is the fallback option when no other type can be identified by the pattern. | [Supported Markdown Features](Supported-Markdown-Features) |
| html | Topics written in standard HTML format. | Links ending in .html or .htm | [Supported HTML Features](Supported-HTML-Features) |
| vsts-workitem | A single TFS/VSTS Work Item | n/a | [Supported VSTS Features](Supported-VSTS-Features) |

## Additional Content Types Under Development or Consideration

The following topic types are currently being considered. Those with a checkmark are under active development.

* [x] TFS Work Items (and/or queries)
* [ ] Plain Text (essentially treated like Markup, I would think)
* [ ] Wiki formats (not sure what that would be exactly :-) )
* [ ] Word
* [ ] One Note
* [ ] PDF
* [ ] Power Point 
* [ ] Excel
* [ ] EML
* [ ] RTF
* [ ] .msg (email messages)
* [ ] CODE Magazine Articles (individual articles and categories or search terms)
* [ ] JIRA Items
* [ ] Some kind of Slack integration (including harvesting Slack history)
* [ ] Screencast
* [ ] Pluralsight
* [ ] Swagger/SwaggerHub
* [ ] Reference source code (so actual source code repositories displayed like Microsoft reference source)
    * [ ] GitHub
    * [ ] TfsGit
    * [ ] General Git
    * [ ] Other source control systems such as Mercurial
* [ ] Evernote
* [ ] Help Monster/Builder
* [ ] Moqups
* [ ] Wikipedia
* [ ] LaTeX
* [ ] NuGet (so people can document things one needs to get from NuGet and how to get it)
