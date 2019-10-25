using System;
namespace TextDecoratorDotNet.Syntax
{
    internal class RootBlock : ContainerBlock
    {
        public override void Execute(TemplateContext context)
        {
            foreach (var block in Blocks)
            {
                block.Execute(context);
            }
        }
    }
}
