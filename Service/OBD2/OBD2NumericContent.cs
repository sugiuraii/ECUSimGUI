using System;

namespace SZ2.ECUSimulatorGUI.Service.OBD2
{
    public class OBD2NumericContent
    {
        private readonly byte _pid;
        private readonly int _returnByteLength;
        private Func<double, double> _conversion_function;
        private String _unit;
        public OBD2NumericContent(byte pid, int returnByteLength, Func<double, double> conversion_function, String unit)
        {
            _pid = pid;
            _returnByteLength = returnByteLength;
            _conversion_function = conversion_function;
            _unit = unit;
        }

        public byte PID { get { return _pid; } }
        public int ReturnByteLength { get { return _returnByteLength; } }

        public Func<double, double> ConversionFunction
        {
            get
            {
                return _conversion_function;
            }
        }

        public String Unit
        {
            get
            {
                return _unit;
            }
        }
    }
}
