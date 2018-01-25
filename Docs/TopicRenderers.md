# Topic Renderers

Topics are rendered based on the type specified for each topic. Each topic has a registered "Topic Renderer" object, which implements the ITopicRenderer interface. The following diagram shows the fundamental setup:

```nomnoml
[<abstract>ITopicRenderer|RenderToHtml();RenderToJson();GetTemplateName()]
[ITopicRenderer]<:-[MarkdownTopicRenderer]
[ITopicRenderer]<:-[HtmlTopicRenderer]
[ITopicRenderer]<:-[WorkItemTopicRenderer]

#fontSize: 10
#lineWidth: 1
#fill: #d2dbee
```

## ITopicRenderer Interface

### RenderToHtml()

This is the most commonly utilized method. This method produces HTML output based on the provided input. For instance, the Markdown renderer processes the markdown in this method (provided by means of the first parameter) and turns in into HTML, which is the return value. 

If the renderer does not directly produce HTML output (usually because an advanced Razor template is used to create the output), it can simply return string.Empty from this method.

> Note: The return value of this method is assigned to Model.Html, from where itcan be accessed in the Razor templates.

### RenderToJson()

Some topic renderers may choose the create JSON output for further processing by templates. This can be implemented through the RenderToJson() method. Very often, the input already is JSON, in which case the input JSON can simply be returned.

If the renderer does not intend to create any JSON output, it can simply return string.Empty from this method. 

> Note: The return value of this method is assigned to Model.Json, from where itcan be accessed in the Razor templates. Furthermore, a live version of the object the JSON string represents can be accessed through Model.JsonObject.

### GetTemplateName()

This method returns the name of the Razor template that is to be used with this topic type. One of the parameter suggests a name. When in doubt, return the suggested name.

## Example Topic Renderers

### Simple Topic Renderer

The following renderer simply renders today's date as HTML:

```cs
public class HtmlTopicRenderer : ITopicRenderer
{
    public string RenderToHtml(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null)
    {
        return "<p>" + DateTime.Now.ToString() + "</p>";
    }

    public string RenderToJson(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null) => string.Empty;
    
    public string GetTemplateName(TopicInformation topic, string suggestedTemplateName, ISettingsProvider settings = null) => suggestedTemplateName;
}
```

### Simple Topic Renderer using JSON

```cs
public class HtmlTopicRenderer : ITopicRenderer
{
    public string RenderToJson(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null)
    {
        return "{ \"date\": {" + DateTime.Now.ToString() + "} }";
    }

    public string RenderToHtml(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null) => string.Empty;
    
    public string GetTemplateName(TopicInformation topic, string suggestedTemplateName, ISettingsProvider settings = null) => "DateTemplate";
}
```

Note that this returns the name "DateTemplate" as the Razor template that is to be rendered. Therefore, we can define the Razor template (DataTemplate.cshtml) like so:

```html
@model DocHound.Models.Docs.TopicViewModel

@{
    Layout = "../_Layout.cshtml";
}

<article class="content-container">
    <p>Date: @Model?.JsonObject?.date</p>

    @Html.Partial("../_PartialDefaultBottomNavigation.cshtml", Model)
</article>

<aside class="sidebar">
    @Html.Partial("../_PartialDefaultSidebar.cshtml", Model)
</aside>
```

> Note: The "Elvis-Operator" (?.) is not strictly required, but it is considered good form, since it prevents null reference exceptions in case the JSON was not valid.

### The Markdown Topic Renderer

For a real-world example, consider this implementation of the markdown topic renderer uses to implement markdown functionality. This renderer uses the raw topic that is provided (in markdown format) and turns it into HTML. Note that is uses configured settings to decide which features to enable:

