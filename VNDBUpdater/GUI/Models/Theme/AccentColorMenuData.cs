using MahApps.Metro;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VNDBUpdater.GUI.CustomClasses.Commands;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.Services.User;

namespace VNDBUpdater.GUI.Models.Theme
{
    public class AccentColorMenuData
    {
        protected UserModel _User;
        protected IUserService _UserService;

        public AccentColorMenuData(IUserService UserService)
        {
            _UserService = UserService;
            _UserService.OnUpdated += OnUserUpdated;

            Task.Factory.StartNew(async () => await Initialize());
        }

        private async Task Initialize()
        {
            OnUserUpdated(this, await _UserService.Get());
        }

        private void OnUserUpdated(object sender, UserModel User)
        {
            _User = User;
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

            _User.GUI.SelectedAppTheme = theme.Item1.Name;
            _User.GUI.SelectedAppAccent = accent.Name;

            _UserService.Update(_User);
        }
    }
}
