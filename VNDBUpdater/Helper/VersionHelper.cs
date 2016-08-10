using Octokit;
using System;
using System.Reflection;

namespace VNDBUpdater.Helper
{
    public static class VersionHelper
    {
        public static string CurrentVersion
        {
            get {  return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public static string NewestVersion
        {
            get
            {
                try
                {
                    var releases = new GitHubClient(new ProductHeaderValue("VNDBUpdater")).Repository.Release.GetAll("Onkelsam", "VNDBUpdater").Result;
                    return releases[0].TagName;
                }
                catch (Exception ex)
                {
                    EventLogger.LogError(nameof(VersionHelper), ex);
                    return string.Empty;
                }                
            }
        }

        public static bool NewVersionAvailable()
        {
            return CurrentVersion != NewestVersion;
        }
    }
}
