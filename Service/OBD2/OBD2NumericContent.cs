using System;

namespace SZ2.ECUSimulatorGUI.Service.OBD2
{
    public class OBD2NumericContent
    {
        private readonly byte[] _valBytes;
        private readonly byte _pid;
        private readonly int _pidByteLength;
        private Func<double, double> _conversion_function;
        private String _unit;
        public OBD2NumericContent(byte pid, int pidByteLength, Func<double, double> conversion_function, String unit)
        {
            _pid = pid;
            _pidByteLength = pidByteLength;
            _valBytes = new byte[_pidByteLength];
            _conversion_function = conversion_function;
            _unit = unit;
        }

        public byte PID { get { return _pid; } }
        public int PIDByteLength { get { return _pidByteLength; } }

        public byte[] ValueBytes { get { return _valBytes; } }

        public void SetValue(UInt32 value)
        {
            if((value >> (_valBytes.Length*4)) > 0)
                throw new ArgumentException("Value is out of range.");
                
            for(int i = 0; i < _valBytes.Length; i++)
            {
                int maskOffset = i*4;
                UInt32 mask = (UInt32)0b1111 << maskOffset;
                _valBytes[i] = (byte)((value & mask) >> maskOffset);
            }
        }

        public double ConvertedPhysicalValue
        {
            get
            {
                UInt32 intVal = 0;
                for(int i = 0; i < _pidByteLength; i++)
                    intVal = intVal | (UInt32)_valBytes[i] << i;
                
                return _conversion_function((double)intVal);
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
