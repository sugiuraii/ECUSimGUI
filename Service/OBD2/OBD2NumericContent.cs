using System;

namespace SZ2.ECUSimGUI.Service.OBD2
{
    public class OBD2NumericContent
    {
        private readonly byte[] _valBytes;
        private readonly byte _pid;
        private readonly int _pidByteLength;
        private Func<UInt32, double> _conversion_function;
        private readonly string _unit;

        public OBD2NumericContent(byte pid, int pidByteLength, Func<UInt32, double> conversion_function, String unit)
        {
            if (pid % 0x20 == 0)
                throw new ArgumentException("PIDs mutiple of 0x20 are reserved for available PID flag.");
            _pid = pid;
            _pidByteLength = pidByteLength;
            _valBytes = new byte[_pidByteLength];
            _conversion_function = conversion_function;
            _unit = unit;
        }

        public byte PID { get => _pid; }
        public int PIDByteLength { get => _pidByteLength; }

        public byte[] ValueBytes { get => _valBytes; }
        public int ByteLength { get => _valBytes.Length; }
        public void SetValue(byte[] value)
        {
            if (value.Length > ByteLength)
                throw new ArgumentException("Value is out of range.");

            for (int i = 0; i < _valBytes.Length; i++)
                _valBytes[i] = value[i];
        }

        public double ConvertedPhysicalValue
        {
            get => _conversion_function(GetUInt32Value());
        }
        private UInt32 GetUInt32Value()
        {
            UInt32 uint32Val = 0;
            for (int i = 0; i < PIDByteLength; i++)
            {
                int bitShift = 8 * (PIDByteLength - i - 1);
                uint32Val = uint32Val | (UInt32)(_valBytes[i] << bitShift);
            }
            return uint32Val;
        }

        public string Unit { get => _unit; }

    }
}
