# Table of Contents File Structure

The following is an example of a TOC Json File structure:

```json
{
  "title": "Kava Docs Documentation",
  "owner": "Kava Docs Inc.",
  "language": "en-US",
  "version": "1.0",
  "requireHttps |  | true (default) or false  | No |
  "theme": {
    "standardTheme": "Default",
    "customeCss": null,
    "customTemplate": null 
  },
  "settings": {
| Abbreviations |  | true (default) or false  | No |
| AutoIdentifiers |  | true (default) or false  | No |
| AutoLinks |  | true (default) or false  | No |
| Citations |  | true (default) or false  | No |
| CustomContainers |  | true (default) or false  | No |
| DiagramsMermaid |  | true or false (default) | No |
| DiagramsNomnoml |  | true or false (default) | No |
| EmojiAndSmiley |  | true (default) or false  | No |
| EmphasisExtras |  | true (default) or false  | No |
| Figures |  | true (default) or false  | No |
| Footnotes |  | true (default) or false  | No |
| GenericAttributes |  | true (default) or false  | No |
| GridTables |  | true (default) or false  | No |
| ListExtras |  | true (default) or false  | No |
| Mathematics |  | true or false (default) | No |
| MediaLinks |  | true (default) or false  | No |
| PipeTables |  | true (default) or false  | No |
| PragmaLines |  | true (default) or false  | No |
| SmartyPants |  | true (default) or false  | No |
| SyntaxHighlighting |  | true (default) or false  | No |
| TaskLists |  | true (default) or false  | No |
| YamlFrontMatter": true
  },
  "menu": [
    {
      "title": "Docs Home",
      "link": "/"
    },
    {
      "title": "West-Wind Home",
      "link": "http://www.west-wind.com"
    },
    {
      "title": "CODE Magazine",
      "link": "http://codemag.com/magazine"
    },
    {
      "title": "CODE Consulting",
      "link": "http://codemag.com/consulting"
    },
    {
      "title": "CODE Training",
      "link": "http://codemag.com/training"
    }
  ],
  "topics": [
    {
      "title": "Welcome to Kava Docs",
      "link": "Welcome.md"
    },
    {
      "title": "What's New?",
      "link": "WhatsNew.md",
      "topics": [
        {
          "title": "Change Log",
          "link": "ChangeLog.md"
        },
        {
          "title": "To-Do List",
          "link": "Todo.md"
        }
      ]
    },
    {
      "title": "Reference Guide",
      "topics": [
        {
          "title": "TOC File Structure",
          "link": "TocFileStructure.md"
        },
        {
          "title": "CSS Customization",
          "link": "CssStructure.md"
        },
        {
          "title": "Supported Markdown Features",
          "link": "SupportedMarkdownFeatures.md",
          "topics": [
              {
                "title": "Mermaid Charts",
                "link": "Mermaid.md",
                "settings": {
              | DiagramsMermaid": true
                }
              }, 
              {
                "title": "Nomnoml Charts",
                "link": "Nomnoml.md",
                "settings": {
              | DiagramsNomnoml": true
                }
              },
              {
                "title": "Mathematics (LaTeX)",
                "link": "Mathematics.md",
                "settings": {
              | Mathematics": true
                }
              }
          ]
        }
      ]
    }
  ]
}

```

## Root Properties

| Property     | Description                              | Values | Required |
|--------------|------------------------------------------|--------|----------|
| title        | Title of the documentation set           | string | Yes |
| owner        | Legal owner and copyright holder         | string | No |
| version      | Version of the docs                      | string | No |
| requireHttps | Indicates whether the docs must be accessed using HTTPS | true or false | No |
| title        | Title of the documentation set           | string | Yes |
| menu         | Contains an array of menu items (see below) | array of menu items | No |
| theme        | Contains information about the applied theme (see below)  | theme object | No |
| settings     | Settings for the entire tree (creates defaults for each topic, although each individual topic node can choose to override a default setting)  | settings object | No |
| topics       | Contains an array of topics (see below)  | array of topics | Yes |

