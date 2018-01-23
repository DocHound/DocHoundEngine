namespace DocHound.Interfaces
{
    public static class TopicTypeNames
    {
        public static string Markdown => "markdown";
        public static string Html => "html";
        public static string VstsWorkItem => "vsts-workitem";
        public static string VstsWorkItemQuery => "vsts-workitem-query";
        public static string VstsWorkItemQueryToc => "vsts-workitem-query:toc";
        public static string VstsWorkItemQueries => "vsts-workitem-queries";
        public static string VstsWorkItemQueriesToc => "vsts-workitem-queries:toc";
    }

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
            return IsMatch(typeName, TopicTypeNames.VstsWorkItem) || 
                   IsMatch(typeName, TopicTypeNames.VstsWorkItemQueries) || 
                   IsMatch(typeName, TopicTypeNames.VstsWorkItemQueriesToc) ||
                   IsMatch(typeName, TopicTypeNames.VstsWorkItemQuery) ||
                   IsMatch(typeName, TopicTypeNames.VstsWorkItemQueryToc); 
        }

        public static string GetNormalizeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return string.Empty;
            return typeName.ToLowerInvariant();
        }
    }
}
