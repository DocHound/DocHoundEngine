
## Slugs and Links
Each topic contains a `Slug` and `Link` entry. Either or both can be set but at least one of them has to be set. 

### Link
`Link` determines the site relative or external link to render the topic. For Web or repository or local links, the path referenced here is a project relative path.

>  @icon-question-circle Should path be extensionless? Or contain extension/link to exact file?

### Slug
The slug is the filename part of the link to the file and is used to generate the name of the file on disk that generally holds the markdown or html content. 

Naming Convention uses **kebab case** (ie. - for spaces) 

> @icon-question-circle Should we use CamelCase instead of Kebab Case?


## Paths in Links
Files can be stored in subfolders below the root in a hierarchy. So:

```
subfolder/readme.md
```

is a valid link as is:

```
subfolder/readme
```

If the extension is not specified  `.md` is assumed.

