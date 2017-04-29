using System.Windows.Input;
using VNDBUpdater.GUI.CustomClasses.Commands;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.Services.Version;

namespace VNDBUpdater.GUI.ViewModels
{
    class AboutViewModel : ViewModelBase, IAboutViewModel
    {
        private IVersionService _VersionService;

        public AboutViewModel(IVersionService VersionService)
            : base()
        {
            _VersionService = VersionService;
        }

        public string Version
        {
            get { return "VNDBUpdater Version: " + _VersionService.CurrentVersion; }
        }

        public string Copyright
        {
            get { return _VersionService.Copyright; }
        }

        public string LatestRelease
        {
            get { return "Latest Release: " + _VersionService.NewVersion; }
        }

        private ICommand _OpenGitHubLink;

        public ICommand OpenGitHubLink
        {
            get
            {
                return _OpenGitHubLink ?? 
                    (_OpenGitHubLink = new RelayCommand(x => _VersionService.OpenLinkToNewestVersion()));
            }
        }
    }
}
