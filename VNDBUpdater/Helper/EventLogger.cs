using System;
using System.Diagnostics;
using System.Globalization;

namespace VNDBUpdater.Helper
{
    public static class EventLogger
    {
        public static void LogInformation(string location, string message)
        {
            Trace.TraceInformation(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + " Location : " + location + ", Message: " + message);
        }

        public static void LogError(string location, Exception error)
        {
            Trace.TraceError(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + " : " + " Error caught in: " + location + Environment.NewLine + " Error: " + Environment.NewLine + error.Message + Environment.NewLine + error.GetType().Name + Environment.NewLine + error.StackTrace);
        }
    }
}
