using System;
using System.Diagnostics;
using System.Reflection;

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
            get { return ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title + " Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public string Copyright
        {
            get { return ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false)).Copyright; }
        }

        public void ExecuteOpenGitHubLink(object parameter)
        {
            Process.Start("https://github.com/Onkelsam/VNDBUpdater/releases");
        }
    }
}
