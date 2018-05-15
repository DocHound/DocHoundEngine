using System.Threading.Tasks;

namespace DocHound.ContentLoaders
{
    public static class ContentSniffer
    {
        public static async Task<string> DownloadContent(DownloadMode mode, string url)
        {
            switch (mode)
            {
                case DownloadMode.HttpGet:
                    return await WebClientEx.GetStringAsync(url);
            }

            return null;
        }
    }

    public enum DownloadMode
    {
        HttpGet
    }
}
