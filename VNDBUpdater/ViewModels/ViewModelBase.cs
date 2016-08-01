using System.Collections.Specialized;
using System.ComponentModel;

namespace VNDBUpdater.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected CommandMap _Commands;

        public CommandMap Commands { get { return _Commands; } }

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ViewModelBase()
        {
            _Commands = new CommandMap();
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
