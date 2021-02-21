using System;
using System.ComponentModel;
using Reactive.Bindings;
using SZ2.ECUSimulatorGUI.Service.OBD2;

namespace SZ2.ECUSimulatorGUI.Model
{
    public class ECUSimulatorGUIViewModel : INotifyPropertyChanged
    {
        private readonly ECUSimulatorGUIModel Model;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => Model.PropertyChanged += value;
            remove => Model.PropertyChanged -= value;
        }

        public ReactivePropertySlim<string> COMPortName { get => Model.COMPortName; }
        public ReactivePropertySlim<bool> StartButtonEnabled { get => Model.StartButtonEnabled; }
        public ReadOnlyReactivePropertySlim<bool> StopButtonEnabled { get => Model.StopButtonEnabled; }
        public ReactivePropertySlim<OBD2ParameterCode> ParameterCodeToSet { get => Model.ParameterCodeToSet; }
        public ReadOnlyReactiveProperty<UInt32> MaxValue { get => Model.MaxValue; }
        public ReactivePropertySlim<UInt32> SetValue { get => Model.SetValue; }
        public ReactiveCommand StartCommand { get => Model.StartCommand; }
        public ReactiveCommand StopCommand { get => Model.StopCommand; }

        public ECUSimulatorGUIViewModel(ECUSimulatorGUIModel model)
        {
            Model = model;
        }
    }
}