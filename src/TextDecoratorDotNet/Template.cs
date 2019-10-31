using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace TextDecoratorDotNet
{
    public class Template
    {
        public static async Task<Template<T>> CompileAsync<T>(
            string template,
            string[]? imports = null)
        {
            var script = CodeGenerator.Generate(template, new CodeGeneratorParameters(typeof(T)));

            var scriptOptions = ScriptOptions.Default
                .AddImports("System", "System.IO")
                .AddReferences(typeof(TemplateBase<T>).Assembly)
                .AddImports(typeof(TemplateBase<T>).Namespace)
                .AddReferences(typeof(T).Assembly);

            if (imports != null)
                scriptOptions = scriptOptions.AddImports(imports);

            var templateDelegate = await CSharpScript.EvaluateAsync<Action<TextWriter, T>>(script, scriptOptions);

            return new Template<T>(templateDelegate);
        }
    }

    public class Template<T>
    {
        private readonly Action<TextWriter, T> _template;

        internal Template(Action<TextWriter, T> template)
        {
            _template = template;
        }

        public string Run(T context, int bufferSize = 1024)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var sb = new StringBuilder(bufferSize);
            using var sw = new StringWriter(sb);
            _template.Invoke(sw, context);

            return sb.ToString();
        }
    }
}
