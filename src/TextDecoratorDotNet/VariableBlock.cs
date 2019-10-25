namespace TextDecoratorDotNet
{
    public class VariableBlock : TemplateBlock
    {
        private readonly string _variableName;

        internal VariableBlock(string variableName)
        {
            _variableName = variableName;
        }

        public override void Execute(TemplateContext context)
        {
            if (context.Variables.TryGetValue(_variableName, out var value))
            {
                context.Output.Write(value.ToString());
            }
        }

        public override string ToString()
        {
            return "{{" + _variableName + "}}";
        }
    }
}
