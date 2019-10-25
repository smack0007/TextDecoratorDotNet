using System.IO;

namespace TextDecoratorDotNet
{
    public class TemplateContext
    {
        public TextWriter Output { get; }

        public TemplateVariables Variables { get; }

        public TemplateContext(TextWriter output, TemplateVariables variables)
        {
            Output = output;
            Variables = variables;
        }
    }
}
