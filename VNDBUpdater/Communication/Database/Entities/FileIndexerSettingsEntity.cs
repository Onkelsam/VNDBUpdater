using System.Collections.Generic;
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

        public int MinFolderLength { get; private set; }
        public int MaxDeviation { get; private set; }
        public List<string> Folders { get; private set; }
        public List<string> ExcludedFolders { get; private set; }
        public List<string> ExcludedExes { get; private set; }
    }
}
