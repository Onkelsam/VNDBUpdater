using System;
using System.Reflection;

namespace VNDBUpdater.ViewModels
{
    class AboutViewModel : ViewModelBase
    {
        public string Version
        {
            get { return ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title + " Version: " + Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public string Copyright
        {
            get { return ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false)).Copyright; }
        }
    }
}
