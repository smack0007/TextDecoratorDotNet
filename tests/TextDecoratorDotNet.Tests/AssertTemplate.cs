using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TextDecoratorDotNet.Tests
{
    public static class AssertTemplate
    {
        public static void Equal<T>(string expected, string template, T context)
        {
            var templateObj = Template.CompileAsync<T>(template).GetAwaiter().GetResult();

            Assert.Equal(expected, templateObj.Run(context));
        }
    }
}
