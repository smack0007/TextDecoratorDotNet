using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace TextDecoratorDotNet.Tests
{
    public class CodeBlockTestsContext
    {
        public IEnumerable<string> Names { get; set; } = Enumerable.Empty<string>();
    }

    public class CodeBlockTests
    {
        [Fact]
        public void SingleLineCodeBlock()
        {
            AssertTemplate.Equal(
@"Bar, Baz",
@"@{ var namesForDisplay = string.Join("", "", Names.Where(x => x.StartsWith(""B""))); }@namesForDisplay",
                new CodeBlockTestsContext()
                {
                    Names = new string[] { "Foo", "Bar", "Baz" }
                });
        }

        [Fact]
        public void MultiLineCodeBlock()
        {
            AssertTemplate.Equal(
@"Bar, Baz",
@"@{
    var namesToDisplay = Names.Where(x => x.StartsWith(""B""));
    var namesForDisplay = string.Join("", "", namesToDisplay);
}@namesForDisplay",
                new CodeBlockTestsContext()
                {
                    Names = new string[] { "Foo", "Bar", "Baz" }
                });
        }
    }
}
