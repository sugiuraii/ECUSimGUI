using System;

namespace SZ2.ECUSimulatorGUI.Service.OBD2
{
    public class ECUSimulatorException : Exception
    {
        public ECUSimulatorException(string message) : base(message)
        {
        }
    }
}