using System.Collections.Generic;
using VNDBUpdater.Communication.Database.Entities;

namespace VNDBUpdater.GUI.Models.VisualNovel
{
    public class FileIndexerSettingsModel
    {
        private readonly List<string> ExcludedExeFileNames = new List<string>() { "uninstall", "エンジン設定", "unins000", "supporttools",
            "startuptool", "filechk", "directxcheck", "setup", "uninst64", "uninst", "cmvscheck64", "cmvsconfig64", "uninst32", "cmvscheck32", "cmvsconfig32", "gameupdate64", "filechecker", "config", "uninstaller",
            "inst", "patch", "_uninst", "setting", "authtool", "ファイル破損チェックツール", "launcher" };

        public FileIndexerSettingsModel()
        {
            Folders = new List<string>();
            ExcludedFolders = new List<string>();
            ExcludedExes = new List<string>(ExcludedExeFileNames);

            MinFolderLength = 3;
            MaxDeviation = 5;
        }

        public FileIndexerSettingsModel(FileIndexerSettingsEntity entity)
            : this()
        {
            Folders = entity.Folders;
            ExcludedFolders = entity.ExcludedFolders;
            ExcludedExes = entity.ExcludedExes;

            MinFolderLength = entity.MinFolderLength;
            MaxDeviation = entity.MaxDeviation;
        }

        public List<string> Folders
        {
            get;
            private set;
        }

        public List<string> ExcludedFolders
        {
            get;
            private set;
        }

        public List<string> ExcludedExes
        {
            get;
            private set;
        }

        public int MinFolderLength
        {
            get;
            set;
        }

        public int MaxDeviation
        {
            get;
            set;
        }

        public FileIndexerSettingsModel SetDefault()
        {
            ExcludedFolders = new List<string>();
            ExcludedExes = new List<string>(ExcludedExeFileNames);

            MinFolderLength = 3;
            MaxDeviation = 5;

            return this;
        }
    }
}
