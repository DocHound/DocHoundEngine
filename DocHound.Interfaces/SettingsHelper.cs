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
            var type = typeof(SettingsEnum);
            var memInfo = type.GetMembers().FirstOrDefault(m => SettingsHelper.IsMatch(setting, m.Name));
            if (memInfo != null)
            {
                var attributes = memInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false);
                if (attributes.Length > 0)
                    return (T)((DefaultValueAttribute)attributes[0]).Value;
            }

            return default(T);
        }

        public static T GetSetting<T>(SettingsEnum setting, dynamic repositorySettings = null, dynamic topicSettings = null, dynamic requestRootSettings = null)
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
            var type = typeof(SettingsEnum);
            var memInfo = type.GetMember(setting.ToString());
            if (memInfo.Length > 0)
            {
                var attributes = memInfo[0].GetCustomAttributes(typeof(DefaultValueAttribute), false);
                if (attributes.Length > 0)
                    return (T)((DefaultValueAttribute)attributes[0]).Value;
            }

            return default(T);
        }

        public static bool IsSettingSet(SettingsEnum setting, dynamic repositorySettings = null, dynamic topicSettings = null)
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

        private static T GetDynamicSetting<T>(SettingsEnum setting, dynamic settingsObject)
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

        private static bool IsDynamicSettingSet(SettingsEnum setting, dynamic settingsObject)
        {
            if (settingsObject == null) return false;

            var settingName = GetSettingNameFromSetting(setting);

            var settingsDictionary = settingsObject as IDictionary<string, JToken>;
            if (settingsDictionary != null)
                if (settingsDictionary.Keys.Any(k => IsMatch(k, settingName))) return true;

            var settingsDictionary2 = settingsObject as IDictionary<string, object>;
            if (settingsDictionary2 != null)
                if (settingsDictionary2.Keys.Any(k => IsMatch(k, settingName))) return true;

            var alternateSettingNames = GetAllternateSettingNamesFromSetting(setting);
            foreach (var alternateSettingName in alternateSettingNames)
            {
                var settingsDictionary3 = settingsObject as IDictionary<string, JToken>;
                if (settingsDictionary3 != null)
                    if (settingsDictionary3.Keys.Any(k => IsMatch(k, alternateSettingName))) return true;

                var settingsDictionary4 = settingsObject as IDictionary<string, object>;
                if (settingsDictionary4 != null)
                    if (settingsDictionary4.Keys.Any(k => IsMatch(k, alternateSettingName))) return true;
            }

            return false;
        }

        private static bool IsDynamicSettingSet(string setting, dynamic settingsObject)
        {
            if (settingsObject == null) return false;

            var settingsDictionary = settingsObject as IDictionary<string, JToken>;
            if (settingsDictionary != null)
                if (settingsDictionary.Keys.Any(k => IsMatch(k, setting))) return true;

            var settingsDictionary2 = settingsObject as IDictionary<string, object>;
            if (settingsDictionary2 != null)
                if (settingsDictionary2.Keys.Any(k => IsMatch(k, setting))) return true;

            var settingStrong = GetSettingFromSettingName(setting);
            var alternateSettingNames = GetAllternateSettingNamesFromSetting(settingStrong);
            foreach (var alternateSettingName in alternateSettingNames)
            {
                var settingsDictionary3 = settingsObject as IDictionary<string, JToken>;
                if (settingsDictionary3 != null)
                    if (settingsDictionary3.Keys.Any(k => IsMatch(k, alternateSettingName))) return true;

                var settingsDictionary4 = settingsObject as IDictionary<string, object>;
                if (settingsDictionary4 != null)
                    if (settingsDictionary4.Keys.Any(k => IsMatch(k, alternateSettingName))) return true;
            }

            return false;
        }

        private static T GetDictionarySetting<T>(SettingsEnum setting, IDictionary<string, JToken> dictionary)
        {
            if (dictionary == null) return default(T);

            var settingName = GetSettingNameFromSetting(setting);
            var settingKey = dictionary.Keys.FirstOrDefault(k => IsMatch(k, settingName));
            if (!string.IsNullOrEmpty(settingKey))
                return dictionary[settingKey].Value<T>();

            var alternateSettingNames = GetAllternateSettingNamesFromSetting(setting);
            foreach (var alternateSettingName in alternateSettingNames)
            {
                settingKey = dictionary.Keys.FirstOrDefault(k => IsMatch(k, alternateSettingName));
                if (!string.IsNullOrEmpty(settingKey))
                    return dictionary[settingKey].Value<T>();
            }

            return default(T);
        }

        private static T GetDictionarySetting<T>(SettingsEnum setting, IDictionary<string, object> dictionary)
        {
            if (dictionary == null) return default(T);

            var settingName = GetSettingNameFromSetting(setting);
            var settingKey = dictionary.Keys.FirstOrDefault(k => IsMatch(k, settingName));
            if (!string.IsNullOrEmpty(settingKey))
                return (T)dictionary[settingKey];

            var alternateSettingNames = GetAllternateSettingNamesFromSetting(setting);
            foreach (var alternateSettingName in alternateSettingNames)
            {
                settingKey = dictionary.Keys.FirstOrDefault(k => IsMatch(k, alternateSettingName));
                if (!string.IsNullOrEmpty(settingKey))
                    return (T)dictionary[settingKey];
            }
            return default(T);
        }

        private static T GetDictionarySetting<T>(string setting, IDictionary<string, JToken> dictionary)
        {
            if (dictionary == null) return default(T);

            var settingKey = dictionary.Keys.FirstOrDefault(k => IsMatch(k, setting));
            if (!string.IsNullOrEmpty(settingKey))
                return dictionary[settingKey].Value<T>();

            var strongSetting = GetSettingFromSettingName(setting);
            var alternateSettingNames = GetAllternateSettingNamesFromSetting(strongSetting);
            foreach (var alternateSettingName in alternateSettingNames)
            {
                settingKey = dictionary.Keys.FirstOrDefault(k => IsMatch(k, alternateSettingName));
                if (!string.IsNullOrEmpty(settingKey))
                    return dictionary[settingKey].Value<T>();
            }
            return default(T);
        }

        private static T GetDictionarySetting<T>(string setting, IDictionary<string, object> dictionary)
        {
            if (dictionary == null) return default(T);

            var settingKey = dictionary.Keys.FirstOrDefault(k => IsMatch(k, setting));
            if (!string.IsNullOrEmpty(settingKey))
                return (T)dictionary[settingKey];

            var strongSetting = GetSettingFromSettingName(setting);
            var alternateSettingNames = GetAllternateSettingNamesFromSetting(strongSetting);
            foreach (var alternateSettingName in alternateSettingNames)
            {
                settingKey = dictionary.Keys.FirstOrDefault(k => IsMatch(k, alternateSettingName));
                if (!string.IsNullOrEmpty(settingKey))
                    return (T)dictionary[settingKey];
            }
            return default(T);
        }

        private static dynamic GlobalConfiguration;

        public static SettingsEnum GetSettingFromSettingName(string settingName)
        {
            var settingEnumType = typeof(SettingsEnum);
            var settingEnumNames = Enum.GetNames(typeof(SettingsEnum));

            foreach (var settingEnumName in settingEnumNames)
                if (IsMatch(settingEnumName, settingName))
                    return (SettingsEnum)Enum.Parse(typeof(SettingsEnum), settingEnumName);

            return SettingsEnum.Unknown;
        }

        public static string GetSettingNameFromSetting(SettingsEnum setting) => setting.ToString().Trim().ToLowerInvariant();
        public static List<string> GetAllternateSettingNamesFromSetting(SettingsEnum setting)
        {
            var names = new List<string>();

            var type = typeof(SettingsEnum);
            var memInfo = type.GetMembers().FirstOrDefault(m => SettingsHelper.IsMatch(setting.ToString(), m.Name));
            if (memInfo != null)
            {
                var attributes = memInfo.GetCustomAttributes(typeof(AlternateNameAttribute), false);
                foreach (var attribute in attributes.OfType<AlternateNameAttribute>())
                    names.Add(attribute.Name);
            }

            return names;
        }

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