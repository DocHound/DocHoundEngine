using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Microsoft.CSharp.RuntimeBinder;

namespace DocHound.Interfaces
{
    public class TocSettings
    {
        private readonly dynamic _settings;
        private readonly Type _settingsType;
        private readonly dynamic _settings2;
        private readonly Type _settings2Type;

        public TocSettings(dynamic settings, dynamic settings2)
        {
            _settings = settings;
            _settingsType = settings?.GetType();
            _settings2 = settings2;
            _settings2Type = settings2?.GetType();
        }

        public bool GetBooleanSetting(bool defaultValue, [CallerMemberName] string settingName = "")
        {
            if (settingName == null) return defaultValue;
            if (settingName.Length > 1)
            {
                // Turning it into camel case
                var firstCharacter = settingName.Substring(0, 1).ToLowerInvariant();
                settingName = firstCharacter + settingName.Substring(1);
            }


            // We give the second object (likely the one associated with the actual tree node) a try
            if (_settings2 != null)
            {
                try
                {
                    var value = BuildDynamicGetter(_settings2Type, settingName).Invoke(_settings2).ToString().ToLowerInvariant();
                    if (value == "true") return true;
                    if (value == "false") return false;
                }
                catch
                {
                }
            }

            // If we got this far, we are giving the first object (likely the one on the root of the TOC) a try
            if (_settings != null)
            {
                try
                {
                    var value = BuildDynamicGetter(_settingsType, settingName).Invoke(_settings).ToString().ToLowerInvariant();
                    if (value == "true") return true;
                    if (value == "false") return false;
                }
                catch
                {
                }
            }


            // Nothing so far, so we go with the default
            return defaultValue;
        }

        private static Func<object, object> BuildDynamicGetter(Type targetType, string propertyName)
        {
            var rootParam = Expression.Parameter(typeof(object));
            var propBinder = Binder.GetMember(CSharpBinderFlags.None, propertyName, targetType, new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var propGetExpression = Expression.Dynamic(propBinder, typeof(object), Expression.Convert(rootParam, targetType));
            var getPropExpression = Expression.Lambda<Func<object, object>>(propGetExpression, rootParam);
            return getPropExpression.Compile();
        }

        public bool UseAbbreviations => GetBooleanSetting(true);
        public bool UseAutoIdentifiers => GetBooleanSetting(true);
        public bool UseAutoLinks => GetBooleanSetting(true);
        public bool UseCitations => GetBooleanSetting(true);
        public bool UseCustomContainers => GetBooleanSetting(true);
        public bool UseDiagramsMermaid => GetBooleanSetting(false);
        public bool UseDiagramsNomnoml => GetBooleanSetting(false);
        public bool UseEmojiAndSmiley => GetBooleanSetting(true);
        public bool UseEmphasisExtras => GetBooleanSetting(true);
        public bool UseFigures => GetBooleanSetting(true);
        public bool UseFootnotes => GetBooleanSetting(true);
        public bool UseGenericAttributes => GetBooleanSetting(true);
        public bool UseGridTables => GetBooleanSetting(true);
        public bool UseListExtras => GetBooleanSetting(true);
        public bool UseMathematics => GetBooleanSetting(false);
        public bool UseMediaLinks => GetBooleanSetting(true);
        public bool UsePipeTables => GetBooleanSetting(true);
        public bool UsePragmaLines => GetBooleanSetting(false);
        public bool UseSmartyPants => GetBooleanSetting(true);
        public bool UseSyntaxHighlighting => GetBooleanSetting(true);
        public bool UseTaskLists => GetBooleanSetting(true);
        public bool UseYamlFrontMatter => GetBooleanSetting(true);

    }
}
