using System;

namespace SZ2.ECUSimGUI.Service.OBD2
{
    public class ECUSimException : Exception
    {
        public ECUSimException(string message) : base(message)
        {
        }
    }
}