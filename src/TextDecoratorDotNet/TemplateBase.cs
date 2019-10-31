using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextDecoratorDotNet
{
    public class TemplateBase<T>
    {
        private readonly TextWriter _output;

        protected T _Context { get; }

        public TemplateBase(TextWriter output, T context)
        {
            _output = output;
            _Context = context;
        }

        protected void _Write(object value) => _output.Write(value);

        protected void _WriteLiteral(string literal) => _output.Write(literal);
    }
}
