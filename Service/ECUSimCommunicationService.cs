using System;
using System.Collections.Generic;
using System.Linq;
using SZ2.ECUSimulatorGUI.Service.OBD2;
using System.IO.Ports;
using System.Text;

namespace SZ2.ECUSimulatorGUI.Service
{
    public class ECUSimCommunicationService : IDisposable
    {
        public event EventHandler<bool> CommunicateStateChanged;
        public event EventHandler<Exception> CommunicateErrorOccured;
        private readonly OBD2ContentTable obd2ContentTable = new OBD2ContentTable();
        private bool RunningState = false;
        //private SerialPort serialPort;

        public ECUSimCommunicationService()
        {
        }

        public void Dispose()
        {
            CommunicateStop();
            /*
            serialPort.Close();
            serialPort.Dispose();
            */
        }

        public void CommunicateStart(string portName)
        {/*
            serialPort = new SerialPort(portName);
            serialPort.BaudRate = 115200;
            serialPort.NewLine = "\n";
            serialPort.Open();
        */
            this.RunningState = true;
            if(CommunicateStateChanged != null)
                CommunicateStateChanged(this, RunningState);
        }

        public void CommunicateStop()
        {
            //serialPort.Close();
            //serialPort.Dispose();
            this.RunningState = false;
            if(CommunicateStateChanged != null)
                CommunicateStateChanged(this, RunningState);
        }
        public UInt32 GetUInt32Val(OBD2ParameterCode code)
        {
            var obdContent = obd2ContentTable.Table[code];
            return obdContent.MaxUInt32Val;
        }

        public double GetConvertedPhysicalVal(OBD2ParameterCode code)
        {
            var obdContent = obd2ContentTable.Table[code];
            return obdContent.ConvertedPhysicalValue;
        }

        public string GetPhysicalUnit(OBD2ParameterCode code)
        {
            var obdContent = obd2ContentTable.Table[code];
            return obdContent.Unit;
        }

        public UInt32 GetMaxUInt32Val(OBD2ParameterCode code)
        {
            var obdContent = obd2ContentTable.Table[code];
            return obdContent.MaxUInt32Val;
        }
        public void SetPIDValue(OBD2ParameterCode code, UInt32 value)
        {
            var obdContent = obd2ContentTable.Table[code];
            obdContent.SetValue(value);

            WritePIDValue(obdContent.PID, (byte)obdContent.PIDByteLength, obdContent.ValueBytes);
        }

        private void WritePIDValue(byte pid, byte byteLength, byte[] valByte)
        {
            if(RunningState)
            {
                var strBuilder = new StringBuilder();
                strBuilder.Append(pid.ToString("X2"));
                strBuilder.Append(byteLength.ToString("X2"));
                for(int i = 0; i < 4; i++)
                {
                    if(i < byteLength)
                        strBuilder.Append(valByte[i].ToString("X2"));
                    else
                        strBuilder.Append("00");
                }
                var outStr = strBuilder.ToString();
                Console.WriteLine(outStr);
                //serialPort.WriteLine(outStr);
            }
        }
    }
}