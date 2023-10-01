using System;
using System.Diagnostics.CodeAnalysis;

namespace GDX
{
    /// <summary>
    ///     An exception used to indicate when a method is unavailable on a specific runtime.
    /// </summary>
    [ExcludeFromCodeCoverage]
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