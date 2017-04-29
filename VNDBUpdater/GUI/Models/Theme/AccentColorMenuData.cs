using MahApps.Metro;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VNDBUpdater.GUI.CustomClasses.Commands;
using VNDBUpdater.Services.User;

namespace VNDBUpdater.GUI.Models.Theme
{
    public class AccentColorMenuData
    {
        private IUserService _UserService;

        public AccentColorMenuData(IUserService UserService)
        {
            _UserService = UserService;
        }

        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }

        private ICommand changeAccentCommand;

        public ICommand ChangeAccentCommand
        {
            get
            {
                return changeAccentCommand 
                    ?? (changeAccentCommand = new RelayCommand(x => DoChangeTheme(x)));
            }
        }

        protected virtual void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var accent = ThemeManager.GetAccent(Name);

            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);

            _UserService.Get().GUI.SelectedAppTheme = theme.Item1.Name;
            _UserService.Get().GUI.SelectedAppAccent = accent.Name;
        }
    }
}
