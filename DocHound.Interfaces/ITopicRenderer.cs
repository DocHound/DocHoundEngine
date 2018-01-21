namespace DocHound.Interfaces
{
    public interface ITopicRenderer
    {
        string RenderToHtml(TopicInformation topic, string imageRootUrl = "", TocSettings settings = null);
    }

}
