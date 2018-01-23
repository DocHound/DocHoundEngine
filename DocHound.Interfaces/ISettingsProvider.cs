namespace DocHound.Interfaces
{
    public interface ISettingsProvider
    {
        T GetSetting<T>(Settings setting);
    }
}