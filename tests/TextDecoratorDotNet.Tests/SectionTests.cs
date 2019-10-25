using Xunit;

namespace TextDecoratorDotNet.Tests
{
    public class SectionTests
    {
        [Fact]
        public void Not_Rendered_Section()
        {
            string template = @"Hello {{#showNames}}Bob and Joe{{/showNames}}!";

            string expected = @"Hello !";

            Assert.Equal(
                expected,
                TemplateEngine.Run(
                    template,
                    new TemplateVariables()
                    {
                        ["showNames"] = false,
                    }));
        }

        [Fact]
        public void Rendered_Section()
        {
            string template = @"Hello {{#showNames}}Bob and Joe{{/showNames}}!";

            string expected = @"Hello Bob and Joe!";

            Assert.Equal(
                expected,
                TemplateEngine.Run(
                    template,
                    new TemplateVariables()
                    {
                        ["showNames"] = true,
                    }));
        }

        [Fact]
        public void Rendered_Section_Multiple_Lines()
        {
            string template = @"Hello
{{#showNames}}Bob
and
Joe{{/showNames}}!";

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
                        ["showNames"] = true,
                    }));
        }
    }
}
