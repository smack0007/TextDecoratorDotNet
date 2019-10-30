﻿using System;
using System.Text;

namespace TextDecoratorDotNet
{
    public static class CodeGenerator
    {
        enum BufferContents
        {
            Literal,

            Expression
        }

        public static CodeGeneratorResult Generate(string template, CodeGeneratorParameters parameters)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            StringBuilder output = new StringBuilder();
            CodeGeneratorResult result = new CodeGeneratorResult();

            GenerateScript(output, template, parameters, result);

            result.Code = output.ToString();

            return result;
        }

        private static void GenerateScript(
            StringBuilder output,
            string template,
            CodeGeneratorParameters parameters,
            CodeGeneratorResult result)
        {
            StringBuilder methodOutput = new StringBuilder();

            int lineNumber = 1;

            GenerateMethodBody(
                methodOutput,
                template,
                parameters,
                0,
                template.Length - 1,
                ref lineNumber,
                result);

            output.AppendLine($@"class ScriptTemplate : Template<{parameters.TemplateContextName}>
{{
    public ScriptTemplate(TextWriter output, {parameters.TemplateContextName} context) : base(output, context) {{ }}

    public string Name => _context.Name;

    public void __Run__()
    {{
{methodOutput}
    }}
}}
return new Action<TextWriter, {parameters.TemplateContextName}>((output, context) =>
{{
    var template = new ScriptTemplate(output, context);
    template.__Run__();
}});");
        }

        private static void GenerateMethodBody(
            StringBuilder output,
            string template,
            CodeGeneratorParameters parameters,
            int start,
            int end,
            ref int lineNumber,
            CodeGeneratorResult result)
        {
            StringBuilder buffer = new StringBuilder(1024);

            int i = start;
            while (i <= end)
            {
                if (template[i] == '@')
                {
                    if (LookAhead(template, i, "@@"))
                    {
                        buffer.Append("@");
                        i += 2;
                        continue;
                    }

                    if (buffer.Length != 0)
                    {
                        string bufferContents = buffer.ToString();

                        FlushBuffer(
                            parameters,
                            BufferContents.Literal,
                            buffer,
                            output,
                            lineNumber);

                        lineNumber += CountOccurences(bufferContents, 0, bufferContents.Length, Environment.NewLine);
                    }

                    i++;
                    int beforePos = i;

                    if (LookAhead(template, i, "{"))
                    {
                        ParseCodeBlock(template, parameters, ref i, ref lineNumber, buffer, output);
                    }
                    else if (LookAhead(template, i, "for"))
                    {
                        ParseLogicBlock("for", template, parameters, ref i, ref lineNumber, buffer, output, result);
                    }
                    else if (LookAhead(template, i, "foreach"))
                    {
                        ParseLogicBlock("foreach", template, parameters, ref i, ref lineNumber, buffer, output, result);
                    }
                    else if (LookAhead(template, i, "if"))
                    {
                        ParseLogicBlock("if", template, parameters, ref i, ref lineNumber, buffer, output, result);
                    }
                    else if (LookAhead(template, i, "property"))
                    {
                        ParseDeclaration("property", template, parameters, ref i, ref lineNumber, buffer, output, (x) => result.Properties.Add(x));
                    }
                    else if (LookAhead(template, i, "using"))
                    {
                        ParseDeclaration("using", template, parameters, ref i, ref lineNumber, buffer, output, (x) => result.Usings.Add(x));
                    }
                    else if (LookAhead(template, i, "var"))
                    {
                        ParseDeclaration("var", template, parameters, ref i, ref lineNumber, buffer, output, (x) => output.AppendLine($"{x};"));
                    }
                    else if (LookAhead(template, i, "while"))
                    {
                        ParseLogicBlock("while", template, parameters, ref i, ref lineNumber, buffer, output, result);
                    }
                    else
                    {
                        ParseExpression(template, parameters, ref i, ref lineNumber, buffer, output);
                    }

                    lineNumber += CountOccurences(template, beforePos, i, Environment.NewLine);
                }
                else
                {
                    buffer.Append(template[i]);
                    i++;
                }
            }

            FlushBuffer(
                parameters,
                BufferContents.Literal,
                buffer,
                output,
                lineNumber);
        }

        private static int CountOccurences(string template, int start, int end, string keyword)
        {
            int count = 0;

            for (int i = start; i <= end; i++)
            {
                if (LookAhead(template, i, keyword))
                    count++;
            }

            return count;
        }

        private static bool LookAhead(string template, int i, string keyword)
        {
            if (i + keyword.Length > template.Length)
                return false;

            for (int j = 0; j < keyword.Length; j++)
            {
                if (template[i + j] != keyword[j])
                    return false;
            }

            return true;
        }

        private static int FindNext(string template, int i, string keyword)
        {
            for (int j = i; j < template.Length; j++)
            {
                if (LookAhead(template, j, keyword))
                    return j;
            }

            return -1;
        }

        private static int FindClosingParen(string template, int i)
        {
            int depth = 1;

            for (int j = i + 1; j < template.Length; j++)
            {
                if (template[j] == '(')
                {
                    depth++;
                }
                else if (template[j] == ')')
                {
                    depth--;

                    if (depth <= 0)
                        return j;
                }
            }

            return -1;
        }

        private static int FindClosingCurlyBrace(string template, int i)
        {
            int depth = 1;

            for (int j = i + 1; j < template.Length; j++)
            {
                if (template[j] == '{')
                {
                    depth++;
                }
                else if (template[j] == '}')
                {
                    depth--;

                    if (depth <= 0)
                        return j;
                }
            }

            return -1;
        }

        private static void AppendLine(
            StringBuilder output,
            string template,
            int start,
            int end)
        {
            for (int i = start; i <= end; i++)
                output.Append(template[i]);

            output.Append(Environment.NewLine);
        }

        private static void FlushBuffer(
            CodeGeneratorParameters parameters,
            BufferContents contents,
            StringBuilder buffer,
            StringBuilder output,
            int lineNumber)
        {
            if (parameters.IncludeLineDirectives)
                output.AppendLine($"#line {lineNumber}");

            if (buffer.Length > 0)
            {
                switch (contents)
                {
                    case BufferContents.Literal:
                        string literal = buffer.ToString().Replace("\"", "\"\"");
                        output.AppendLine($"WriteLiteral(@\"{literal}\");");
                        break;

                    case BufferContents.Expression:
                        output.AppendLine($"Write({buffer.ToString()});");
                        break;
                }

                buffer.Clear();
            }
        }

        private static void ParseCodeBlock(
            string template,
            CodeGeneratorParameters parameters,
            ref int i,
            ref int lineNumber,
            StringBuilder buffer,
            StringBuilder output)
        {
            int closeCurlyBrace = FindClosingCurlyBrace(template, i);

            if (closeCurlyBrace == -1)
                throw new TextDecoratorDotNetException("Matching } not found while parsing code block.");

            int startCode = i + 1;
            int endCode = closeCurlyBrace - 1;

            if (startCode < endCode)
                AppendLine(output, template, startCode, endCode);

            i = closeCurlyBrace + 1;
        }

        private static void ParseLogicBlock(
            string type,
            string template,
            CodeGeneratorParameters parameters,
            ref int i,
            ref int lineNumber,
            StringBuilder buffer,
            StringBuilder output,
            CodeGeneratorResult result)
        {
            if (parameters.IncludeLineDirectives)
                output.AppendLine($"#line {lineNumber}");

            int openParen = FindNext(template, i, "(");

            if (openParen == -1)
                throw new TextDecoratorDotNetException(string.Format("Expected ( while parsing {0}.", type));

            int closeParen = FindClosingParen(template, openParen);

            if (closeParen == -1)
                throw new TextDecoratorDotNetException(string.Format("Matching ) not found while parsing {0}.", type));

            int openCurlyBrace = FindNext(template, closeParen, "{");

            if (openCurlyBrace == -1)
                throw new TextDecoratorDotNetException(string.Format("Expected {{ while parsing {0}.", type));

            int closeCurlyBrace = FindClosingCurlyBrace(template, openCurlyBrace);

            if (closeCurlyBrace == -1)
                throw new TextDecoratorDotNetException(string.Format("Matching }} not found while parsing {0}.", type));

            output.Append(' '); // Append a single space to compensate for the @ sign.
            AppendLine(output, template, i, openCurlyBrace);

            int startCode = openCurlyBrace + 1;
            int endCode = closeCurlyBrace - 1;

            while (startCode < template.Length && LookAhead(template, startCode, Environment.NewLine))
            {
                startCode += Environment.NewLine.Length;
                lineNumber++;
            }

            if (startCode < endCode)
            {
                GenerateMethodBody(
                    output,
                    template,
                    parameters,
                    startCode,
                    endCode,
                    ref lineNumber,
                    result);
            }

            output.AppendLine("}");

            i = closeCurlyBrace + 1;
        }

        private static void ParseExpression(
            string template,
            CodeGeneratorParameters parameters,
            ref int i,
            ref int lineNumber,
            StringBuilder buffer,
            StringBuilder output)
        {
            bool finished = false;

            while (!finished)
            {
                if (!char.IsLetterOrDigit(template[i]))
                {
                    finished = true;

                    if (template[i] == '.')
                    {
                        buffer.Append(template[i]);
                        i++;
                        finished = false;
                    }
                    else if (template[i] == '[')
                    {
                        ParseIndexer(template, parameters, ref i, ref lineNumber, buffer);
                        finished = false;
                    }
                }
                else
                {
                    buffer.Append(template[i]);
                    i++;
                }

                if (i >= template.Length)
                    finished = true;
            }

            FlushBuffer(
                parameters,
                BufferContents.Expression,
                buffer,
                output,
                lineNumber);
        }

        private static void ParseIndexer(
            string template,
            CodeGeneratorParameters parameters,
            ref int i,
            ref int lineNumber,
            StringBuilder buffer)
        {
            int depth = 1;

            buffer.Append(template[i]);
            i++;

            while (depth > 0 && i < template.Length)
            {
                if (template[i] == '[')
                {
                    depth++;
                }
                else if (template[i] == ']')
                {
                    depth--;
                }

                buffer.Append(template[i]);
                i++;
            }
        }

        private static void ParseDeclaration(
            string type,
            string template,
            CodeGeneratorParameters parameters,
            ref int i,
            ref int lineNumber,
            StringBuilder buffer,
            StringBuilder output,
            Action<string> action)
        {
            int endOfLine = FindNext(template, i, "\n");
            int newLineLength = 1;

            if (endOfLine == -1)
                endOfLine = template.Length;

            if (template[endOfLine - 1] == '\r')
            {
                endOfLine--;
                newLineLength++;
            }

            string content = template.Substring(i + type.Length + 1, endOfLine - i - type.Length - 1);
            action(content);

            i = endOfLine + newLineLength;
        }
    }
}