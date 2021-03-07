using Reactive.Bindings;
using System;
using System.Reactive.Linq;
using System.ComponentModel;

namespace SZ2.ECUSimGUI.Model
{
    public class ReactivePropertyBlazorModelBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected ReactivePropertySlim<T> GetDefaultReactivePropertySlim<T>(T defaultVal, string name)
        {
            var rp = new ReactivePropertySlim<T>(defaultVal);
            rp.Subscribe(_ =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            });
            return rp;
        }
        protected ReactiveProperty<T> GetDefaultReactiveProperty<T>(T defaultVal, string name)
        {
            var rp = new ReactiveProperty<T>(defaultVal);
            rp.Subscribe(_ =>
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            });
            return rp;
        }
    }
}