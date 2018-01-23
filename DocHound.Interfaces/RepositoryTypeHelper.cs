namespace DocHound.Interfaces
{
    public static class RepositoryTypeNames
    {
        public static string GitHubRaw => "githubraw";
        public static string VstsGit => "vstsgit";
        public static string VstsWorkItemTracking => "vstswit";
    }

    public enum RepositoryTypes
    {
        Undefined,
        GitHubRaw,
        VstsGit,
        VstsWorkItemTracking
    }

    public static class RepositoryTypeHelper
    {
        public static RepositoryTypes GetTypeFromTypeName(string typeName)
        {
            if (IsMatch(typeName, RepositoryTypeNames.GitHubRaw)) return RepositoryTypes.GitHubRaw;
            if (IsMatch(typeName, RepositoryTypeNames.VstsGit)) return RepositoryTypes.VstsGit;
            if (IsMatch(typeName, RepositoryTypeNames.VstsWorkItemTracking)) return RepositoryTypes.VstsWorkItemTracking;

            return RepositoryTypes.Undefined;
        }

        public static bool IsMatch(string typeString, string compareTo)
        {
            if (string.IsNullOrEmpty(typeString)) return false;
            if (string.IsNullOrEmpty(compareTo)) return false;
            return GetNormalizeName(typeString) == GetNormalizeName(compareTo);
        }

        public static string GetNormalizeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return string.Empty;
            return typeName.ToLowerInvariant();
        }
    }
}