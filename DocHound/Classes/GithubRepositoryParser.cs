using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GithubGraphQl
{
    public class GithubRepositoryParser
    {
        private readonly string _owner;
        private readonly string _repository;

        public string Token { get; set; }
        public string ApiName { get; set; } = "APITest";
        public string ApiVersion { get; set; } = "0.1";

        public string UserAgent { get; set; } = "KavaDocs HTTP Client";


        public GithubRepositoryParser(string owner,
            string repository,
            string token = null,
            string apiName = null,
            string apiVersion = null)
        {

            _owner = owner;
            _repository = repository;

            if (!string.IsNullOrEmpty(token))
                Token = token;

            if (!string.IsNullOrEmpty(apiName))
                ApiName = apiName;
            if (!string.IsNullOrEmpty(apiVersion))
                ApiName = apiName;
        }


        #region Get Folder Tree Helpers        

        public async Task<List<GithubFolderItem>> GetFolder(string branch, string path = "")
        {
            string relativePath = string.Empty;
            if (!string.IsNullOrEmpty(path))
                relativePath = "/" + path.Replace("\\", "//").Replace("//", "/");

            string url = $"https://api.github.com/repos/{_owner}/{_repository}/git/trees/{branch}?recursive=1";
            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);

            if (!string.IsNullOrEmpty(Token))
                client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + Token);

            client.CachePolicy =
                new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.Reload);

            var json = await client.DownloadStringTaskAsync(new Uri(url)).ConfigureAwait(false);

            // collect entire tree

            var list = new List<JsonItem>();

            // Capture the raw list
            dynamic jobject = JObject.Parse(json) as dynamic;
            var tree = jobject.tree;
            foreach (var item in tree)
            {
                string fullPath = item.path;


                var jItem = new JsonItem()
                {
                    Path = fullPath,
                    Type = item.type
                };
                if (item.type == "blob")
                    jItem.Size = item.size;

                list.Add(jItem);
            }

            var parsed = await ParseListIntoTree(list, path);


            return parsed;
        }

        public async Task<List<GithubFolderItem>> ParseListIntoTree(IEnumerable<JsonItem> list, string path)
        {
            var glist = new List<GithubFolderItem>();
            string dashed = path + "/";
            int dashLength = dashed.Length;

            var parsed = list.Where(i =>
            {
                if (string.IsNullOrEmpty(path))
                    return !i.Path.Contains("/");

                // not our path
                if (!i.Path.StartsWith(dashed))
                    return false;

                // this must be a sub-path
                if (i.Path.LastIndexOf('/') > dashLength)
                    return false;


                // Yup
                return true;
            });

            foreach (var item in parsed)
            {
                var gitem = new GithubFolderItem()
                {
                    FullPath = item.Path,
                };
                gitem.Name = gitem.FullPath;
                if (gitem.FullPath.Contains("/"))
                    gitem.Name = Path.GetFileName(gitem.FullPath);

                if (item.Type == "tree")
                    gitem.Items = await ParseListIntoTree(list, gitem.FullPath);
                else
                    gitem.Size = item.Size;

                glist.Add(gitem);
            }

            return glist;
        }


        #endregion

        #region Item Helpers

        /// <summary>
        /// Retrieve the contents of an file/resource from a
        /// from a Github repository. Provide a path to retrieve in
        /// `branch:path\file.md` format.
        /// </summary>
        /// <param name="path">path to a resource: `master:file.txt` or `master:path/file.txt`</param>
        /// <returns>file content or throws</returns>
        public async Task<GithubContentItem> GetItemContent(string path)
        {
            //https://api.github.com/repos/RickStrahl/MarkdownMonster/contents/README.md

            string url = $"https://api.github.com/repos/{_owner}/{_repository}/contents/{path}";
            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);

            if (!string.IsNullOrEmpty(Token))
                client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + Token);

            client.CachePolicy =
                new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.Reload);

            Console.WriteLine(url);
            var json = await client.DownloadStringTaskAsync(new Uri(url)).ConfigureAwait(false);

            var jObject = JObject.Parse(json);


            int size = jObject["size"].Value<int>();
            string name = jObject["name"].Value<string>();
            string sha = jObject["sha"].Value<string>();

            var item = new GithubContentItem()
            {
                ByteSize = size,
                FullPath = path,
                Name = name,
                Sha = sha
            };

            string binExtensions = "|png|jpg|jpeg|gif|tiff|tif|bmp|zip|7z|rar|tar|pdf|doc|xls|ppt|";

            var ext = Path.GetExtension(item.Name)?.ToLower();
            if (!string.IsNullOrEmpty(ext) && binExtensions.Contains("|" + ext + "|"))
                item.IsBinary = true;
            if (!item.IsBinary)
            {
                var base64Content = jObject["content"].Value<string>();
                item.Text = Encoding.UTF8.GetString(Convert.FromBase64String(base64Content));
            }

            return item;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<GithubContentItem> GetItemContent(GithubFolderItem item)
        {
            return await GetItemContent(item.FullPath);
        }

        #endregion
    }


    /// <summary>
    /// Holds information about an individual folder item
    /// </summary>    
    public class GithubFolderItem
    {
        /// <summary>
        /// The simple name of this file/resource
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type: blob or tree
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Full Github path to this item
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Size of a file resource
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// If this is a tree, contains child FolderItems entries
        /// </summary>
        public List<GithubFolderItem> Items { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Items?.Count ?? 0})";
        }
    }


    /// <summary>
    /// An individual Github file/resource item
    /// </summary>
    public class GithubContentItem
    {
        /// <summary>
        /// The simple name of the resource blob
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The full path to the resource
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Size in bytes
        /// </summary>
        public int ByteSize { get; set; }

        /// <summary>
        /// If the contents is binary data
        /// </summary>
        public bool IsBinary { get; set; }

        /// <summary>
        /// Contents of the file/resource
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Sha hash for this file for easy access
        /// https://api.github.com/repos/RickStrahl/MarkdownMonster/git/blobs/68f52879fc3c4ae4299d921ab266bd707415adb3
        /// </summary>
        public string Sha { get; set; }

        public override string ToString()
        {
            return $"{Name}  ({ByteSize.ToString("n0")})";
        }
    }


    public class JsonItem
    {
        public string Path { get; set; }
        public string Type { get; set; }

        public int Size { get; set; }
    }

}
