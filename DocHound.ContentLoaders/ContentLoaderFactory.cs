using DocHound.Interfaces;
using System;
using System.Collections.Generic;

namespace DocHound.ContentLoaders
{
    public static class ContentLoaderFactory
    {
        private static readonly Dictionary<RepositoryTypes, Type> RegisteredLoaders = new Dictionary<RepositoryTypes, Type>();

        public static bool RegisterLoader<T>(RepositoryTypes repositoryType) where T : IContentLoader, new()
        {
            if (!RegisteredLoaders.ContainsKey(repositoryType))
            {
                RegisteredLoaders.Add(repositoryType, typeof(T));
                return true;
            }
            return false;
        }

        public static IContentLoader GetContentLoader(RepositoryTypes repositoryType, ISettingsProvider settingsProvider)
        {
            // TODO: We probably need to figure out a way to pass in settings to the loader

            if (RegisteredLoaders.ContainsKey(repositoryType))
            {
                var loaderType = RegisteredLoaders[repositoryType];
                var instance = Activator.CreateInstance(loaderType) as IContentLoader;
                instance?.SetSettingsProvider(settingsProvider);
                return instance;
            }

            return null;
        }
    }
}
