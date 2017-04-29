using System.Collections.Generic;
using VNDBUpdater.Data;
using VNDBUpdater.GUI.Models.VisualNovel;

namespace VNDBUpdater.Communication.Database.Entities
{
    public class FileIndexerSettingsEntity
    {
        public FileIndexerSettingsEntity()
        {
            Folders = new List<string>();
            ExcludedFolders = new List<string>();
            ExcludedExes = new List<string>();

            ExcludedExes.AddRange(Constants.ExcludedExeFileNames);
            MinFolderLength = 3;
            MaxDeviation = 5;
        }

        public FileIndexerSettingsEntity(FileIndexerSettingsModel model)
        {
            Folders = model.Folders;
            ExcludedFolders = model.ExcludedFolders;
            ExcludedExes = model.ExcludedExes;
            MinFolderLength = model.MinFolderLength;
            MaxDeviation = model.MaxDeviation;
        }

        public int MinFolderLength { get; set; }
        public int MaxDeviation { get; set; }
        public List<string> Folders { get; set; }
        public List<string> ExcludedFolders { get; set; }
        public List<string> ExcludedExes { get; set; }
    }
}
