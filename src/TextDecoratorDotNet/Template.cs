using System;
using System.IO;

namespace TextDecoratorDotNet
{
    public class Template
    {
        private RootBlock root;

        internal Template(RootBlock root)
        {
            this.root = root;
        }

        public void Run(TextWriter output, TemplateVariables variables)
        {
            ExecuteContext context = new ExecuteContext(output, variables);
            this.root.Execute(context);
        }
    }
}
