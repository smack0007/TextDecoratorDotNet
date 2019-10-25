using System.Collections.Generic;

namespace TextDecoratorDotNet.Syntax
{
    public abstract class ContainerBlock : TemplateBlock
    {
        public List<TemplateBlock> Blocks { get; } = new List<TemplateBlock>();
    }
}
