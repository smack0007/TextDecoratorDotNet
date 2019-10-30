using System;

namespace TextDecoratorDotNet
{
    public class CodeGeneratorParameters
    {
        public Type TemplateContextType { get; }

        public bool IncludeLineDirectives { get; }

        public CodeGeneratorParameters(
            Type templateContextType,
            bool includeLineDirectives = true)
        {
            TemplateContextType = templateContextType;
            IncludeLineDirectives = includeLineDirectives;
        }
    }
}
