using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace VNDBUpdater.Services.Logger
{
    public class LoggerService : ILoggerService
    {
        private readonly string _LogPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Log.txt";
        private readonly int _MaxLogSize = 1000000;

        private static object _Lock = new object();

        public void Log(Exception ex, [CallerMemberName]string memberName = "")
        {
            Write("Caller: " + memberName + Environment.NewLine + GetException(ex));
        }

        public void Log(string message, [CallerMemberName]string memberName = "")
        {
            Write("Caller: " + memberName + Environment.NewLine + message);
        }

        private string GetException(Exception ex, string msg = "")
        {
            msg += new StringBuilder()
                .AppendLine(ex.GetType().Name)
                .AppendLine()
                .AppendLine(ex.Message)
                .AppendLine()
                .AppendLine(ex.StackTrace != null ? ex.StackTrace : string.Empty)
                .ToString();

            return ex.InnerException != null
                ? GetException(ex.InnerException, msg)
                : msg;
        }

        private void Write(string line)
        {
            lock (_Lock)
            {
                CheckLogSize();

                if (!File.Exists(_LogPath))
                {
                    using (var fs = File.Create(_LogPath)) { };
                }

                using (var sw = new StreamWriter(new FileStream(_LogPath, FileMode.Append, FileAccess.Write, FileShare.None)))
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ": " + line + Environment.NewLine);

                    sw.Flush();
                    sw.Close();
                }
            }
        }

        private void CheckLogSize()
        {
            try
            {
                if (File.Exists(_LogPath))
                {
                    if (new FileInfo(_LogPath).Length > _MaxLogSize)
                    {
                        File.Delete(_LogPath);
                    }
                }
            }
            catch (Exception ex)
            {
                // Ignore.
            }
        }
    }
}
