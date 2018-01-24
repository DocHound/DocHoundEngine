namespace DocHound.Classes
{
    public static class VstsTagHelper
    {
        public static string GetStateHtmlTag(string state)
        {
            var stateClass = state.ToLowerInvariant().Replace(" ", "-").Replace("--","-").Replace("--","-");
            stateClass = "vsts-state vsts-state-" + stateClass;
            return "<div class=\"" + stateClass + "\"> </div>";
        }
    }
}