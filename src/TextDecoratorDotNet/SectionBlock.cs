using System.IO;

namespace TextDecoratorDotNet
{
    internal class SectionBlock : ContainerBlock
    {
        private string _variableName;
        private bool _isInverted;

        public SectionBlock(string variableName, bool isInverted)
        {
            _variableName = variableName;
            _isInverted = isInverted;
        }

        public override void Execute(TemplateContext context)
        {
            if (context.Variables.TryGetValue(_variableName, out var value) &&
                !VariableHelper.IsFalsyValue(value))
            {
                foreach (var block in Blocks)
                {
                    block.Execute(context);
                }
            }
        }

        public override string ToString()
        {
            return "{{#" + _variableName + "}}";
        }
    }
}
