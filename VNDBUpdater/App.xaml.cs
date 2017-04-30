using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.Unity;
using System;
using System.Windows;
using VNDBUpdater.BackgroundTasks.Factory;
using VNDBUpdater.BackgroundTasks.Interfaces;
using VNDBUpdater.Communication.Database;
using VNDBUpdater.Communication.Database.Caching;
using VNDBUpdater.Communication.Database.Interfaces;
using VNDBUpdater.Communication.VNDB;
using VNDBUpdater.Communication.VNDB.Interfaces;
using VNDBUpdater.GUI.ViewModels;
using VNDBUpdater.GUI.ViewModels.Interfaces;
using VNDBUpdater.GUI.ViewModels.MainView;
using VNDBUpdater.Services.Dialogs;
using VNDBUpdater.Services.Filters;
using VNDBUpdater.Services.Launch;
using VNDBUpdater.Services.LaunchMonitor;
using VNDBUpdater.Services.Logger;
using VNDBUpdater.Services.Login;
using VNDBUpdater.Services.Status;
using VNDBUpdater.Services.Tags;
using VNDBUpdater.Services.Traits;
using VNDBUpdater.Services.User;
using VNDBUpdater.Services.Version;
using VNDBUpdater.Services.VN;
using VNDBUpdater.Services.WindowHandler;

namespace VNDBUpdater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IUnityContainer Container = new UnityContainer();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Initialize();

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var monitor = Container.Resolve<ILaunchMonitorService>();

            monitor.StartMonitoring();

            var splash = Container.Resolve<GUI.Views.SplashScreen>();

            splash.Closed += OnSplashScreenClosed;
            splash.ShowDialog();
        }

        private void Initialize()
        {
            Container.RegisterInstance<IUnityContainer>(Container);

            Container.RegisterType<ILoggerService, LoggerService>(new ContainerControlledLifetimeManager());

            // API and Database and Caching Initialization.
            Container.RegisterType<IRedis, RedisBase>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IVNDB, VNDBBase>(new ContainerControlledLifetimeManager());

            Container.RegisterType<IVNRepository, VNRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IUserRepository, UserRepository>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IFilterRepository, FilterRepository>(new ContainerControlledLifetimeManager());

            Container.RegisterType<IVNDBGetter, VNDBGetter>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IVNDBSetter, VNDBSetter>(new ContainerControlledLifetimeManager());

            Container.RegisterType<ICache, CachingLayer>(new ContainerControlledLifetimeManager());


            // Service Initialization.
            Container.RegisterType<ITagService, TagService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ITraitService, TraitService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IVNService, VNService>();
            Container.RegisterType<IFilterService, FilterService>();
            Container.RegisterType<IUserService, UserService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IStatusService, StatusService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IDialogService, DialogService>();
            Container.RegisterType<IVersionService, VersionService>();
            Container.RegisterType<ILoginService, LoginService>();
            Container.RegisterType<ILaunchService, LaunchService>();
            Container.RegisterType<IWindowHandlerService, WindowHandlerService>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ILaunchMonitorService, LaunchMonitorService>(new ContainerControlledLifetimeManager());


            // Main View Initialization.
            Container.RegisterType<IMainWindowModel, MainViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IVisualNovelsGridWindowModel, VNDatagridViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ICharacterTabWindowModel, CharacterTabViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IMenuBarWindowModel, MenuBarViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IScreenshotTabWindowModel, ScreenshotViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<ITagTabWIndowModel, TagTabViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IVisualNovelInfoWindowModel, VNInfoViewModel>(new ContainerControlledLifetimeManager());


            // Sub Views Initialization.
            Container.RegisterType<ISplashScreenWindowModel, SplashScreenViewModel>();
            Container.RegisterType<IOptionsWindowModel, OptionsViewModel>();
            Container.RegisterType<IFileIndexerWindowModel, FileIndexerViewModel>();
            Container.RegisterType<ICreateFilterWindowModel, CreateFilterViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAddVisualNovelsWindowModel, AddVisualNovelsViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IAboutViewModel, AboutViewModel>();

            // Other.
            Container.RegisterType<ITaskFactory, BackgroundTaskFactory>();
            Container.RegisterInstance(DialogCoordinator.Instance);
        }

        private void OnSplashScreenClosed(object sender, EventArgs e)
        {
            if ((sender as Window).DialogResult == true)
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose;

                var user = Container.Resolve<IUserService>();

                ThemeManager.ChangeAppStyle(Current, ThemeManager.GetAccent(user.Get().GUI.SelectedAppAccent), ThemeManager.GetAppTheme(user.Get().GUI.SelectedAppTheme));

                var mainWindow = Container.Resolve<GUI.Views.MainWindow>();

                mainWindow.ShowDialog();
            }
            else
            {
                Current.Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            var user = Container.Resolve<IUserRepository>();

            user.Add(user.Get(0));

            var redis = Container.Resolve<IRedis>();

            redis.Save();
            redis.Dispose();

            var vndb = Container.Resolve<IVNDB>();

            vndb.Dispose();

            var monitor = Container.Resolve<ILaunchMonitorService>();

            monitor.Dispose();
        }

        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhandledException);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception)args.ExceptionObject;

            var logger = Container.Resolve<LoggerService>();

            logger.Log(e);

            if (MessageBox.Show("Application ran into error... Check the 'eventlog.txt' for further information. Continue regardless?", "ERROR", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                Current.Shutdown();
            }
        }
    }
}
