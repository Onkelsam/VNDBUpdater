using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using VNDBUpdater.Data;

namespace VNDBUpdater.Helper
{
    public static class FileHelper
    {
        public static string GetExePath()
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Exe File |*.exe",
                FilterIndex = 1,
                Multiselect = false,
                DefaultExt = "exe"
            };

            if (fileDialog.ShowDialog() == true)
            {
                if (fileDialog.FileName.EndsWith(".exe"))
                    return fileDialog.FileName;
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }

        public static string GetFolderPath()
        {
            var fileDialog = new FolderBrowserDialog()
            {
                Description = "Select folder"
            };

            fileDialog.ShowDialog();

            return fileDialog.SelectedPath;
        }

        public static void Decompress(FileInfo fileToDecompress)
        {
            using (var originalFile = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                using (var decompressedFileStream = File.Create(newFileName))
                {
                    using (var decompressionStream = new GZipStream(originalFile, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                    }
                }
            }
        }

        public static void DeleteFile(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);
        }

        public static void DeleteTooLargeFile(string filename, long maxfilesize)
        {
            if (File.Exists(filename))
                if (new FileInfo(filename).Length > maxfilesize)
                    DeleteFile(filename);
        }

        public static void BackupDatabase()
        {
            if (File.Exists(Constants.DirectoryPath + Constants.PathToDatabase + Constants.DatabaseName))
                File.Copy(Constants.DirectoryPath + Constants.PathToDatabase + Constants.DatabaseName, Constants.DirectoryPath + Constants.PathToDatabase + Constants.BackupDatabaseName, true);
        }

        public static void CreateFile(string path)
        {
            if (!File.Exists(path))
            {
                using (var fs = File.Create(path)) { } ;
            }
        }
    }
}
