namespace VNDBUpdater.Services.Version
{
    public interface IVersionService
    {
        void OpenLinkToNewestVersion();

        string CurrentVersion { get; }
        bool NewVersionAvailable { get; }
        string NewVersion { get; }
        string Copyright { get; }
    }
}
