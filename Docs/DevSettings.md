# Development: Using the Settings/Configuration System

The DocHound engine provides a standardized way to get configuration options (a.k.a. "settings"). Options can be set globally (currently in app settings, but this will change) as well as through a settings object on the TOC root and also on individual topis though the same settings. (See also: [TOC File Structure](TOC-File-Structure)

## The SettingsHelper Object

Within the application, settings can be accessed like so:

```cs
var x = SettingsHelper.GetSetting<string>(Settings.RepositoryType, RepositorySettings, CurrentTopicSettings);
```

This uses a pre-defined Settings enum (a string overload is also available) to retrieve settings from potentially three different sources: 1) current topic settings, 2) settings for all topics in the TOC, and 3) global app settings. (Settings can be overridden in this order).

The second and third parameters are optional. Therefore, the following all work:

```cs
var x = SettingsHelper.GetSetting<string>(Settings.RepositoryType, RepositorySettings, CurrentTopicSettings);
var x = SettingsHelper.GetSetting<string>(Settings.RepositoryType, RepositorySettings);
var x = SettingsHelper.GetSetting<string>(Settings.RepositoryType);
```

Note however, that in most cases, you want to pass all paremters.

## TopicViewModel.GetSetting<T>()

For everything done within the TopicViewModel class, it is recommended that the GetSettings<T>() convenience method is used. This is equivalent to the examples above that use all three parameters. The convenience method makes sure that appropriate TOC and Topic settings are considered.

```cs
var x = GetSetting<string>(Settings.RepositoryType);
```

> Note: TopicViewModel.GetSetting<T>() also uses per-request caching for performance. This means that no settings can change while a single topic is rendered. This is probably a safe assumption for most scenarios.
