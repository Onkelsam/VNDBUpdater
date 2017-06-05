using Hardcodet.Wpf.TaskbarNotification;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using VNDBUpdater.GUI.Models.VisualNovel;
using VNDBUpdater.Services.User;

namespace VNDBUpdater.Services.WindowHandler
{
    public class WindowHandlerService : IWindowHandlerService
    {
        private UserModel _User;
        private List<Window> _WindowCollection;

        private IUserService _UserService;

        public WindowHandlerService(IUserService UserService)
        {
            _WindowCollection = new List<Window>();

            _UserService = UserService;
            _User = Task.Run(() => _UserService.GetAsync()).Result;

            _UserService.OnUpdated += OnUserUpdated;
        }

        private void OnUserUpdated(object sender, UserModel User)
        {
            _User = User;
        }

        public void Open(Window window, CancelEventHandler closingEventHandler = null)
        {
            if (closingEventHandler != null)
            {
                window.Closing += closingEventHandler;
            }

            window.Closing += OnWindowClosing;

            SetTaskbarVisibility(window);

            window.Show();
            window.Activate();

            _WindowCollection.Add(window);
        }

        public void Show(object controlToSearchForWindow)
        {
            var window = GetTaskbarWindow(controlToSearchForWindow);

            window.ShowInTaskbar = true;

            window.Show();
            window.Activate();
            window.WindowState = WindowState.Normal;
        }

        public void Minimize(Window window)
        {
            window.WindowState = WindowState.Minimized;

            SetTaskbarVisibility(window);
        }

        public void CloseAll()
        {
            for (int i = 0; i < _WindowCollection.Count; i++)
            {
                _WindowCollection[i].Close();
            }
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _WindowCollection.Remove(_WindowCollection.Where(x => x.Equals((sender as Window))).First());
        }

        private void SetTaskbarVisibility(Window window)
        {
            window.ShowInTaskbar = !_User.Settings.MinimizeToTray;
        }

        private Window GetTaskbarWindow(object commandParameter)
        {
            var tb = commandParameter as TaskbarIcon;

            return tb == null 
                ? null 
                : TryFindParent<Window>(tb);
        }

        private T TryFindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = GetParentObject(child);

            if (parentObject == null)
            {
                return null;
            }

            T parent = parentObject as T;

            if (parent != null)
            {
                return parent;
            }
            else
            {
                return TryFindParent<T>(parentObject);
            }
        }

        private DependencyObject GetParentObject(DependencyObject child)
        {
            if (child == null)
            {
                return null;
            }

            ContentElement contentElement = child as ContentElement;

            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);

                if (parent != null)
                {
                    return parent;
                }

                FrameworkContentElement fce = contentElement as FrameworkContentElement;

                return fce != null
                    ? fce.Parent
                    : null;
            }

            return VisualTreeHelper.GetParent(child);
        }
    }
}
