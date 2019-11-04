using System.Collections.Generic;
using Xunit;

namespace TextDecoratorDotNet.Tests
{
    public class ForEachTestsContext
    {
        public List<string> Names { get; set; }
    }

    public class ForEachTests
    {
        [Fact]
        public void SingleForEachLoop()
        {
            AssertTemplate.Equal(
@"Hello Foo!
Hello Bar!
Hello Baz!
",
@"@foreach (var name in Names) {
Hello @name!
}",
                new ForEachTestsContext()
                {
                    Names = new List<string>() { "Foo", "Bar", "Baz" },
                });
        }
    }
}