## Menu Collection

Contains an array of menu items that are rendered as part of the documentationdocumentations.

| Property     | Description                                     | Values | Required |
|--------------|-------------------------------------------------|--------|----------|
| title        | Menu item title                                 | string | Yes |
| link         | The URL (or partial URL) the menu item links to | string | Yes |

## Theme Object

Contains individual settings that allow for theme customization.

| Property       | Description                                     | Values | Required |
|----------------|-------------------------------------------------|--------|----------|
| standardTheme  | Name of the theme supported by Kava Docs        | Supported values: Default, Dark (more to come) | No |
| customCss      | URL (relative or absolute) to a CSS file with CSS overrides | Partial URL or full URL | No |
| customTemplate | *reserved for future use* |  | No |
| syntaxTheme    | Name of the color theme for syntax highlighting | Supported values: **kavadocs**, **kavadocsdark**, brown-paper, brown-papersq, codepen-embed, color-brewer, darcula, dark, darkula, default, dracula, far, foundation, github, github-gist, cooglecode, grayscale, idea, ir-black, kimbie.dark, kimbie.light, magula, mono-blue, monokai, monokai-sublime, obsidian, paraiso-dark, paraiso-light, railcasts, rainbow, solarized-dark, solarized-light, sunburst, twilight, vs, vs2015, xcode, zenburn | No |

## Settings Object

The settings object can be set on the root or an individual topic. Settings on a topic override the default settings in an additive fashion. Values that are set on the root but not on the topics are still respected for a topic, even if that topic overrides other settings.

| Property       | Description                                     | Values | Required |
|----------------|-------------------------------------------------|--------|----------|
| Abbreviations | Should abbreviations be used? | true (default) or false  | No |
| AutoIdentifiers | Should headings automatically be generated with id attributes? | true (default) or false  | No |
| AutoLinks | Should URLs be automatically turned into links? | true (default) or false  | No |
| Citations | Support the citations feature? | true (default) or false  | No |
| CustomContainers | Support custom container syntax? | true (default) or false  | No |
| DiagramsMermaid | Support mermaid diagrams? | true or false (default) | No |
| DiagramsNomnoml | Support nomnoml diagrams? | true or false (default) | No |
| EmojiAndSmiley | Support automatic emojis and smileys? | true (default) or false  | No |
| EmphasisExtras | Support extra emphasis? | true (default) or false  | No |
| Figures | Support figures? | true (default) or false  | No |
| Footnotes | Support footnotes? | true (default) or false  | No |
| GenericAttributes | Support generic attributes? | true (default) or false  | No |
| GridTables | Support grid tables? | true (default) or false  | No |
| ListExtras | Support list extras? | true (default) or false  | No |
| Mathematics | Support LaTeX mathematics notation? | true or false (default) | No |
| MediaLinks | Support media links? | true (default) or false  | No |
| PipeTables | Support pipe tables? | true (default) or false  | No |
| PragmaLines | Support pragma lines? | true (default) or false  | No |
| SmartyPants | Support SmartyPants? | true (default) or false  | No |
| SyntaxHighlighting | Support source code syntax highlighting? | true (default) or false  | No |
| TaskLists | Support task lists? | true (default) or false  | No |
| YamlFrontMatter | Support YAML FrontMatter? | true (default) or false  | No |

## Topics Collection

| Property     | Description                              | Values | Required |
|--------------|------------------------------------------|--------|----------|
| title        | Title of the topic                       | string | Yes |
| link         | Url (typically relative, but can be full) to the topic file | string | No |
| slug         | Url slug that is used by default for this topic. *(Note: When URLs are manually entered, Kava Docs can find other patterns also, but this slug is used as the default slug that is used when clicking on a node in the tree).* | string | No |
| settings          | Settings specific to this topic (additive and overriding the root settings) | settings object | No |
| ...          | ... | ... | |
