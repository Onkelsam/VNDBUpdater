using System.Collections.Generic;
using VNDBUpdater.Communication.Database.Entities;
using VNDBUpdater.Data;

namespace VNDBUpdater.GUI.Models.VisualNovel
{
    public class FileIndexerSettingsModel
    {
        public FileIndexerSettingsModel()
        {
            Folders = new List<string>();
            ExcludedFolders = new List<string>();
            ExcludedExes = new List<string>();

            ExcludedExes.AddRange(Constants.ExcludedExeFileNames);
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
            set;
        }

        public List<string> ExcludedFolders
        {
            get;
            set;
        }

        public List<string> ExcludedExes
        {
            get;
            set;
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
            ExcludedExes = new List<string>();

            ExcludedExes.AddRange(Constants.ExcludedExeFileNames);
            MinFolderLength = 3;
            MaxDeviation = 5;

            return this;
        }
    }
}
