using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TextDecoratorDotNet.Tests
{
    public class MethodCallTestsContext
    {
        public int VoidMethodCallCount { get; private set; }

        public void VoidMethod()
        {
            VoidMethodCallCount++;
        }

        public string Wrap(string input) => $"#{input}#";
    }

    public class MethodCallTests
    {
        [Fact]
        public void CanCallVoidMethod()
        {
            var context = new MethodCallTestsContext();

            AssertTemplate.Equal(
                @"Hello!",
                @"@{ VoidMethod(); }Hello!",
                context);

            Assert.Equal(1, context.VoidMethodCallCount);            
        }

        [Fact]
        public void CanCallWith1ParamAndReturnValue()
        {
            var context = new MethodCallTestsContext();

            AssertTemplate.Equal(
                @"Hello #Foo#!",
                @"Hello @Wrap(""Foo"")!",
                context);
        }
    }
}
