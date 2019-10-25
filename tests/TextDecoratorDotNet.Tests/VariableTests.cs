using Xunit;

namespace TextDecoratorDotNet.Tests
{    
    public class VariableTests
    {
        [Fact]
        public void No_Variables()
        {
            Assert.Equal(
                "Hello World!",
                TemplateEngine.Run(
                    "Hello World!",
                    new TemplateVariables()));
        }

        [Fact]
        public void Single_Variable()
        {
            Assert.Equal(
                "Hello Bob!",
                TemplateEngine.Run(
                    "Hello {{Name}}!",
                    new TemplateVariables()
                    {
                        ["Name"] = "Bob"
                    }));
        }

        [Fact]
        public void Two_Variables()
        {
            Assert.Equal(
                "Hello Bob and Joe!",
                TemplateEngine.Run(
                    "Hello {{Name1}} and {{Name2}}!",
                    new TemplateVariables()
                    {
                        ["Name1"] = "Bob",
                        ["Name2"] = "Joe"
                    }));
        }

        [Fact]
        public void No_Variables_Multiple_Lines()
        {
            string template = @"Hello
Bob
and
Joe!";

            string expected = @"Hello
Bob
and
Joe!";

            Assert.Equal(
                expected,
                TemplateEngine.Run(
                    template,
                    new TemplateVariables()
                    {
                        ["Name1"] = "Bob",
                        ["Name2"] = "Joe"
                    }));
        }

        [Fact]
        public void Two_Variables_Multiple_Lines()
        {
            string template = @"Hello
{{Name1}}
and
{{Name2}}!";

            string expected = @"Hello
Bob
and
Joe!";

            Assert.Equal(
                expected,
                TemplateEngine.Run(
                    template,
                    new TemplateVariables()
                    {
                        ["Name1"] = "Bob",
                        ["Name2"] = "Joe"
                    }));
        }
    }
}
