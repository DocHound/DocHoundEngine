namespace DocHound.Interfaces
{
    public interface ISettingsProvider
    {
        T GetSetting<T>(Settings setting);
        bool IsSettingSpecified(Settings setting);
        void OverrideSetting<T>(Settings setting, T value);
    }
}