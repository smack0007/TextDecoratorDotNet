using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TextDecoratorDotNet;

namespace HelloWorld
{   
    public class HelloWorldTemplateContext : TemplateContext
    {
        public string Name { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var template = await Template.CompileAsync<HelloWorldTemplateContext>(
                "Hello @Name!"
            );

            var sb = new StringBuilder(1024);

            template.Invoke(new StringWriter(sb), new HelloWorldTemplateContext { Name = "Foo" });
            sb.AppendLine();

            template.Invoke(new StringWriter(sb), new HelloWorldTemplateContext { Name = "Bar" });
            sb.AppendLine();

            Console.Write(sb.ToString());

            Console.ReadKey();
        }
    }
}
