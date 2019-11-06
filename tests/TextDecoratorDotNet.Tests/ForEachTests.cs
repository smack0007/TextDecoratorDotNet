using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TextDecoratorDotNet.Tests
{
    public class ForEachTestsContext
    {
        public IEnumerable<string> Names { get; set; } = Enumerable.Empty<string>();
    }

    public class ForEachTests
    {
        [Fact]
        public void SingleForEachLoopProducesSingleLine()
        {
            AssertTemplate.Equal(
@"Hello Foo! Hello Bar! Hello Baz! ",
@"@foreach (var name in Names) {Hello @name! }",
                new ForEachTestsContext()
                {
                    Names = new string[] { "Foo", "Bar", "Baz" },
                });
        }

        [Fact]
        public void SingleForEachLoopProducesMultipleLines()
        {
            AssertTemplate.Equal(
@"Hello Foo!
Hello Bar!
Hello Baz!
",
@"@foreach (var name in Names)
{
Hello @name!
}",
                new ForEachTestsContext()
                {
                    Names = new string[] { "Foo", "Bar", "Baz" },
                });
        }
    }
}
