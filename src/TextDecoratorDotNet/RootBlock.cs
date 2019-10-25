using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDecoratorDotNet
{
    internal class RootBlock : ContainerBlock
    {
        public override void Execute(ExecuteContext context)
        {
            foreach (var block in this.Blocks)
            {
                block.Execute(context);
            }
        }
    }
}
