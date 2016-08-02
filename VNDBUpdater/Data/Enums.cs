
namespace VNDBUpdater.Data
{
    public enum TagCategory : byte
    {
        All = 0,
        cont,
        ero,
        tech
    };

    public enum VisualNovelCatergory : byte
    {
        Unknown = 0,
        Playing,
        Finished,
        Stalled,
        Dropped,
        Wish
    };

    public enum ErrorResponse
    {
        Throttled = 0,
        Unknown
    };

    public enum VNDBCommunicationStatus
    {
        LoggedIn = 0,
        NotLoggedIn,
        Error,
        Throttled,
    };
}
