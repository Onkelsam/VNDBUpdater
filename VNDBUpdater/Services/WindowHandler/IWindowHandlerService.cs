using System.ComponentModel;
using System.Windows;

namespace VNDBUpdater.Services.WindowHandler
{
    public interface IWindowHandlerService
    {
        void Open(Window window, CancelEventHandler closingEventHandler = null);
        void Show(object controlToSearchForWindow);
        void Minimize(Window window);
        void CloseAll();
    }
}
