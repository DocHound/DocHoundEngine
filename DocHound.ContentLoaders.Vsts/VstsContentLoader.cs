using DocHound.Interfaces;
using System;
using System.Threading.Tasks;

namespace DocHound.ContentLoaders.Vsts
{
    public class VstsContentLoader : ContentLoaderBase
    {
        public override string GetImageRootUrl(string contentUri)
        {
            var imageRootUrl = "/___FileProxy___?mode=" + RepositoryTypeNames.VstsGit + "&path=";
            if (contentUri.Contains("/"))
                imageRootUrl += JustPath(contentUri) + "/";
            return imageRootUrl;
        }

        public override async Task<string> GetContentStringAsync(string link, string topicTypeName)
        {
            if (string.IsNullOrEmpty(link)) return string.Empty;
            return await VstsHelper.GetFileContents(link, GetSetting<string>(Settings.VstsInstance), GetSetting<string>(Settings.VstsProjectName), GetSetting<string>(Settings.VstsDocsFolder), GetSetting<string>(Settings.VstsPat), GetSetting<string>(Settings.VstsApiVersion));
        }

        public override string GetLogoUrl()
        {
            var logoUrl = GetSetting<string>(Settings.LogoPath);
            var logoUrlLower = logoUrl.ToLowerInvariant();
            var logoUrlIsAbsolute = true;
            if (!logoUrl.StartsWith("http://") && !logoUrl.StartsWith("https://")) logoUrlIsAbsolute = false;
            if (!logoUrlIsAbsolute)
                logoUrl = $"/___FileProxy___?mode=vstsgit&path={logoUrl}";
            return logoUrl;
        }

        public override async Task<string> GetTocJsonAsync()
        {
            var tocJson = await VstsHelper.GetTocJson(GetSetting<string>(Settings.VstsInstance), GetSetting<string>(Settings.VstsProjectName), GetSetting<string>(Settings.VstsDocsFolder), GetSetting<string>(Settings.VstsPat), GetSetting<string>(Settings.VstsApiVersion));
            return tocJson;
        }
    }
}
