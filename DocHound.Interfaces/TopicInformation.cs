namespace DocHound.Interfaces
{
    public class TopicInformation
    {
        public string OriginalName { get; set; }
        public string OriginalNameNormalized => OriginalName.Trim().ToLowerInvariant();

        public string OriginalContent { get; set; }
        public string Type { get; set; }
    }
}
