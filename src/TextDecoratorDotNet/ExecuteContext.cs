using System.IO;

namespace TextDecoratorDotNet
{
    public class ExecuteContext
    {
        public TextWriter Output { get; }

        public TemplateVariables Variables { get; }

        public ExecuteContext(TextWriter output, TemplateVariables variables)
        {
            this.Output = output;
            this.Variables = variables;
        }
    }
}
