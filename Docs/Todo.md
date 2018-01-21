# Kava Docs - To-Do List

## Must-Haves for Minimum Viable Product

* [ ] Landing Page Web Site (www.kavadocs.com) 
* [ ] Accounts and Log-Ins
* [ ] A few more supported sources
    * [ ] TFS Work Items
    * [ ] DOCX
    * [ ] OneNote
    * [ ] ...?
* [ ] Themes
* [ ] Full index search
* [ ] Wildcard domain support
* [ ] Create fundamental documentation for how to use and customize Kava Docs

## Rendering Engine

* [ ] Themes
    * [ ] Dark theme
    * [ ] Sepia theme
* [ ] Allow for different templates (or CSS styles) for different topics or topic trees. For instance, I would like to show the CODE Ohana booklet topics with a different page template (sepia Hawaiian theme) from other topics
* [ ] Support copy & Paste of code snippets (have an icon in the header that people can click on to copy)
    * [ ] Maybe we could even show the name of the language at the top of the snippet
* [ ] Markdown improvements
    * [ ] Would be nice to have options for all markdown extensions (like SmartyPants and all that... see also: [Supported Markdown Features](Supported Markdown Features))
    * [ ] Support FontAwesome
    * [x] Take a look at all the standard (optional) Markdig features to see what should be supported in ours (probably a lot)
    * [x] Would be nice to support math notation
    * [x] Would be nice to support flow charts and other charts (such as UML) - (mermaid and nomnoml)
* [ ] Markdown
    * [ ] Markdown needs to do a better job of fixin up image links
* [ ] Html
    * [ ] Only use HTML body
    * [ ] Should we eliminate scripts from HTML topics?
* [ ] Styling
    * [x] Checkboxes (such as in this list) currently look pretty bad. (GitHub has a better style for these)
    * [x] H3 needs to get their own style
    * [x] Need good default styles for tables
    * [x] Need good default style for blockquotes
    * [x] Should have better styles for the links to sub-topics created automatically when a parent topic is empty.
* [x] Showing the outline with indent levels
* [x] Make the current tab work on mobile (responsive)
* [x] Handle image paths relative to documents stored in root folders properly

## Overall Processing

* [x] URL parameters identifying topics should *not* be case sensitive
* [x] Allow for settings on the entire TOC as well as on individual topics
* [ ] The MasterUrl settting used by GitHubRaw, should probably be renamed to GitHubMasterUrl
* [ ] The name of the logo file shouldn't be hardcoded, but a setting in the TOC file instead. (We can still use the current approach as a fall-back)
* [ ] Should support slug better
* [ ] Topics in the TOC should have a "visible" or "hidden" flag, so they can be managed in the tree (and be considered for URL patterns and such), but not displayed as part of the tree. This is useful in a number of scenarios. For instance, one could create a custom slug the topic can be found by, even though we may not want it in the tree (such as when embedding a document inside of another). It is also useful for tooling, such as when we want a tool that shows all files that are not yet referenced by the TOC (which could get annoying if you have files you really do not want to show up in the tree).
* [ ] Would be nice to allow links to external markdown files (or really any other topic we support) to be brought inline on-demand (so it wouldn't always be there, but it could be 'expanded' in... a good example of this would be in our own [Supported Markdown Features topic](Supported Markdown Features), where I would like to pull in the externally linked explanations of markdown features)
* [ ] Handlebars templates would be a nice feature to support at least for simple templating. (Probably on a topic-by-topic basis... for instance, we could have a template just for specific types of TFS items)
* [ ] When there is a slug passed in that doesn't seem to go with a specific topic, we should show an appropriate message, rather than defaulting back to the first topic
* [ ] Allow overriding of CSS styles on individual topics
* [ ] The fav-icon needs to be configurable
* [ ] Make sure that all the data in the topic is data-driven (from the repository)
* [ ] It would be nice to be able to get away with just once instance of the tree in the document
* [ ] Maybe listing contributors to a topic or all the docs (optional)
* [ ] Maybe we could also show last-edited date on a topic
* [ ] Microsoft shows “estimated time to read” at the top of topics
* [ ] Sharing of documents would be nice
* [ ] Need to be able to refer to other TOC files as a single node from within a TOC file
    * [ ] We want to do the same inside a topic by having a special placeholder in the doc (we are thinking about a kava-topic HTML tag
* [ ] Edit link right in the topic
* [ ] Threaded discussions, or at least comments
* [ ] PDF export
* [ ] Need to get the search implemented
   * [ ] Need the more advanced ability to do a full search across all documents using Azure Search
   * [ ] Keywords and description need to be passed to the fully indexed search as relatively high priority fields
   * [x] Need the basic ability to filter the tree
   * [x] Tree filtering should respect keywords
* [x] Make the main menu configurable
* [x] Allow overriding of CSS styles
* [x] The tree should invisibly contain keywords so a tree filter will also include keywords
* [x] Document title needs to be picked up from the repository
* [x] Document description needs to be picked up from the repository
* [x] Document keywords needs to be picked up from the repository
* [x] Switch from prettify to highlight

