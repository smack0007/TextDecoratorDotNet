using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TextDecoratorDotNet.Tests
{
    public class GetTestsContext
    {
        public string Name { get; set; }
    }

    public class GetTests
    {
        [Fact]
        public void GetStringProperty()
        {
            AssertTemplate.Equal(
                "Hello FooBar!",
                "Hello @Name!",
                new GetTestsContext()
                {
                    Name = "FooBar",
                });
        }
    }
}
