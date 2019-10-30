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

        public int Age { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var template = await Template.CompileAsync<HelloWorldTemplateContext>(
                "Hello @Name you are @Age years old!"
            );

            var sb = new StringBuilder(1024);

            template.Invoke(new StringWriter(sb), new HelloWorldTemplateContext { Name = "Foo", Age = 35 });
            sb.AppendLine();

            template.Invoke(new StringWriter(sb), new HelloWorldTemplateContext { Name = "Bar", Age = 42 });
            sb.AppendLine();

            Console.Write(sb.ToString());

            Console.ReadKey();
        }
    }
}