```cs
public class MarkdownTopicRenderer : ITopicRenderer
{
    public string RenderToHtml(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null)
    {
        if (string.IsNullOrEmpty(topic.OriginalContent)) return string.Empty;
        return MarkdownToHtml(topic, imageRootUrl, settings);
    }

    private string MarkdownToHtml(TopicInformation topic, string imageRootUrl, ISettingsProvider settings)
    {
        var builder = new MarkdownPipelineBuilder();
        BuildPipeline(builder, settings);
        var html = Markdig.Markdown.ToHtml(topic.OriginalContent, builder.Build());
        html = HtmlHelper.FixHtml(html, imageRootUrl);
        return html;
    }

    protected virtual MarkdownPipelineBuilder BuildPipeline(MarkdownPipelineBuilder builder, ISettingsProvider settings)
    {
        if (settings.GetSetting<bool>(Settings.UseAbbreviations)) builder = builder.UseAbbreviations();
        if (settings.GetSetting<bool>(Settings.UseAutoIdentifiers)) builder = builder.UseAutoIdentifiers();
        if (settings.GetSetting<bool>(Settings.UseAutoLinks)) builder = builder.UseAutoLinks();
        if (settings.GetSetting<bool>(Settings.UseCitations)) builder = builder.UseCitations();
        if (settings.GetSetting<bool>(Settings.UseCustomContainers)) builder = builder.UseCustomContainers();
        if (settings.GetSetting<bool>(Settings.UseDiagramsMermaid) || settings.GetSetting<bool>(Settings.UseDiagramsNomnoml)) builder = builder.UseDiagrams();
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

        return builder;
    }

    public string GetTemplateName(TopicInformation topic, string suggestedTemplateName, ISettingsProvider settings = null) => suggestedTemplateName;
    public string RenderToJson(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null) => string.Empty;
}
```

### VSTS Work Item Renderer

The following example is the real-world implementation of the VSTS Work Item topic renderer. Note that this renderer doesn't actually "render" anything as such. Instead, it simply uses the original content, which happens to be JSON retrieved from VSTS, and returns it as the JSON to use for the output. The more important aspect is that the renderer decides which template is best suited, depending on the specific topic type (this renderer is used for all VSTS work item related features, such as work items, query results, and lists of queries):

```cs
public class WorkItemTopicRenderer : ITopicRenderer
{
    public string RenderToJson(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null) => topic.OriginalContent;

    public string GetTemplateName(TopicInformation topic, string suggestedTemplateName, ISettingsProvider settings = null)
    {
        if (TopicTypeHelper.IsMatch(topic.Type, TopicTypeNames.VstsWorkItem)) return "Vsts/WorkItem";
        if (TopicTypeHelper.IsMatch(topic.Type, TopicTypeNames.VstsWorkItemQuery)) return "Vsts/WorkItemQueryResult";
        if (TopicTypeHelper.IsMatch(topic.Type, TopicTypeNames.VstsWorkItemQueries)) return "Vsts/WorkItemQueries";
            
        return suggestedTemplateName;
    }

    public string RenderToHtml(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null) => string.Empty;
}
```

The different templates then pick up the JSON and turn it into a meaningful topic. As a represantative example, here is a simplified version of WorkItem.cshtml:

```html
@model DocHound.Models.Docs.TopicViewModel

@{
    Layout = "../_Layout.cshtml";
}

@section Styles {
    <link href="@(Model.ThemeFolderRaw)/Vsts/Vsts.css" type="text/css" rel="stylesheet" />
}

<article class="content-container">
    @{
        dynamic workItems = Model.JsonObject;
        if (workItems?.value.Count > 0)
        {
            var workItem = workItems.value[0];
            var fields = workItem.fields;

            // Title
            var title = fields["System.Title"].ToString().Trim();
            if (fields["System.WorkItemType"] != null) {
                title = fields["System.WorkItemType"].ToString().Trim() + ": " + title;
            }
            <h1 id="@title">@title</h1>

            // Header info table
            <div class="vsts-workitem-statearea">
            <table>
            @if (fields["System.AssignedTo"] != null) {
                <tr><td class="vsts-workitem-label">Assigned To:</td><td class="vsts-workitem-data">@fields["System.AssignedTo"]</td></tr>            
            }
            @if (fields["System.WorkItemType"] != null) {
                <tr><td class="vsts-workitem-label">Work Item Type:</td><td class="vsts-workitem-data">@fields["System.WorkItemType"]</td></tr>            
            }
            /* More here */
            </table>
            </div>

            // Description
            <h2 id="workitemdescription">Description</h2>
            @if (fields["System.Description"] != null) {
                @Html.Raw(Model.Vsts.FixHtml(fields["System.Description"]))
            }
            else {
                <p><i>n/a</i></p>
            }
            /* More here */
        }
    }

    @Html.Partial("../_PartialDefaultBottomNavigation.cshtml", Model)
</article>

<aside class="sidebar">
    @Html.Partial("../_PartialDefaultSidebar.cshtml", Model)
</aside>
```
