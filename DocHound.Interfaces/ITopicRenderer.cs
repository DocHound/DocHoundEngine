namespace DocHound.Interfaces
{
    public interface ITopicRenderer
    {
        string RenderToHtml(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null);
        string GetTemplateName(TopicInformation topic, string suggestedTemplateName, ISettingsProvider settings = null);
    }

}
