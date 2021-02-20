using System;
using System.Collections.Generic;
using System.Linq;
using SZ2.ECUSimulatorGUI.Service.OBD2;
using System.IO.Ports;

namespace SZ2.ECUSimulatorGUI.Service
{
    public class ECUSimCommunicationService : IDisposable
    {
        private readonly OBD2ContentTable obd2ContentTable = new OBD2ContentTable();
        //private readonly SerialPort serialPort;

        public ECUSimCommunicationService()
        {
            /*
            serialPort = new SerialPort("/dev/ttyUSB0");
            serialPort.BaudRate = 115200;
            serialPort.NewLine = "\n";
            serialPort.Open();
            */
        }

        public void Dispose()
        {
            /*
            serialPort.Close();
            serialPort.Dispose();
            */
        }

        public void SetPIDValue(OBD2ParameterCode code, UInt32 value)
        {
            var obdContent = obd2ContentTable.Table[code];
            obdContent.SetValue(value);

            WritePIDValue(obdContent.PID, (byte)obdContent.PIDByteLength, obdContent.ValueBytes);
        }

        private void WritePIDValue(byte pid, byte byteLength, byte[] valByte)
        {
            string pidStr = pid.ToString("X2");
            string byteLengthStr = byteLength.ToString("X2");
            string valByteStr = "";
            for(int i = 0; i < valByte.Length; i++)
                valByteStr.Concat(valByte[i].ToString("X2"));

            Console.WriteLine(pidStr + byteLengthStr + valByteStr);
            //serialPort.WriteLine(pidStr + byteLengthStr + valByteStr);
        }
    }
}