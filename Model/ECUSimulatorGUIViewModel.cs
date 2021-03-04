using System;
using System.ComponentModel;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SZ2.ECUSimulatorGUI.Service.OBD2;
using System.Reactive.Linq;

namespace SZ2.ECUSimulatorGUI.Model
{
    public class ECUSimulatorGUIViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly ECUSimulatorGUIModel Model;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add => Model.PropertyChanged += value;
            remove => Model.PropertyChanged -= value;
        }

        public event EventHandler<Exception> CommunicateErrorOccured
        {
            add => Model.CommunicateErrorOccured += value;
            remove => Model.CommunicateErrorOccured += value;
        }

        public ReactivePropertySlim<string> COMPortName { get => Model.COMPortName; }
        public ReactivePropertySlim<bool> StartButtonEnabled { get => Model.StartButtonEnabled; }
        public ReadOnlyReactivePropertySlim<bool> StopButtonEnabled { get => Model.StopButtonEnabled; }
        public ReactivePropertySlim<OBD2ParameterCode> ParameterCodeToSet { get => Model.ParameterCodeToSet; }
        public ReadOnlyReactivePropertySlim<UInt32> MaxUInt32Value { get; private set; }
        public ReactivePropertySlim<UInt32> UInt32SetValue { get; private set; }
        public ReactivePropertySlim<byte[]> SetValue { get => Model.SetValue; }
        public ReadOnlyReactivePropertySlim<bool[]> SetValueByteEnabledFlag { get; private set; }
        public ReactivePropertySlim<UInt16[]> UInt16SetValue { get; private set;}
        public ReadOnlyReactivePropertySlim<bool[]> SetValueUInt16EnabledFlag { get; private set; }
        
        public ReadOnlyReactivePropertySlim<int> ValueByteLength { get => Model.ValueByteLength; }
        public ReactiveCommand StartCommand { get => Model.StartCommand; }
        public ReactiveCommand StopCommand { get => Model.StopCommand; }
        public ReadOnlyReactivePropertySlim<double> PhysicalValue { get => Model.PhysicalValue; }
        public ReadOnlyReactivePropertySlim<string> PhysicalUnit { get => Model.PhysicalUnit; }

        public ECUSimulatorGUIViewModel(ECUSimulatorGUIModel model)
        {
            Model = model;
            MaxUInt32Value = ValueByteLength.Select(v => MaxValFromByteLength(v)).ToReadOnlyReactivePropertySlim();
            UInt32SetValue = SetValue.ToReactivePropertySlimAsSynchronized(p => p.Value,
                                                                           v => (uint)v[0] << 24 | (uint)v[1] << 16 | (uint)v[2] << 8 | (uint)v[3],
                                                                           v => new byte[] { (byte)(v & 0xFF000000 >> 24), (byte)(v & 0x00FF0000 >> 16), (byte)(v & 0x0000FF00 >> 8), (byte)(v & 0x000000FF) });
            SetValueByteEnabledFlag = ValueByteLength.Select(v => new bool[] { v >= 1, v >= 2, v >= 3, v >= 4 }).ToReadOnlyReactivePropertySlim();
            UInt16SetValue = SetValue.ToReactivePropertySlimAsSynchronized(p => p.Value,
                                                                           v => new UInt16[] {(ushort)((uint)v[0] << 8 | (uint)v[1]),  (ushort)((uint)v[2] << 8 | (uint)v[3])},
                                                                           v => new byte[] { (byte)(v[0] & 0xFF00 >> 8), (byte)(v[0] & 0x00FF), (byte)(v[1] & 0xFF00 >> 8), (byte)(v[1] & 0x00FF) });
            SetValueUInt16EnabledFlag = ValueByteLength.Select(v => new bool[] { v >= 2, v >= 4}).ToReadOnlyReactivePropertySlim();   
        }

        private UInt32 MaxValFromByteLength(int byteLength) => ~((0xFFFFFFFFU << byteLength));

        public void Dispose()
        {
            MaxUInt32Value.Dispose();
            UInt32SetValue.Dispose();
            SetValueByteEnabledFlag.Dispose();
            UInt16SetValue.Dispose();
            SetValueByteEnabledFlag.Dispose();
        }
    }
}