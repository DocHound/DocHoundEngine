using System;
using System.Threading.Tasks;

namespace DocHound.Classes
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

        public static async Task<string> DownloadGitHubApiContent(string owner, string repository, string pat, string fileName)
        {
            try
            {
                var client = new GithubRepositoryParser(owner, repository, pat);
                var content = await client.GetItemContent(fileName);
                return content?.Text;
            }
            catch
            {
            }
            return null;
        }
    }

    public enum DownloadMode
    {
        HttpGet
    }
}