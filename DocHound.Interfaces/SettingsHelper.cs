using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DocHound.Interfaces
{
    public static class SettingsHelper
    {
        public static void SetGlobalConfiguration(object configuration)
        {
            GlobalConfiguration = configuration;
        }

        public static T GetSetting<T>(string setting, dynamic repositorySettings = null, dynamic topicSettings = null, dynamic requestRootSettings = null)
        {
            // Topic-specific settings overrule everything else
            if (IsDynamicSettingSet(setting, topicSettings))
                return GetDynamicSetting<T>(setting, topicSettings);

            // Settings for the whole repository still overrule app-global settings
            if (IsDynamicSettingSet(setting, repositorySettings))
                return GetDynamicSetting<T>(setting, repositorySettings);

            // If we have request-specific root settings, we consider that
            if (IsDynamicSettingSet(setting, requestRootSettings))
                return GetDynamicSetting<T>(setting, requestRootSettings);

            // We haven't found anything yet, so we take a look at the global app settings
            if (GlobalConfiguration != null)
            {
                var dynamicGlobalConfiguraiton = (dynamic)GlobalConfiguration;
                var value = dynamicGlobalConfiguraiton[setting];
                if (value != null) return value;
            }

            // We still haven't found anything, so we check if the setting enum defines a default value for the current setting
            var type = typeof(Settings);
            var memInfo = type.GetMembers().FirstOrDefault(m => SettingsHelper.IsMatch(setting, m.Name));
            if (memInfo != null)
            {
                var attributes = memInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false);
                if (attributes.Length > 0)
                    return (T)((DefaultValueAttribute)attributes[0]).Value;
            }

            return default(T);
        }

        public static T GetSetting<T>(Settings setting, dynamic repositorySettings = null, dynamic topicSettings = null, dynamic requestRootSettings = null)
        {
            // Topic-specific settings overrule everything else
            if (IsDynamicSettingSet(setting, topicSettings))
                return GetDynamicSetting<T>(setting, topicSettings);

            // Settings for the whole repository still overrule app-global settings
            if (IsDynamicSettingSet(setting, repositorySettings))
                return GetDynamicSetting<T>(setting, repositorySettings);

            // If we have request-specific root settings, we consider that
            if (IsDynamicSettingSet(setting, requestRootSettings))
                return GetDynamicSetting<T>(setting, requestRootSettings);

            // We haven't found anything yet, so we take a look at the global app settings
            if (GlobalConfiguration != null)
            {
                var dynamicGlobalConfiguraiton = (dynamic)GlobalConfiguration;
                var value = dynamicGlobalConfiguraiton[GetSettingNameFromSetting(setting)];
                if (value != null) return value;
            }

            // We still haven't found anything, so we check if the setting enum defines a default value for the current setting
            var type = typeof(Settings);
            var memInfo = type.GetMember(setting.ToString());
            if (memInfo.Length > 0)
            {
                var attributes = memInfo[0].GetCustomAttributes(typeof(DefaultValueAttribute), false);
                if (attributes.Length > 0)
                    return (T)((DefaultValueAttribute)attributes[0]).Value;
            }

            return default(T);
        }

        public static bool IsSettingSet(Settings setting, dynamic repositorySettings = null, dynamic topicSettings = null)
        {
            // Topic-specific settings overrule everything else
            if (IsDynamicSettingSet(setting, topicSettings)) return true;

            // Settings for the whole repository still overrule app-global settings
            if (IsDynamicSettingSet(setting, repositorySettings)) return true;

            // We haven't found anything yet, so we take a look at the global app settings
            if (GlobalConfiguration != null)
            {
                var dynamicGlobalConfiguraiton = (dynamic)GlobalConfiguration;
                var value = dynamicGlobalConfiguraiton[GetSettingNameFromSetting(setting)];
                if (value != null) return true;
            }

            return false;
        }

        private static T GetDynamicSetting<T>(string setting, dynamic settingsObject)
        {
            if (settingsObject == null) return default(T);

            var settingsDictionary = settingsObject as IDictionary<string, JToken>;
            if (settingsDictionary != null) 
                return GetDictionarySetting<T>(setting, settingsDictionary);

            var settingsDictionary2 = settingsObject as IDictionary<string, object>;
            if (settingsDictionary2 != null)
                return GetDictionarySetting<T>(setting, settingsDictionary2);

            return default(T);
        }

        private static T GetDynamicSetting<T>(Settings setting, dynamic settingsObject)
        {
            if (settingsObject == null) return default(T);

            var settingsDictionary = settingsObject as IDictionary<string, JToken>;
            if (settingsDictionary != null) 
                return GetDictionarySetting<T>(setting, settingsDictionary);

            var settingsDictionary2 = settingsObject as IDictionary<string, object>;
            if (settingsDictionary2 != null)
                return GetDictionarySetting<T>(setting, settingsDictionary2);

            return default(T);
        }

        private static bool IsDynamicSettingSet(Settings setting, dynamic settingsObject)
        {
            if (settingsObject == null) return false;

            var settingName = GetSettingNameFromSetting(setting);

            var settingsDictionary = settingsObject as IDictionary<string, JToken>;
            if (settingsDictionary != null) 
                return settingsDictionary.Keys.Any(k => IsMatch(k, settingName));

            var settingsDictionary2 = settingsObject as IDictionary<string, object>;
            if (settingsDictionary2 != null) 
                return settingsDictionary2.Keys.Any(k => IsMatch(k, settingName));

            return false;
        }

        private static bool IsDynamicSettingSet(string setting, dynamic settingsObject)
        {
            if (settingsObject == null) return false;

            var settingsDictionary = settingsObject as IDictionary<string, JToken>;
            if (settingsDictionary != null) 
                return settingsDictionary.Keys.Any(k => IsMatch(k, setting));

            var settingsDictionary2 = settingsObject as IDictionary<string, object>;
            if (settingsDictionary2 != null) 
                return settingsDictionary2.Keys.Any(k => IsMatch(k, setting));

            return false;
        }

        private static T GetDictionarySetting<T>(Settings setting, IDictionary<string, JToken> dictionary)
        {
            if (dictionary == null) return default(T);

            var settingName = GetSettingNameFromSetting(setting);
            var settingKey = dictionary.Keys.FirstOrDefault(k => IsMatch(k, settingName));
            if (!string.IsNullOrEmpty(settingKey))
                return dictionary[settingKey].Value<T>();
            return default(T);
        }

        private static T GetDictionarySetting<T>(Settings setting, IDictionary<string, object> dictionary)
        {
            if (dictionary == null) return default(T);

            var settingName = GetSettingNameFromSetting(setting);
            var settingKey = dictionary.Keys.FirstOrDefault(k => IsMatch(k, settingName));
            if (!string.IsNullOrEmpty(settingKey))
                return (T)dictionary[settingKey];
            return default(T);
        }

        private static T GetDictionarySetting<T>(string setting, IDictionary<string, JToken> dictionary)
        {
            if (dictionary == null) return default(T);

            var settingKey = dictionary.Keys.FirstOrDefault(k => IsMatch(k, setting));
            if (!string.IsNullOrEmpty(settingKey))
                return dictionary[settingKey].Value<T>();
            return default(T);
        }

        private static T GetDictionarySetting<T>(string setting, IDictionary<string, object> dictionary)
        {
            if (dictionary == null) return default(T);

            var settingKey = dictionary.Keys.FirstOrDefault(k => IsMatch(k, setting));
            if (!string.IsNullOrEmpty(settingKey))
                return (T)dictionary[settingKey];
            return default(T);
        }

        private static dynamic GlobalConfiguration;

        public static Settings GetSettingFromSettingName(string settingName)
        {
            var settingEnumType = typeof(Settings);
            var settingEnumNames = Enum.GetNames(typeof(Settings));

            foreach (var settingEnumName in settingEnumNames)
                if (IsMatch(settingEnumName, settingName))
                    return (Settings)Enum.Parse(typeof(Settings), settingEnumName);

            return Settings.Unknown;
        }

        public static string GetSettingNameFromSetting(Settings setting) => setting.ToString().Trim().ToLowerInvariant();

        public static bool IsMatch(string settingName, string compareTo)
        {
            if (string.IsNullOrEmpty(settingName)) return false;
            if (string.IsNullOrEmpty(compareTo)) return false;
            return GetNormalizeName(settingName) == GetNormalizeName(compareTo);
        }

        public static string GetNormalizeName(string settingName)
        {
            if (string.IsNullOrEmpty(settingName)) return string.Empty;
            return settingName.ToLowerInvariant();
        }

        public static string GetGlobalSetting(string setting)
        {
            var dynamicGlobalConfiguraiton = (dynamic)GlobalConfiguration;
            return dynamicGlobalConfiguraiton[setting];
        }
    }
}