using DocHound.Interfaces;
using System;
using System.Threading.Tasks;

namespace DocHound.ContentLoaders
{
    public abstract class ContentLoaderBase : IContentLoader
    {
        public abstract Task<string> GetTocJsonAsync();

        public abstract Task<string> GetContentStringAsync(string link, string topicTypeName);

        public abstract string GetImageRootUrl(string contentUri);

        public virtual void SetSettingsProvider(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
        }

        private ISettingsProvider _settingsProvider;

        public T GetSetting<T>(Settings setting) => _settingsProvider.GetSetting<T>(setting);

        public abstract string GetLogoUrl();


        // Some useful methods


        protected virtual async Task<string> WebGet(string url) => await WebClientEx.GetStringAsync(url);

        /// <summary>Returns just the file name part of a full path</summary>
        /// <param name="path">The full path to the file</param>
        /// <returns>File name</returns>
        protected virtual string JustFileName(string path)
        {
            path = path.Replace("/", "\\");
            var parts = path.Split('\\');
            if (parts.Length > 0)
                return parts[parts.Length - 1];
            return string.Empty;
        }

        protected virtual string JustPath(string path)
        {
            var path2 = path.Replace("/", "\\");
            return path.Substring(0, At("\\", path2, Occurs("\\", path2)) - 1);
        }

        protected virtual int Occurs(string searchString, string stringSearched)
        {
            var position = 0;
            var occurred = 0;
            do
            {
                //Look for the search string in the expression
                position = stringSearched.IndexOf(searchString, position, StringComparison.Ordinal);

                if (position < 0) break;
                //Increment the occurred counter based on the current mode we are in
                occurred++;
                position++;
            } while (true);

            //Return the number of occurrences
            return occurred;
        }

        protected virtual int At(string searchFor, string searchIn, int occurrence)
        {
            int counter;
            var occurred = 0;
            var position = 0;

            //Loop through the string and get the position of the requiref occurrence
            for (counter = 1; counter <= occurrence; counter++)
            {
                position = searchIn.IndexOf(searchFor, position, StringComparison.Ordinal);

                if (position < 0) break;
                //Increment the occurred counter based on the current mode we are in
                occurred++;

                //Check if this is the occurrence we are looking for
                if (occurred == occurrence) return position + 1;
                position++;
            }
            return 0;
        }

        public static string GetNormalizedName(string name) // This method also exists in topic.js as getNormalizedName() and should be kept in sync with this method.
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;

            var normalizedName = name;
            normalizedName = normalizedName.Replace(" ", "-");
            normalizedName = normalizedName.Replace("%20", "-");
            //normalizedName = normalizedName.Replace("/", "-");
            normalizedName = normalizedName.Replace(",", string.Empty);
            normalizedName = normalizedName.Replace("(", string.Empty);
            normalizedName = normalizedName.Replace(")", string.Empty);
            normalizedName = normalizedName.Replace("?", string.Empty);
            normalizedName = normalizedName.Replace(":", string.Empty);
            normalizedName = normalizedName.Replace("#", string.Empty);
            normalizedName = normalizedName.Replace("&", string.Empty);
            return normalizedName;
        }
    }
}
