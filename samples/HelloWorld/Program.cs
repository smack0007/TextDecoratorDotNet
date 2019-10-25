using System;
using TextDecoratorDotNet;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                TemplateEngine.Run(
                    "Hello {{name}}!",
                    new TemplateVariables()
                    {
                        ["name"] = "World"
                    }
                )
            );
        }
    }
}
