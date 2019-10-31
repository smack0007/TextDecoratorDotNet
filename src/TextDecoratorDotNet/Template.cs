using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace TextDecoratorDotNet
{
    public class Template
    {
        public static async Task<Action<TextWriter, T>> CompileAsync<T>(
            string template,
            string[]? imports = null)
        {
            var script = CodeGenerator.Generate(template, new CodeGeneratorParameters(typeof(T)));

            var scriptOptions = ScriptOptions.Default
                .AddImports("System", "System.IO")
                .AddReferences(typeof(Template<T>).Assembly)
                .AddImports(typeof(Template<T>).Namespace)
                .AddReferences(typeof(T).Assembly);

            if (imports != null)
                scriptOptions = scriptOptions.AddImports(imports);

            return await CSharpScript.EvaluateAsync<Action<TextWriter, T>>(script, scriptOptions);
        }
    }

    public class Template<T>
    {
        private readonly TextWriter _output;
        
        protected T _Context { get; }

        public Template(TextWriter output, T context)
        {
            _output = output;
            _Context = context;
        }

        protected void _Write(object value) => _output.Write(value);

        protected void _WriteLiteral(string literal) => _output.Write(literal);
    }
}
