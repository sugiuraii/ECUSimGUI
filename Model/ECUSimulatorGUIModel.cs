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
        public ReactivePropertySlim<string> COMPortName { get; set; }
        public ReactivePropertySlim<bool> StartButtonEnabled { get; set; }
        public ReadOnlyReactivePropertySlim<bool> StopButtonEnabled { get; set; }
        public ReactivePropertySlim<OBD2ParameterCode> ParameterCodeToSet { get; set; }
        public ReadOnlyReactiveProperty<UInt32> MaxValue { get; set; }
        public ReactivePropertySlim<UInt32> SetValue { get; set; }
        public ReadOnlyReactivePropertySlim<double> PhysicalValue {get; set;}
        public ReadOnlyReactivePropertySlim<string> PhysicalUnit {get; set;}
        
        public ReactiveCommand StartCommand { get; set; }
        public ReactiveCommand StopCommand { get; set; }
        public ECUSimulatorGUIModel(ECUSimCommunicationService serivce, ILogger<ECUSimulatorGUIModel> logger)
        {
            this.logger = logger;
            this.Service = serivce;

            this.COMPortName = GetDefaultReactivePropertySlim<string>("/dev/ttyUSB0", "COMPortName");
            this.StartButtonEnabled = GetDefaultReactivePropertySlim<bool>(true, "StartButtonEnabled");
            this.StopButtonEnabled = this.StartButtonEnabled.Select(v => !v).ToReadOnlyReactivePropertySlim();

            this.ParameterCodeToSet = GetDefaultReactivePropertySlim<OBD2ParameterCode>(OBD2ParameterCode.Engine_Load, "ParameterCodeToSet");
            this.SetValue = GetDefaultReactivePropertySlim<UInt32>(0, "SetValue");
            this.SetValue.Subscribe(v => Service.SetPIDValue(ParameterCodeToSet.Value, v));

            this.MaxValue = ParameterCodeToSet.Select(code => Service.GetMaxUInt32Val(code)).ToReadOnlyReactiveProperty();
            this.PhysicalUnit = ParameterCodeToSet.Select(code => Service.GetPhysicalUnit(code)).ToReadOnlyReactivePropertySlim();
            
            this.ParameterCodeToSet.Subscribe(cd => SetValue.Value = Service.GetUInt32Val(cd));
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
            this.MaxValue.Dispose();
            this.SetValue.Dispose();
            this.StartCommand.Dispose();
            this.StopCommand.Dispose();
        }
    }
}