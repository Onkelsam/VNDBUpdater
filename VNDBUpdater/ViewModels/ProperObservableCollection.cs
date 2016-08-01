using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace VNDBUpdater.ViewModels
{
    /// <summary>
    /// Observablecollection that actually updates when an object in it gets changed.
    /// See: https://stackoverflow.com/questions/1427471/observablecollection-not-noticing-when-item-in-it-changes-even-with-inotifyprop
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ProperObservableCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public ProperObservableCollection()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        public ProperObservableCollection(IEnumerable<T> pItems)
            : this()
        {
            foreach (var item in pItems)
            {
                Add(item);
            }
        }

        private void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                }
            }
        }

        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
            OnCollectionChanged(args);
        }
    }
}
