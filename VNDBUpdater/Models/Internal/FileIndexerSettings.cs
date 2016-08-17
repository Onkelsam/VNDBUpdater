using System.Collections.Generic;
using VNDBUpdater.Data;

namespace VNDBUpdater.Models.Internal
{
    public class FileIndexerSettings
    {
        public List<string> Folders { get; set; }
        public List<string> ExcludedFolders { get; set; }
        public List<string> ExcludedExes { get; set; }
        public int MinFolderLength { get; set; }
        public int MaxDeviation { get; set; }

        public FileIndexerSettings()
        {
            Folders = new List<string>();
            ExcludedFolders = new List<string>();
            ExcludedExes = new List<string>();                        
        }

        public FileIndexerSettings SetDefault()
        {
            ExcludedFolders = new List<string>();
            ExcludedExes = new List<string>();

            ExcludedExes.AddRange(Constants.ExcludedExeFileNames);
            MinFolderLength = 3;
            MaxDeviation = Constants.MaxDistanceBetweenStringsForIndexer;

            return this;
        }
    }
}
