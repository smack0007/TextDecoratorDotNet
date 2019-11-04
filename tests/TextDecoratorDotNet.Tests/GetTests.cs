using Xunit;

namespace TextDecoratorDotNet.Tests
{
    public class GetTestsContext
    {
        public string Name1 { get; set; }
        
        public string Name2 { get; set; }
    }

    public class GetTests
    {
        [Fact]
        public void GetStringProperty()
        {
            AssertTemplate.Equal(
                "Hello FooBar!",
                "Hello @Name1!",
                new GetTestsContext()
                {
                    Name1 = "FooBar",
                });
        }

        [Fact]
        public void GetMultipleStringProperties()
        {
            AssertTemplate.Equal(
                "Hello Foo and Bar!",
                "Hello @Name1 and @Name2!",
                new GetTestsContext()
                {
                    Name1 = "Foo",
                    Name2 = "Bar"
                });
        }
    }
}
