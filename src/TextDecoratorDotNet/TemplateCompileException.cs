using System;
using System.Collections.Generic;
using System.Text;

namespace TextDecoratorDotNet
{
    public class TemplateCompileException : TextDecoratorDotNetException
    {
        public string TemplateScript { get; }

        internal TemplateCompileException(string message, string templateScript)
            : base(message)
        {
            TemplateScript = templateScript;
        }

        internal TemplateCompileException(string message, string templateScript, Exception innerException)
            : base(message, innerException)
        {
            TemplateScript = templateScript;
        }
    }
}
