namespace DocHound.Interfaces
{
    public interface ITopicRenderer
    {
        string RenderToHtml(TopicInformation topic, string imageRootUrl = "", ISettingsProvider settings = null);
    }

}
