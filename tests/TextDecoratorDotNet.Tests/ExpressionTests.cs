using Xunit;

namespace TextDecoratorDotNet.Tests
{
    public class ExpressionTestsContext
    {
        public string Name { get; set; } = string.Empty;

        public string Wrap(string input) => $"#{input}#";
    }

    public class ExpressionTests
    {
        [Fact]
        public void ExpressionsCanBeginWithParen()
        {
            AssertTemplate.Equal(
                "Hello #FooBar#!",
                "Hello @(Wrap(Name))!",
                new ExpressionTestsContext()
                {
                    Name = "FooBar",
                });
        }
    }
}
