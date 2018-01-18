# Kava Docs - To-Do List

## Most-Haves for Minimum Viable Product

* [ ] Accounts and Log-Ins
* [ ] A few more supported sources
    * [ ] TFS Work Items
    * [ ] DOCX
    * [ ] OneNote
    * [ ] ...?
* [ ] Full index search
* [ ] Wildcard domain support

## Rendering Engine

* [X] Make the current tab work on mobile (responsive)
* [x] Handle image paths relative to documents stored in root folders properly
* [ ] Themes
    * [ ] Dark theme
    * [ ] Sepia theme
* [ ] Allow for different templates (or CSS styles) for different topics or topic trees. For instance, I would like to show the CODE Ohana booklet topics with a different page template (sepia Hawaiian theme) from other topics
* [x] Showing the outline with indent levels
* [ ] Support copy & Paste of code snippets (have an icon in the header that people can click on to copy)
    * [ ] Maybe we could even show the name of the language at the top of the snippet
* [ ] Styling
    * [ ] Checkboxes (such as in this list) currently look pretty bad
    * [x] H3 needs to get their own style
    * [x] Need good default styles for tables
    * [x] Need good default style for blockquotes
* [ ] Markdown improvements
    * [ ] Support FontAwesome
    * [ ] Take a look at all the standard (optional) Markdig features to see what should be supported in ours (probably a lot)
    * [ ] Would be nice to support math notation
    * [ ] Would be nice to support scientific notations
    * [ ] Would be nice to support flow charts and other charts (such as UML)

## Overall Processing

* [x] Make the main menu configurable
* [ ] Allow overriding of CSS styles
* [ ] The fav-icon needs to be configurable
* [ ] Make sure that all the data in the topic is data-driven (from the repository)
* [x] The tree should invisibly contain keywords so a tree filter will also include keywords
* [ ] It would be nice to be able to get away with just once instance of the tree in the document
* [x] Document title needs to be picked up from the repository
* [x] Document description needs to be picked up from the repository
* [x] Document keywords needs to be picked up from the repository
* [x] Switch from prettify to highlight
* [ ] Maybe listing contributors to a topic or all the docs (optional)
* [ ] Maybe we could also show last-edited date on a topic
* [ ] Microsoft shows “estimated time to read” at the top of topics
* [ ] Sharing of documents would be nice
* [ ] Need to be able to refer to other TOC files as a single node from within a TOC file
    * [ ] We want to do the same inside a topic by having a special placeholder in the doc (we are thinking about a kava-topic HTML tag
* [ ] Need to get the search implemented
   * [x] Need the basic ability to filter the tree
   * [x] Tree filtering should respect keywords
   * [ ] Need the more advanced ability to do a full search across all documents using Azure Search
   * [ ] Keywords and description need to be passed to the fully indexed search as relatively high priority fields
* [ ] Edit link right in the topic
* [ ] Threaded discussions, or at least comments
* [ ] PDF export

