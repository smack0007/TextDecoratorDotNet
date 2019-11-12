using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static string Generate(string template, CodeGeneratorParameters parameters)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            StringBuilder output = new StringBuilder(1024);

            GenerateScript(output, template, parameters);

            return output.ToString();
        }

        public static string GetTypeString(Type type)
        {
            if (type == typeof(void))
                return "void";

            string name = type.Name;

            if (type.IsGenericType)
            {
                name = name.Substring(0, name.IndexOf('`'));
                name += "<";

                bool first = true;
                foreach (var arg in type.GetGenericArguments())
                {
                    name += GetTypeString(arg);

                    if (!first)
                        name += ", ";

                    first = false;
                }

                name += ">";
            }

            return type.Namespace + "." + name;
        }

        public static IEnumerable<PropertyInfo> GetTypeProperties(Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.GetMethod?.IsPublic ?? false && !x.IsSpecialName && x.DeclaringType != typeof(object));
        }

        public static IEnumerable<MethodInfo> GetTypeMethods(Type type)
        {
            return type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(x => !x.IsSpecialName && x.DeclaringType != typeof(object));
        }

        private static void GenerateScript(
            StringBuilder output,
            string template,
            CodeGeneratorParameters parameters)
        {
            StringBuilder methodOutput = new StringBuilder(1024);

            int lineNumber = 1;

            GenerateMethodBody(
                methodOutput,
                template,
                parameters,
                0,
                template.Length - 1,
                ref lineNumber);

            output.AppendLine($@"class ScriptTemplate : TemplateBase<{parameters.TemplateContextType.FullName}> {{");
            output.AppendLine($"\tpublic ScriptTemplate(TextWriter output, {parameters.TemplateContextType.FullName} context) : base(output, context) {{ }}");
            
            foreach (var property in GetTypeProperties(parameters.TemplateContextType))
            {
                output.Append($"\tprivate {GetTypeString(property.PropertyType)} {property.Name} {{ get => _Context.{property.Name}; ");

                if (property.SetMethod?.IsPublic ?? false)
                {
                    output.Append($"set => _Context.{property.Name} = value; ");
                }

                output.AppendLine("}");
            }

            foreach (var method in GetTypeMethods(parameters.TemplateContextType))
            {
                var methodParams = method.GetParameters();
                var methodParamsDeclaration = string.Join(", ", method.GetParameters().Select(x => $"{GetTypeString(x.ParameterType)} {x.Name}"));
                var methodParamsList = string.Join(", ", method.GetParameters().Select(x => x.Name));

                if (!method.IsStatic)
                {
                    output.AppendLine($"\tprivate {GetTypeString(method.ReturnType)} {method.Name}({methodParamsDeclaration}) => _Context.{method.Name}({methodParamsList});");
                }
                else
                {
                    output.AppendLine($"\tprivate static {GetTypeString(method.ReturnType)} {method.Name}({methodParamsDeclaration}) => {GetTypeString(method.DeclaringType)}.{method.Name}({methodParamsList});");
                }
            }

            output.AppendLine("\tpublic void _Run() {");
            output.Append(methodOutput.ToString());
            output.AppendLine("\t}");
            output.AppendLine("}");

            output.AppendLine();

            output.AppendLine($"return new Action<TextWriter, {parameters.TemplateContextType.FullName}>((output, context) => {{");
            output.AppendLine("\tvar template = new ScriptTemplate(output, context);");
            output.AppendLine("\ttemplate._Run();");
            output.AppendLine("});");
        }

        private static void GenerateMethodBody(
            StringBuilder output,
            string template,
            CodeGeneratorParameters parameters,
            int start,
            int end,
            ref int lineNumber)
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

                    if (LookAhead(template, i, "("))
                    {
                        ParseExpressionBlock(template, ref i, buffer, '(', ')');

                        FlushBuffer(
                            parameters,
                            BufferContents.Expression,
                            buffer,
                            output,
                            lineNumber);
                    }
                    else if (LookAhead(template, i, "{"))
                    {
                        ParseCodeBlock(template, parameters, ref i, ref lineNumber, output);
                    }
                    else if (LookAhead(template, i, "for"))
                    {
                        ParseLogicBlock("for", template, parameters, ref i, ref lineNumber, output);
                    }
                    else if (LookAhead(template, i, "foreach"))
                    {
                        ParseLogicBlock("foreach", template, parameters, ref i, ref lineNumber, output);
                    }
                    else if (LookAhead(template, i, "if"))
                    {
                        ParseLogicBlock("if", template, parameters, ref i, ref lineNumber, output);
                    }
                    else if (LookAhead(template, i, "while"))
                    {
                        ParseLogicBlock("while", template, parameters, ref i, ref lineNumber, output);
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
                        output.AppendLine($"_WriteLiteral(@\"{literal}\");");
                        break;

                    case BufferContents.Expression:
                        output.AppendLine($"_Write({buffer.ToString()});");
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
            StringBuilder output)
        {
            if (parameters.IncludeLineDirectives)
                output.AppendLine($"#line {lineNumber}");

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
            StringBuilder output)
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
                    ref lineNumber);
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
                    else if (template[i] == '(')
                    {
                        ParseExpressionBlock(template, ref i, buffer, '(', ')');
                        finished = false;
                    }
                    else if (template[i] == '[')
                    {
                        ParseExpressionBlock(template, ref i, buffer, '[', ']');
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

        private static void ParseExpressionBlock(
            string template,
            ref int i,
            StringBuilder buffer,
            char startChar,
            char endChar)
        {
            int depth = 1;

            buffer.Append(template[i]);
            i++;

            while (depth > 0 && i < template.Length)
            {
                if (template[i] == startChar)
                {
                    depth++;
                }
                else if (template[i] == endChar)
                {
                    depth--;
                }

                buffer.Append(template[i]);
                i++;
            }
        }
    }
}
