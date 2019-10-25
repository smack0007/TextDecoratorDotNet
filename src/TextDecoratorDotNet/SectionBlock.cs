using System.IO;

namespace TextDecoratorDotNet
{
    internal class SectionBlock : ContainerBlock
    {
        private string variableName;
        private bool isInverted;

        public SectionBlock(string variableName, bool isInverted)
        {
            this.variableName = variableName;
            this.isInverted = isInverted;
        }

        public override void Execute(ExecuteContext context)
        {
            object value = null;
            if (context.Variables.TryGetValue(this.variableName, out value) &&
                !VariableHelper.IsFalsyValue(value))
            {
                foreach (var block in this.Blocks)
                {
                    block.Execute(context);
                }
            }
        }

        public override string ToString()
        {
            return "{{#" + this.variableName + "}}";
        }
    }
}
