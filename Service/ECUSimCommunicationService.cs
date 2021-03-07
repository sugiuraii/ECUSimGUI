using System;
using System.Collections.Generic;
using System.Linq;
using SZ2.ECUSimGUI.Service.OBD2;
using System.IO.Ports;
using System.Text;
using Microsoft.Extensions.Logging;
using System.IO;

namespace SZ2.ECUSimGUI.Service
{
    public class ECUSimCommunicationService : IDisposable
    {
        private readonly ILogger<ECUSimCommunicationService> logger; 
        public event EventHandler<bool> CommunicateStateChanged;
        public event EventHandler<Exception> CommunicateErrorOccured;
        private readonly OBD2ContentTable obd2ContentTable = new OBD2ContentTable();
        private bool RunningState = false;
        private SerialPort serialPort;

        public ECUSimCommunicationService(ILogger<ECUSimCommunicationService> logger)
        {
            this.logger = logger;
        }

        public void Dispose()
        {
            CommunicateStop();
        }

        public void CommunicateStart(string portName)
        {
            try
            {
                logger.LogInformation("Serial port open.");
                logger.LogInformation("Portname : " + portName);
                serialPort = new SerialPort(portName);
                serialPort.BaudRate = 115200;
                serialPort.NewLine = "\n";
                serialPort.Open();
                logger.LogInformation("BaudRate is " + serialPort.BaudRate.ToString());

                WaitReceivingECUSimReady();

                // Enable SerialPort input logging
                serialPort.DataReceived += (sender, e) => {
                    var port = (SerialPort)sender;
                    string message = port.ReadExisting();
                    if(message.Contains("error", StringComparison.OrdinalIgnoreCase))
                        logger.LogWarning("Error message from ECUSim : " + message);
                    else
                        logger.LogDebug("Message from ECUSim : " + message);
                };

                logger.LogInformation("ECUSim communication started.");
                this.RunningState = true;
                if (CommunicateStateChanged != null)
                    CommunicateStateChanged(this, RunningState);

            }
            catch(Exception ex) when ((ex is IOException) || (ex is ECUSimException) || (ex is UnauthorizedAccessException))
            {
                logger.LogError(ex.Message);
                CommunicateErrorOccured(this, ex);
            }
        }

        private void WaitReceivingECUSimReady()
        {
            logger.LogInformation("Waiting CAN BUS Shield init ok message....");
            while(true)
            {
                string response = serialPort.ReadLine();
                if(response.Contains("CAN BUS Shield init ok!"))
                {
                    logger.LogInformation("CAN BUS Shield init ok message is received.");
                    break;
                }
                if(response.Contains("error", StringComparison.OrdinalIgnoreCase) || response.Contains("fail", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ECUSimException("CAN BUS Shield initialization is failed. Response is :" + response); 
                }
            }
        }

        public void CommunicateStop()
        {
            logger.LogInformation("Serial port closing.");
            if(serialPort != null)
            {
                serialPort.Close();
                serialPort.Dispose();
            }
            this.RunningState = false;
            if (CommunicateStateChanged != null)
                CommunicateStateChanged(this, RunningState);
            logger.LogInformation("Serial port closed.");
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

        public int GetPIDByteLength(OBD2ParameterCode code)
        {
            var obdContent = obd2ContentTable.Table[code];
            return obdContent.ByteLength;
        }

        public byte[] GetPIDValue(OBD2ParameterCode code)
        {
            var obdContent = obd2ContentTable.Table[code];
            return obdContent.ValueBytes;
        }

        public void SetPIDValue(OBD2ParameterCode code, byte[] value)
        {
            var obdContent = obd2ContentTable.Table[code];
            obdContent.SetValue(value);

            WritePIDValue(obdContent.PID, (byte)obdContent.PIDByteLength, obdContent.ValueBytes);
        }

        private void WritePIDValue(byte pid, byte byteLength, byte[] valByte)
        {
            if (RunningState)
            {
                var strBuilder = new StringBuilder();
                strBuilder.Append(pid.ToString("X2"));
                for (int i = 0; i < 4; i++)
                {
                    if (i < byteLength)
                        strBuilder.Append(valByte[i].ToString("X2"));
                    else
                        strBuilder.Append("00");
                }
                var outStr = strBuilder.ToString();
                logger.LogDebug("Write serial message: " + outStr);
                serialPort.WriteLine(outStr);
            }
        }
    }
}