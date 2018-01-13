using System.Threading.Tasks;

namespace DocHound.Classes
{
    public static class ContentSniffer
    {
        public async static Task<object> DownloadContent(DownloadMode mode, string url)
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