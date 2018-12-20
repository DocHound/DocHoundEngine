namespace DocHound.Interfaces
{
    public interface ISettingsProvider
    {
        T GetSetting<T>(SettingsEnum setting);
        bool IsSettingSpecified(SettingsEnum setting);
        void OverrideSetting<T>(SettingsEnum setting, T value);
    }
}