using System;
using System.Threading.Tasks;
using TextDecoratorDotNet;

namespace HelloWorld
{
    public class HelloWorldTemplateParams
    {
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            var template = await Template.CompileAsync<HelloWorldTemplateParams>(
                "Hello @Name you are @Age years old!"
            );

            Console.WriteLine(template.Run(new HelloWorldTemplateParams { Name = "Foo", Age = 35 }));

            Console.WriteLine(template.Run(new HelloWorldTemplateParams { Name = "Bar", Age = 42 }));

            Console.ReadKey();
        }
    }
}
