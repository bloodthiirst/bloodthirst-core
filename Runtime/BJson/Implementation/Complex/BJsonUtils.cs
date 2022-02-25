using System;
using System.Text;

namespace Bloodthirst.BJson
{
    public static class BJsonUtils
    {
        public const string NULL_IDENTIFIER = "null";

        public static bool IsCachedOrNull(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings, out object cached)
        {
            // todo : check if null in JSON
            if (parseState.CurrentToken.TokenType == JSONTokenType.NULL)
            {
                cached = null;
                return true;
            }

            int tmpIndex = parseState.CurrentTokenIndex;
            tmpIndex++;

            while (tmpIndex < parseState.Tokens.Count)
            {
                Token<JSONTokenType> currentToken = parseState.Tokens[tmpIndex];

                // skip spaces
                if (currentToken.TokenType == JSONTokenType.SPACE)
                {
                    tmpIndex++;
                    continue;
                }

                // exit if object ended
                if (currentToken.TokenType == JSONTokenType.OBJECT_END)
                {
                    tmpIndex++;
                    continue;
                }

                // stp while no identitifer
                if (currentToken.TokenType != JSONTokenType.IDENTIFIER)
                {
                    tmpIndex++;
                    continue;
                }

                // key found
                string key = currentToken.ToString();

                // skip until the first colon
                while (parseState.Tokens[tmpIndex].TokenType != JSONTokenType.COLON)
                {
                    tmpIndex++;
                    continue;
                }

                tmpIndex++;

                currentToken = parseState.Tokens[tmpIndex];

                // check for caching
                if (key == "$id")
                {
                    int id = int.Parse(currentToken.ToString());
                    context.TryGetCached(id, out cached);

                    // just over the value
                    tmpIndex++;

                    // just over the ] or }
                    tmpIndex++;

                    parseState.CurrentTokenIndex = tmpIndex;

                    return true;
                }

                break;

            }


            cached = null;
            return false;
        }

        public static bool WriteIdFormatter(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (context.IsCached(instance, out int id))
            {
                jsonBuilder.Append('{');

                context.Indentation++;
                jsonBuilder.Append(Environment.NewLine);
                jsonBuilder.AddIndentation(context.Indentation);

                jsonBuilder.Append("$id : ");
                jsonBuilder.Append(id);

                context.Indentation--;
                jsonBuilder.Append(Environment.NewLine);
                jsonBuilder.AddIndentation(context.Indentation);

                jsonBuilder.Append('}');
                return true;
            }

            return false;
        }

        public static bool WriteIdNonFormatter(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (context.IsCached(instance, out int id))
            {
                jsonBuilder.Append('{');
                jsonBuilder.Append("$id:");
                jsonBuilder.Append(id);
                jsonBuilder.Append('}');
                return true;
            }

            return false;
        }

        public static void WriteTypeInfoFormatted(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            // type
            Type concreteType = instance.GetType();

            jsonBuilder.AddIndentation(context.Indentation);
            jsonBuilder.Append("$type : ");
            jsonBuilder.Append('"');
            jsonBuilder.Append(concreteType.AssemblyQualifiedName);
            jsonBuilder.Append('"');
            jsonBuilder.Append(',');
        }

        public static void WriteTypeInfoNonFormatted(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            // type
            Type concreteType = instance.GetType();

            jsonBuilder.Append("$type:");
            jsonBuilder.Append('"');
            jsonBuilder.Append(concreteType.AssemblyQualifiedName);
            jsonBuilder.Append('"');
            jsonBuilder.Append(',');
        }
    }
}
