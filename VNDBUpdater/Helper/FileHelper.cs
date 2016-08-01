using System;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using VNDBUpdater.Communication.Database;
using VNDBUpdater.Models;

namespace VNDBUpdater.Helper
{
    public static class FileHelper
    {
        private static string[] _Folders;
        private const int MAXDISTANCEBETWEENSTRING = 3;

        private static string[] Folders
        {
            get
            {
                if (_Folders == null)
                {
                    var InstallFolder = RedisCommunication.GetInstallFolder();

                    if (InstallFolder != null)
                    {
                        _Folders = Directory.GetDirectories(InstallFolder, "*.*", SearchOption.AllDirectories);
                        return _Folders;
                    }
                }

                return _Folders;
            }
        }

        public static void ResetFolders()
        {
            _Folders = null;
        }

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
                Description = "Select main folder where your Visual Novels are stored."
            };

            DialogResult result = fileDialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fileDialog.SelectedPath))
                return fileDialog.SelectedPath;
            else
                return RedisCommunication.GetInstallFolder();
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

        public static string SearchForVisualNovelExe(VisualNovel VN)
        {
            if (Folders != null)
            {
                foreach (var folder in Folders)
                {
                    if (ComputeLevenshteinDistance(new DirectoryInfo(folder).Name.ToLower().Trim(), VN.Basics.title.ToLower().Trim()) < MAXDISTANCEBETWEENSTRING)
                    {
                        foreach (var file in Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories))
                        {
                            if (Path.GetExtension(file) == ".exe")
                            {
                                if (file.ToLower().Contains("unins"))
                                    continue;

                                if (file.Contains("エンジン設定"))
                                    continue;

                                return file;
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Algorithm that compares two strings and returns how many characters are different.
        /// </summary>
        private static int ComputeLevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }
    }
}
