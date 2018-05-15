using System.Threading.Tasks;

namespace DocHound.Interfaces
{
    public interface IContentLoader
    {
        /// <summary>
        /// Loads the table of contents in JSON format from the specified resource
        /// </summary>
        /// <returns>JSON formatted table of contents</returns>
        Task<string> GetTocJsonAsync();

        /// <summary>
        /// Loads the specified content as a string (such as markdown)
        /// </summary>
        /// <param name="uri">Resource identifier for the content (such as a URL)</param>
        /// <param name="topicTypeName">Topic type</param>
        /// <returns>Raw topic contents as a string</returns>
        Task<string> GetContentStringAsync(string link, string topicTypeName);

        /// <summary>
        /// Returns the root URL for images in this repository, based on the provided overall URL
        /// </summary>
        /// <param name="contentUri">Overall URL used to load content</param>
        /// <returns>Image URL relative to the provided contentUrl</returns>
        string GetImageRootUrl(string contentUri);

        /// <summary>
        /// Provides a settings provider that can be used to read settings
        /// </summary>
        /// <param name="settingsProvider">Settings provider</param>
        void SetSettingsProvider(ISettingsProvider settingsProvider);

        /// <summary>
        /// Allows reading of settings
        /// </summary>
        /// <typeparam name="T">Expected return type</typeparam>
        /// <param name="setting">Setting identifier</param>
        /// <returns>Setting value</returns>
        T GetSetting<T>(Settings setting);

        /// <summary>
        /// Returns the appropriate logo URL for this repository
        /// </summary>
        /// <returns>Full Logo URL</returns>
        string GetLogoUrl();
    }
}
