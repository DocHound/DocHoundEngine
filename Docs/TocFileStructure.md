# Table of Contents File Structure

The following is an example of a TOC Json File structure:

```json
{
  "title": "Kava Docs Documentation",
  "owner": "Kava Docs Inc.",
  "language": "en-US",
  "version": "1.0",
  "requireHttps": true,
  "theme": {
    "standardTheme": "Default",
    "customeCss": null,
    "customTemplate": null 
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
        }
      ]
    }
  ]
}
```

## Root Properties

| Property     | Description                              | Values |
|--------------|------------------------------------------|--------|
| title        | Title of the documentation set           | string |
| owner        | Legal owner and copyright holder         | string |
| version      | Version of the docs                      | string |
| requireHttps | Indicates whether the docs must be accessed using HTTPS | true or false |
| title        | Title of the documentation set           | string |
| menu         | Contains an array of menu items (see below) | array of menu items |
| theme        | Contains information about the applied theme (see below)  | theme object |
| topics       | Contains an array of topics (see below)  | array of topics |

## Menu Collection

Contains an array of menu items that are rendered as part of the documentationdocumentations.

| Property     | Description                                     |        |
|--------------|-------------------------------------------------|--------|
| title        | Menu item title                                 | string |
| link         | The URL (or partial URL) the menu item links to | string |

## Theme Object

Contains individual settings that allow for theme customization.

| Property       | Description                                     | Values |
|----------------|-------------------------------------------------|--------|
| standardTheme  | Name of the theme supported by Kava Docs        | Supported values: Default, Dark (more to come) |
| customCss      | URL (relative or absolute) to a CSS file with CSS overrides | Partial URL or full URL |
| customTemplate | *reserved for future use* |  |
| syntaxTheme    | Name of the color theme for syntax highlighting | Supported values: **kavadocs**, **kavadocsdark**, brown-paper, brown-papersq, codepen-embed, color-brewer, darcula, dark, darkula, default, dracula, far, foundation, github, github-gist, cooglecode, grayscale, idea, ir-black, kimbie.dark, kimbie.light, magula, mono-blue, monokai, monokai-sublime, obsidian, paraiso-dark, paraiso-light, railcasts, rainbow, solarized-dark, solarized-light, sunburst, twilight, vs, vs2015, xcode, zenburn |

## Topics Collection

| Property     | Description                              | Values |
|--------------|------------------------------------------|--------|
| title        | Title of the topic                       | string |
| link         | Url (typically relative, but can be full) to the topic file | string |
| ...          | ... | ... |
