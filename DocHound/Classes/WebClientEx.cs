using System;
using System.Net;
using System.Net.Cache;
using System.Threading.Tasks;

namespace DocHound.Classes
{
    public class WebClientEx : WebClient
    {
        public WebClientEx()
        {
            CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = base.GetWebRequest(address) as HttpWebRequest;
            if (webRequest == null) return null;
            webRequest.KeepAlive = false;
            webRequest.Headers.Add(HttpRequestHeader.CacheControl, "no-cache");
            return webRequest;
        }

        public static async Task<string> GetStringAsync(string url)
        {
            using (var client = new WebClientEx())
                return await client.DownloadStringTaskAsync(url);
        }
    }
}