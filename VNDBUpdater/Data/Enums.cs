﻿
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
    };

    public enum ErrorResponse : byte
    {
        Throttled = 0,
        AuthenticationFailed,
        Unknown
    };

    public enum VNDBCommunicationStatus : byte
    {
        LoggedIn = 0,
        NotLoggedIn,
        Error,
        Throttled,
    };

    public enum SpoilerSetting : byte
    {
        Hide = 0,
        ShowMinor,
        ShowAll,        
    };

    public enum SpoilerLevel : byte
    {
        none = 0,
        minor,
        major
    };

    public enum ControlVisibility : byte
    {
        Collapsed = 0,
        Hidden,
        Visible
    };

    public enum SubTabs : byte
    {
        Tags = 0,
        Screenshots,
        Characters
    }
}
