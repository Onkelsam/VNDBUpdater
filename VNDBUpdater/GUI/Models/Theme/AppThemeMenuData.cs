using MahApps.Metro;
using System.Windows;
using VNDBUpdater.Services.User;

namespace VNDBUpdater.GUI.Models.Theme
{
    public class AppThemeMenuData : AccentColorMenuData
    {
        public AppThemeMenuData(IUserService UserService)
            : base(UserService) { }        

        protected override void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var appTheme = ThemeManager.GetAppTheme(Name);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);
        }
    }
}
