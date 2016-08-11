using Hardcodet.Wpf.TaskbarNotification;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using VNDBUpdater.Helper;

namespace VNDBUpdater.Models.Internal
{
    public class WindowHandler
    {
        private List<Window> WindowCollection;

        public WindowHandler()
        {
            WindowCollection = new List<Window>();
        }

        public void Open(Window windowToOpen, CancelEventHandler optionalClosingEventHandler = null)
        {
            if (optionalClosingEventHandler != null)
                windowToOpen.Closing += optionalClosingEventHandler;

            windowToOpen.Closing += OnWindowClosing;

            SetTaskbarVisibility(windowToOpen);

            windowToOpen.Show();
            windowToOpen.Activate();

            WindowCollection.Add(windowToOpen);
        }

        public void Show(object ControlToSearchForWindow)
        {
            var window = GetTaskbarWindow(ControlToSearchForWindow);

            window.ShowInTaskbar = true;

            window.Show();
            window.Activate();
            window.WindowState = WindowState.Normal;
        }

        public void Minimize(Window windoToMinimize)
        {
            windoToMinimize.WindowState = WindowState.Minimized;

            SetTaskbarVisibility(windoToMinimize);
        }

        public void CloseAllWindows()
        {
            for (int i = 0; i < WindowCollection.Count; i++)
                WindowCollection[i].Close();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            WindowCollection.Remove(WindowCollection.Where(x => x.Equals((sender as Window))).First());
        }

        private void SetTaskbarVisibility(Window window)
        {
            if (UserHelper.CurrentUser.Settings.MinimizeToTray)
                window.ShowInTaskbar = false;
            else
                window.ShowInTaskbar = true;
        }

        private Window GetTaskbarWindow(object commandParameter)
        {
            var tb = commandParameter as TaskbarIcon;
            return tb == null ? null : TryFindParent<Window>(tb);
        }

        private T TryFindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = GetParentObject(child);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return TryFindParent<T>(parentObject);
        }

        private DependencyObject GetParentObject(DependencyObject child)
        {
            if (child == null)
                return null;

            ContentElement contentElement = child as ContentElement;

            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);

                if (parent != null)
                    return parent;

                FrameworkContentElement fce = contentElement as FrameworkContentElement;

                return fce != null ? fce.Parent : null;
            }

            return VisualTreeHelper.GetParent(child);
        }
    }
}
