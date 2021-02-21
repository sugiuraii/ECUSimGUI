using System;

namespace SZ2.ECUSimulatorGUI.Service.OBD2
{
    public class OBD2NumericContent
    {
        private readonly byte[] _valBytes;
        private readonly byte _pid;
        private readonly int _pidByteLength;
        private Func<double, double> _conversion_function;
        private readonly string _unit;
        private readonly UInt32 _maxUInt32Val;

        public OBD2NumericContent(byte pid, int pidByteLength, Func<double, double> conversion_function, String unit)
        {
            _pid = pid;
            _pidByteLength = pidByteLength;
            _valBytes = new byte[_pidByteLength];
            _conversion_function = conversion_function;
            _unit = unit;
            _maxUInt32Val = CalcMaxUInt32Val(_pidByteLength);
        }

        public byte PID { get => _pid; }
        public int PIDByteLength { get => _pidByteLength; }

        public byte[] ValueBytes { get => _valBytes; }

        public void SetValue(UInt32 value)
        {
            if (value > MaxUInt32Val)
                throw new ArgumentException("Value is out of range.");

            for (int i = 0; i < _valBytes.Length; i++)
            {
                int maskOffset = i * 4;
                UInt32 mask = (UInt32)0b1111 << maskOffset;
                _valBytes[i] = (byte)((value & mask) >> maskOffset);
            }
        }

        public double ConvertedPhysicalValue
        {
            get
            {
                UInt32 intVal = 0;
                for (int i = 0; i < _pidByteLength; i++)
                    intVal = intVal | (UInt32)_valBytes[i] << i;

                return _conversion_function((double)intVal);
            }
        }

        public string Unit { get => _unit; }
        public UInt32 MaxUInt32Val { get => _maxUInt32Val; }

        private UInt32 CalcMaxUInt32Val(int byteLength)
        {
            switch (byteLength)
            {
                case 1:
                    return 0xFF;
                case 2:
                    return 0xFFFF;
                case 3:
                    return 0xFFFFFF;
                case 4:
                    return 0xFFFFFFFF;
                default:
                    throw new ArgumentException("Byte length needs to be 1 to 4");
            }
        }
    }
}
