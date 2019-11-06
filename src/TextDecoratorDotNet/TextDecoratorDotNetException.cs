using System;

namespace TextDecoratorDotNet
{
    public class TextDecoratorDotNetException : Exception
    {
        internal TextDecoratorDotNetException(string message)
            : base(message)
        {
        }

        internal TextDecoratorDotNetException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
