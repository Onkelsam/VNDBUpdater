using System.Collections.Generic;

namespace VNDBUpdater.Data
{
    public static class Constants
    {
        public static readonly List<string> ExcludedExeFileNames = new List<string>() { "uninstall", "エンジン設定", "unins000", "supporttools",
            "startuptool", "filechk", "directxcheck", "setup", "uninst64", "uninst", "cmvscheck64", "cmvsconfig64", "uninst32", "cmvscheck32", "cmvsconfig32", "gameupdate64", "filechecker", "config", "uninstaller",
            "inst", "patch", "_uninst", "setting", "authtool", "ファイル破損チェックツール", "launcher" };
    }
}
