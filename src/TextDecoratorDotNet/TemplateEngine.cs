using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDecoratorDotNet
{
    public static class TemplateEngine
    {
        private const string OpenTag = "{{";
        private const string CloseTag = "}}";
        private const string StartSection = "#";
        private const string StartInvertedSection = "^";
        private const string EndSection = "/";

        public static string Run(string template, TemplateVariables variables)
        {
            var sb = new StringBuilder(template.Length);

            using var input = new StringReader(template);
            using var output = new StringWriter(sb);

            Run(input, variables, output);

            return sb.ToString();
        }

        public static void Run(TextReader input, TemplateVariables variables, TextWriter output)
        {
            var template = Compile(input);
            template.Run(output, variables);            
        }

        public static Template Compile(TextReader input)
        {
            int lineCount = 0;
            string line;
            var buffer = new StringBuilder(1024);

            var containers = new Stack<ContainerBlock>();
            containers.Push(new RootBlock());

            while ((line = input.ReadLine()) != null)
            {
                lineCount++;

                if (lineCount > 1)
                    buffer.AppendLine();

                int openTagIndex = line.IndexOf(OpenTag);

                if (openTagIndex != -1)
                {
                    buffer.Append(line.Substring(0, openTagIndex));
                }
                else
                {
                    buffer.Append(line);
                }

                while (openTagIndex != -1)
                {                                                  
                    FlushLiteralBlock(buffer, containers);

                    int tagStart = openTagIndex + OpenTag.Length;

                    int tagEnd = line.IndexOf(CloseTag, tagStart);

                    if (tagEnd == -1)
                        throw new TemplateException($"Missing close tag for tag starting at {tagStart}.");

                    string tag = line.Substring(tagStart, tagEnd - tagStart);

                    if (tag.StartsWith(StartSection) || tag.StartsWith(StartInvertedSection))
                    {
                        bool isInverted = tag.StartsWith(StartInvertedSection);
                        tag = tag.Substring(!isInverted ? StartSection.Length : StartInvertedSection.Length);

                        var sectionBlock = new SectionBlock(tag, isInverted);
                        containers.Peek().Blocks.Add(sectionBlock);
                        containers.Push(sectionBlock);
                    }
                    else if (tag.StartsWith(EndSection))
                    {
                        containers.Pop();
                    }
                    else
                    {
                        containers.Peek().Blocks.Add(new VariableBlock(tag));
                    }

                    tagEnd += CloseTag.Length;
                    openTagIndex = line.IndexOf(OpenTag, tagEnd);
                        
                    if (openTagIndex != -1)
                    {
                        buffer.Append(line.Substring(tagEnd, openTagIndex - tagEnd));
                    }
                    else
                    {
                        buffer.Append(line.Substring(tagEnd));
                    }
                }
            }

            FlushLiteralBlock(buffer, containers);

            return new Template((RootBlock)containers.Pop());
        }

        private static void FlushLiteralBlock(StringBuilder buffer, Stack<ContainerBlock> containers)
        {
            if (buffer.Length == 0)
                return;

            containers.Peek().Blocks.Add(new LiteralBlock(buffer.ToString()));
            
            buffer.Clear();            
        }
    }
}
