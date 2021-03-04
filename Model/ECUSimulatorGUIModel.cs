using System;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using SZ2.ECUSimulatorGUI.Service;
using Reactive.Bindings;
using System.Reactive.Linq;
using SZ2.ECUSimulatorGUI.Service.OBD2;

namespace SZ2.ECUSimulatorGUI.Model
{
    public class ECUSimulatorGUIModel : ReactivePropertyBlazorModelBase, IDisposable
    {
        private readonly ILogger logger;
        private readonly ECUSimCommunicationService Service;
        public event EventHandler<Exception> CommunicateErrorOccured;
        public ReactivePropertySlim<string> COMPortName { get; private set; }
        public ReactivePropertySlim<bool> StartButtonEnabled { get; private set; }
        public ReadOnlyReactivePropertySlim<bool> StopButtonEnabled { get; private set; }
        public ReactivePropertySlim<OBD2ParameterCode> ParameterCodeToSet { get; private set; }
        public ReadOnlyReactivePropertySlim<int> ValueByteLength { get; private set; }
        public ReactivePropertySlim<byte[]> SetValue { get; private set; }
        public ReadOnlyReactivePropertySlim<double> PhysicalValue {get; private set;}
        public ReadOnlyReactivePropertySlim<string> PhysicalUnit {get; private set;}
        
        public ReactiveCommand StartCommand { get; private set; }
        public ReactiveCommand StopCommand { get; private set; }
        public ECUSimulatorGUIModel(ECUSimCommunicationService serivce, ILogger<ECUSimulatorGUIModel> logger)
        {
            this.logger = logger;
            this.Service = serivce;

            this.COMPortName = GetDefaultReactivePropertySlim<string>("/dev/ttyUSB0", "COMPortName");
            this.StartButtonEnabled = GetDefaultReactivePropertySlim<bool>(true, "StartButtonEnabled");
            this.StopButtonEnabled = this.StartButtonEnabled.Select(v => !v).ToReadOnlyReactivePropertySlim();

            this.ParameterCodeToSet = GetDefaultReactivePropertySlim<OBD2ParameterCode>(OBD2ParameterCode.Engine_Load, "ParameterCodeToSet");
            this.SetValue = GetDefaultReactivePropertySlim<byte[]>(new byte[]{0,0,0,0}, "SetValue");
            this.SetValue.Subscribe(v => Service.SetPIDValue(ParameterCodeToSet.Value, v));

            this.ValueByteLength = ParameterCodeToSet.Select(code => Service.GetPIDByteLength(code)).ToReadOnlyReactivePropertySlim();
            this.PhysicalUnit = ParameterCodeToSet.Select(code => Service.GetPhysicalUnit(code)).ToReadOnlyReactivePropertySlim();
            
            this.ParameterCodeToSet.Subscribe(cd => SetValue.Value = Service.GetPIDValue(cd));
            this.PhysicalValue = SetValue.Select(_ => Service.GetConvertedPhysicalVal(ParameterCodeToSet.Value)).ToReadOnlyReactivePropertySlim();
            
            this.StartCommand = StartButtonEnabled.ToReactiveCommand(); // Can run only on StartButtonEnabled = true;
            this.StartCommand.Subscribe(() => Service.CommunicateStart(COMPortName.Value));
            this.StopCommand = StopButtonEnabled.ToReactiveCommand(); // Can run only on StopButtonEnabled = true;
            this.StopCommand.Subscribe(() => Service.CommunicateStop());

            Service.CommunicateStateChanged += (sender, arg) => StartButtonEnabled.Value = !arg;
            Service.CommunicateErrorOccured += (sender, arg) => this.CommunicateErrorOccured(sender, arg);
        }

        public void Dispose()
        {
            this.COMPortName.Dispose();
            this.StartButtonEnabled.Dispose();
            this.StopButtonEnabled.Dispose();
            this.ParameterCodeToSet.Dispose();
            this.ValueByteLength.Dispose();
            this.SetValue.Dispose();
            this.PhysicalUnit.Dispose();
            this.PhysicalValue.Dispose();
            this.StartCommand.Dispose();
            this.StopCommand.Dispose();
        }
    }
}