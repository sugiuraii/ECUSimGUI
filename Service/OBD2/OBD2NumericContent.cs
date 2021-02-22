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
            if(pid % 0x20 == 0)
                throw new ArgumentException("PIDs mutiple of 0x20 are reserved for available PID flag.");
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
        public UInt32 UInt32Value
        {
            get
            {
                UInt32 uint32Val = 0;
                for(int i = 0; i < PIDByteLength; i++)
                {
                    int bitShift = 8*(PIDByteLength - i - 1);
                    uint32Val = uint32Val | (UInt32)(_valBytes[i] << bitShift);
                }
                return uint32Val;
            }
        }
        public void SetValue(UInt32 value)
        {
            if (value > MaxUInt32Val)
                throw new ArgumentException("Value is out of range.");

            for (int i = 0; i < _valBytes.Length; i++)
            {
                int maskOffset = i * 8;
                UInt32 mask = 0xFFU << maskOffset;
                int byteIndexToSet = _valBytes.Length - i;
                _valBytes[i] = (byte)((value & mask) >> maskOffset);
            }
            // Reverse byte order
            Array.Reverse(_valBytes);
        }

        public double ConvertedPhysicalValue
        {
            get => _conversion_function((double)this.UInt32Value);
        }

        public string Unit { get => _unit; }
        public UInt32 MaxUInt32Val { get => _maxUInt32Val; }

        private UInt32 CalcMaxUInt32Val(int byteLength)
        {
            switch (byteLength)
            {
                case 1:
                    return 0xFFU;
                case 2:
                    return 0xFFFFU;
                case 3:
                    return 0xFFFFFFU;
                case 4:
                    return 0xFFFFFFFFU;
                default:
                    throw new ArgumentException("Byte length needs to be 1 to 4");
            }
        }
    }
}
