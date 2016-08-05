using System;
using System.Diagnostics;
using System.Reflection;
using VNUpdater.Helper;

namespace VNDBUpdater.ViewModels
{
    class AboutViewModel : ViewModelBase
    {
        public AboutViewModel()
        {
            _Commands.AddCommand("OpenGitHubLink", ExecuteOpenGitHubLink);
        }

        public string Version
        {
            get { return "VNDBUpdater Version: " + VersionHelper.CurrentVersion; }
        }

        public string Copyright
        {
            get { return ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false)).Copyright; }
        }

        public string LatestRelease
        {
            get { return "Latest Release: " + VersionHelper.NewestVersion; }
        }

        public void ExecuteOpenGitHubLink(object parameter)
        {
            Process.Start("https://github.com/Onkelsam/VNDBUpdater/releases");
        }
    }
}
