using System;
using System.ComponentModel;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SZ2.ECUSimGUI.Service.OBD2;
using System.Reactive.Linq;

namespace SZ2.ECUSimGUI.Model
{
    public class ECUSimGUIViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly ECUSimGUIModel Model;
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

        public ReadOnlyReactivePropertySlim<int> ValueByteLength { get => Model.ValueByteLength; }
        public ReactiveCommand StartCommand { get => Model.StartCommand; }
        public ReactiveCommand StopCommand { get => Model.StopCommand; }
        public ReadOnlyReactivePropertySlim<double> PhysicalValue { get => Model.PhysicalValue; }
        public ReadOnlyReactivePropertySlim<string> PhysicalUnit { get => Model.PhysicalUnit; }

        public ECUSimGUIViewModel(ECUSimGUIModel model)
        {
            Model = model;
            MaxUInt32Value = ValueByteLength.Select(v => MaxValFromByteLength(v)).ToReadOnlyReactivePropertySlim();
            UInt32SetValue = SetValue.ToReactivePropertySlimAsSynchronized(p => p.Value,
                                                                           v =>
                                                                           {
                                                                               UInt32 uint32Val = 0;
                                                                               for (int i = 0; i < v.Length; i++)
                                                                               {
                                                                                   int bitShift = 8 * (v.Length - i - 1);
                                                                                   uint32Val |= (uint)v[i] << bitShift;
                                                                               }
                                                                               return uint32Val;
                                                                           },
                                                                           v =>
                                                                           {
                                                                               byte[] bytes = new byte[ValueByteLength.Value];
                                                                               for (int i = 0; i <  bytes.Length; i++)
                                                                               {                                                                                   
                                                                                   int bitShift = 8 * (bytes.Length - i - 1);
                                                                                   bytes[i] = (byte)((v & (0x000000FF << bitShift)) >> bitShift);
                                                                               }
                                                                               return bytes;
                                                                           });
            SetValueByteEnabledFlag = ValueByteLength.Select(v => new bool[] { v >= 1, v >= 2, v >= 3, v >= 4 }).ToReadOnlyReactivePropertySlim();
        }

        private UInt32 MaxValFromByteLength(int byteLength) => byteLength switch
        {
            1 => 0x000000FF,
            2 => 0x0000FFFF,
            3 => 0x00FFFFFF,
            4 => 0xFFFFFFFF,
            _ => throw new ArgumentException("byte length needs to be betwee 1 to 4.")
        };

        public void Dispose()
        {
            MaxUInt32Value.Dispose();
            UInt32SetValue.Dispose();
            SetValueByteEnabledFlag.Dispose();
            SetValueByteEnabledFlag.Dispose();
        }
    }
}