using System;

namespace TextDecoratorDotNet
{
    public class TemplateException : Exception
    {
        public TemplateException(string message)
            : base(message)
        {
        }
    }
}
