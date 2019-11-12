using System;
using System.Collections.Generic;
using System.Linq;
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

        public class Type1
        {
            private void PrivateMethod1()  => Console.WriteLine("Foo");
            
            private void PrivateMethod2() => Console.WriteLine("Bar");

            public void Method1()
            {
                PrivateMethod1();
                PrivateMethod2();
            }

            private static void PrivateStaticMethod1() => Console.WriteLine("PrivateStaticMethod1");

            protected static void ProtectedStaticMethod1() => Console.WriteLine("ProtectedStaticMethod1");

            public static void StaticMethod1()
            {
                PrivateStaticMethod1();
                ProtectedStaticMethod1();
            }
        }

        public class Type2 : Type1
        {
            public int Method2() => 42;

            public static void StaticMethod2()
            {
                ProtectedStaticMethod1();
                Console.WriteLine("StaticMethod2");
            }
        }

        [Theory]
        [InlineData(new string[] { "Method1", "StaticMethod1" }, typeof(Type1))]
        [InlineData(new string[] { "Method1", "Method2", "StaticMethod1", "StaticMethod2", }, typeof(Type2))]
        public void GetTypeMethods(string[] expected, Type input)
        {
            var methodNames = CodeGenerator.GetTypeMethods(input)
                .Select(x => x.Name)
                .OrderBy(x => x)
                .ToArray();

            Assert.Equal(expected, methodNames);
        }
    }
}
