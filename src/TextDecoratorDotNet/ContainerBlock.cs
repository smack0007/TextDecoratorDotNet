using System.Collections.Generic;

namespace TextDecoratorDotNet
{
    public abstract class ContainerBlock : TemplateBlock
    {
        public List<TemplateBlock> Blocks { get; } = new List<TemplateBlock>();
    }
}
