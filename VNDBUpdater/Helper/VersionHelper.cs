using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
                var releases = new GitHubClient(new ProductHeaderValue("VNDBUpdater")).Repository.Release.GetAll("Onkelsam", "VNDBUpdater").Result;
                return releases[0].TagName;
            }
        }

        public static bool NewVersionAvailable()
        {
            return CurrentVersion != NewestVersion;
        }
    }
}
