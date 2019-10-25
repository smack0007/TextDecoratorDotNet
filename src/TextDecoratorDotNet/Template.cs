using System;
using System.IO;

namespace TextDecoratorDotNet
{
    public class Template
    {
        private RootBlock _root;

        internal Template(RootBlock root)
        {
            _root = root;
        }

        public void Run(TextWriter output, TemplateVariables variables)
        {
            var context = new TemplateContext(output, variables);
            _root.Execute(context);
        }
    }
}
