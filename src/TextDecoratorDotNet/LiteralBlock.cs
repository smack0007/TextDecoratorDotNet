namespace TextDecoratorDotNet
{
    public class LiteralBlock : TemplateBlock
    {
        private readonly string literal;

        public LiteralBlock(string literal)
        {
            this.literal = literal;
        }

        public override void Execute(TemplateContext context)
        {
            context.Output.Write(this.literal);
        }

        public override string ToString()
        {
            return this.literal;
        }
    }
}
