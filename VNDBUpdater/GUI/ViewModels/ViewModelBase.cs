using Microsoft.Practices.Unity;
using System.Collections.Specialized;
using System.ComponentModel;
using VNDBUpdater.Services.WindowHandler;

namespace VNDBUpdater.GUI.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected IWindowHandlerService _WindowHandler;

        [Dependency]
        public IWindowHandlerService WindowHandler
        {
            set { _WindowHandler = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;        

        public ViewModelBase()
        {
        }

        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(sender, e);
        }      
    }
}
