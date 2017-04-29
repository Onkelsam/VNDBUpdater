using System.ComponentModel;

namespace VNDBUpdater.Data
{
    public enum SpoilerSetting
    {
        [Description("Hide all")]
        Hide = 0,

        [Description("Show minor spoilers")]
        ShowMinor,

        [Description("Show all spoilers")]
        ShowAll,        
    };

    public enum SpoilerLevel
    {
        none = 0,
        minor,
        major
    };
}
