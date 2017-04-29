using Octokit;
using System;
using System.Diagnostics;
using System.Reflection;
using VNDBUpdater.Services.Logger;

namespace VNDBUpdater.Services.Version
{
    public class VersionService : IVersionService
    {
        private ILoggerService _LoggerService;
        private const string _GitHubReleaseLink = "https://github.com/Onkelsam/VNDBUpdater/releases";

        public VersionService(ILoggerService LoggerService)
        {
            _LoggerService = LoggerService;
        }

        public string CurrentVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string NewVersion
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
                    _LoggerService.Log(ex);

                    return string.Empty;
                }
            }
        }

        public bool NewVersionAvailable
        {
            get
            {
                return CurrentVersion != NewVersion;
            }
        }
        public string Copyright
        {
            get
            {
                return ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false)).Copyright;
            }
        }

        public void OpenLinkToNewestVersion()
        {
            Process.Start(_GitHubReleaseLink);
        }
    }
}
