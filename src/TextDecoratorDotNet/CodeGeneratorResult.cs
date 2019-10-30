using System.Collections.Generic;

namespace TextDecoratorDotNet
{
    public class CodeGeneratorResult
    {
        public string Code { get; set; }

        public List<string> ReferencedAssemblies { get; private set; }

        public List<string> Usings { get; private set; }

        public List<string> Properties { get; private set; }

        public CodeGeneratorResult()
        {
            ReferencedAssemblies = new List<string>();
            Usings = new List<string>();
            Properties = new List<string>();
        }
    }
}
