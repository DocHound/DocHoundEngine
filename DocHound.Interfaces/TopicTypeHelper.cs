namespace DocHound.Interfaces
{
    //public static class TopicTypeNames
    //{
    //    public static string Markdown => "markdown";
    //    public static string Html => "html";
    //    public static string ImageUrl => "imageurl";
    //    public static string VstsWorkItem => "vsts-workitem";
    //    public static string VstsWorkItemQuery => "vsts-workitem-query";
    //    public static string VstsWorkItemQueryToc => "vsts-workitem-query:toc";
    //    public static string VstsWorkItemQueries => "vsts-workitem-queries";
    //    public static string VstsWorkItemQueriesToc => "vsts-workitem-queries:toc";
    //}

    public static class TopicTypeHelper
    {
        public static bool IsMatch(string typeString, string compareTo)
        {
            if (string.IsNullOrEmpty(typeString)) return false;
            if (string.IsNullOrEmpty(compareTo)) return false;
            return GetNormalizeName(typeString) == GetNormalizeName(compareTo);
        }

        public static bool IsVstsWorkItemType(string typeName)
        {
            return IsMatch(typeName, TopicBodyFormats.VstsWorkItem) || 
                   IsMatch(typeName, TopicBodyFormats.VstsWorkItemQueries) || 
                   IsMatch(typeName, TopicBodyFormats.VstsWorkItemQueriesToc) ||
                   IsMatch(typeName, TopicBodyFormats.VstsWorkItemQuery) ||
                   IsMatch(typeName, TopicBodyFormats.VstsWorkItemQueryToc); 
        }

        public static string GetNormalizeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return string.Empty;
            return typeName.ToLowerInvariant();
        }

        public static string GetTopicTypeFromLink(string link)
        {
            link = link.ToLowerInvariant();

            if (link.EndsWith(".md")) return TopicBodyFormats.Markdown;
            if (link.EndsWith(".txt")) return TopicBodyFormats.Markdown;
            if (link.EndsWith(".htm")) return TopicBodyFormats.Html;
            if (link.EndsWith(".html")) return TopicBodyFormats.Html;
            if (link.EndsWith(".jpg")) return TopicBodyFormats.ImageUrl;
            if (link.EndsWith(".jpeg")) return TopicBodyFormats.ImageUrl;
            if (link.EndsWith(".png")) return TopicBodyFormats.ImageUrl;
            if (link.EndsWith(".gif")) return TopicBodyFormats.ImageUrl;
            if (link.EndsWith(".tiff")) return TopicBodyFormats.ImageUrl;

            return string.Empty;
        }
    }
}