using System.IO;
using System.IO.Compression;
using System.Net;

namespace VNDBUpdater.Services
{
    public abstract class TagsAndTraitsBase
    {
        public virtual void Refresh(string downloadLink, string fileName)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(downloadLink, fileName);
                Decompress(new FileInfo(fileName));
            }

            File.Delete(fileName);
        }

        private void Decompress(FileInfo file)
        {
            using (var originalFile = file.OpenRead())
            {
                string currentFileName = file.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - file.Extension.Length);

                using (var decompressedFileStream = File.Create(newFileName))
                using (var decompressionStream = new GZipStream(originalFile, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(decompressedFileStream);
                }
            }
        }
    }
}
