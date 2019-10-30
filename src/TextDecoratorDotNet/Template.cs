using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace TextDecoratorDotNet
{
    public class Template
    {
        public static async Task<Action<TextWriter, T>> CompileAsync<T>(string template)
            where T: TemplateContext
        {
            var script = CodeGenerator.Generate(template, new CodeGeneratorParameters(typeof(T).FullName));

            var scriptOptions = ScriptOptions.Default
                .AddImports("System", "System.IO")
                .AddReferences(typeof(Template<T>).Assembly)
                .AddImports(typeof(Template<T>).Namespace)
                .AddReferences(typeof(T).Assembly);

            return await CSharpScript.EvaluateAsync<Action<TextWriter, T>>(script.Code, scriptOptions);
        }
    }

    public class Template<T> where T: TemplateContext
    {
        private readonly TextWriter _output;
        protected readonly T _context;

        public Template(TextWriter output, T context)
        {
            _output = output;
            _context = context;
        }

        public void Write(string value) => _output.Write(value);

        public void WriteLiteral(string literal) => _output.Write(literal);
    }
}
