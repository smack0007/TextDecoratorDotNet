namespace TextDecoratorDotNet
{
    public class CodeGeneratorParameters
    {
        public string TemplateContextName { get; }

        public CodeGeneratorParameters(
            string templateContextName)
        {
            TemplateContextName = templateContextName;
        }

        public string ClassName { get; set; } = "MyTemplate";

        public string BaseClassName { get; set; } = "Template";

        public bool IncludeLineDirectives { get; set; } = true;
    }
}
