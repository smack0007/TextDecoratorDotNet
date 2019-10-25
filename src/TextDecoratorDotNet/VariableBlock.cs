using System.IO;

namespace TextDecoratorDotNet
{
    public class VariableBlock : TemplateBlock
    {
        private readonly string variableName;

        internal VariableBlock(string variableName)
        {
            this.variableName = variableName;
        }

        public override void Execute(ExecuteContext context)
        {
            object value;
            if (context.Variables.TryGetValue(this.variableName, out value))
            {
                context.Output.Write(value.ToString());
            }
        }

        public override string ToString()
        {
            return "{{" + this.variableName + "}}";
        }
    }
}
