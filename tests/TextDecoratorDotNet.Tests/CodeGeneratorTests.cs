using System;
using System.Collections.Generic;
using Xunit;

namespace TextDecoratorDotNet.Tests
{
    public class CodeGeneratorTests
    {
        [Theory]
        [InlineData("void", typeof(void))]
        [InlineData("System.Int32", typeof(int))]
        [InlineData("System.String", typeof(string))]
        [InlineData("System.Collections.Generic.List<System.String>", typeof(List<string>))]
        public void GetTypeString(string expected, Type input)
        {
            Assert.Equal(expected, CodeGenerator.GetTypeString(input));
        }
    }
}
