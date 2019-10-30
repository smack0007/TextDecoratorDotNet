using System;

namespace TextDecoratorDotNet
{
    public class TextDecoratorDotNetException : Exception
    {
        public TextDecoratorDotNetException(string message)
            : base(message)
        {
        }

        public TextDecoratorDotNetException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
