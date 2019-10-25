using Xunit;

namespace TextDecoratorDotNet.Tests
{
    public class VariableHelperTests
    {
        [Theory]
        [InlineData(null, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(0, true)]
        [InlineData(1, false)]
        [InlineData("", true)]
        [InlineData("!", false)]
        [InlineData(new object[] { }, true)]
        [InlineData(new object[] { "a", 1, false }, false)]
        public void IsFalsyValueTests(object value, bool expected)
        {
            Assert.Equal(expected, VariableHelper.IsFalsyValue(value));
        }
    }
}
