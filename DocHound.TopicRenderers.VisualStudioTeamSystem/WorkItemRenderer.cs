using DocHound.Interfaces;

namespace DocHound.TopicRenderers.VisualStudioTeamSystem
{
    public class WorkItemTopicRenderer : ITopicRenderer
    {
        // We simply use the original JSON and pass it on as the content, which will then be picked up by a template
        public string RenderToJson(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null) => topic.OriginalContent;

        public string GetTemplateName(TopicInformation topic, string suggestedTemplateName, ISettingsProvider settings = null)
        {
            if (TopicTypeHelper.IsMatch(topic.Type, TopicTypeNames.VstsWorkItem)) return "Vsts/WorkItem";
            if (TopicTypeHelper.IsMatch(topic.Type, TopicTypeNames.VstsWorkItemQuery)) return "Vsts/WorkItemQueryResult";
            if (TopicTypeHelper.IsMatch(topic.Type, TopicTypeNames.VstsWorkItemQueries)) return "Vsts/WorkItemQueries";
            
            return suggestedTemplateName; // This is probably not good, but so be it
        }

        public string RenderToHtml(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null) => string.Empty;
    }
}
