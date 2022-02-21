using System;

namespace GDX
{
    public class UnsupportedRuntimeException : Exception
    {
        public UnsupportedRuntimeException()
        {
        }

        public UnsupportedRuntimeException(string message)
            : base(message)
        {
        }

        public UnsupportedRuntimeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}