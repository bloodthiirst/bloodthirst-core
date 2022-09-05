using System;
using System.Text;
using UnityEngine.Assertions;

namespace Bloodthirst.BJson
{
    public static class BJsonUtils
    {
        public const string NULL_IDENTIFIER = "null";
        public static readonly string NEW_LINE = Environment.NewLine;

        public static int GetElementCount(ParserState<JSONTokenType> parseState)
        {
            // check if the array is empty first
            bool isEmpty = parseState.CurrentToken.TokenType == JSONTokenType.OBJECT_START && parseState.Tokens[parseState.CurrentTokenIndex + 1].TokenType == JSONTokenType.OBJECT_END ||
                            parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_START && parseState.Tokens[parseState.CurrentTokenIndex + 1].TokenType == JSONTokenType.ARRAY_END;

            if (isEmpty)
            {
                return 0;
            }


            int elementCount = 0;

            bool elementStart = false;
            int scopeDepth = 0;

            bool isDone = false;

            while (parseState.CurrentTokenIndex < parseState.Tokens.Count && !isDone)
            {
                switch (parseState.CurrentToken.TokenType)
                {
                    case JSONTokenType.OBJECT_START:
                        {
                            scopeDepth++;
                            break;
                        }
                    case JSONTokenType.OBJECT_END:
                        {
                            scopeDepth--;
                            break;
                        }
                    case JSONTokenType.ARRAY_START:
                        {
                            scopeDepth++;
                            break;
                        }
                    case JSONTokenType.ARRAY_END:
                        {
                            scopeDepth--;
                            break;
                        }
                    case JSONTokenType.COLON:
                        {
                            // if we are still included in the "root" object scope
                            // then we make the start of an element
                            elementStart = scopeDepth == 1;
                            break;
                        }
                    case JSONTokenType.COMMA:
                        {
                            elementCount += Convert.ToInt32(scopeDepth == 1);
                            elementStart = false;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                isDone = scopeDepth == 0;
                parseState.CurrentTokenIndex++;
            }

            // if there's a prop at the end of the array
            // we add it
            elementCount += Convert.ToInt32(elementStart) + 1; // normally we should also do a +1 to include the last item , and a -1 to remove the $type entry , but i think ill let the -1 for the $type be done in the serializer

            return elementCount;
        }

        public static void NewLineAtStart(StringBuilder jsonBuilder)
        {
            bool isNotStartOfJson = jsonBuilder.Length != 0;
            int spaceCount = Convert.ToInt32(isNotStartOfJson);

            jsonBuilder.Append(NEW_LINE[0], spaceCount);
            jsonBuilder.Append(NEW_LINE[1], spaceCount);
        }

        public static Type GetConcreteTypeRef(ref ParserState<JSONTokenType> parseState)
        {
            parseState.CurrentTokenIndex++;

            while (parseState.CurrentTokenIndex < parseState.Tokens.Count)
            {
                Token<JSONTokenType> currentToken = parseState.CurrentToken;

                // skip spaces
                if (currentToken.TokenType == JSONTokenType.SPACE)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // exit if object ended
                if (currentToken.TokenType == JSONTokenType.OBJECT_END)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // skip while no identitifer
                if (currentToken.TokenType != JSONTokenType.IDENTIFIER)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // key found
                string key = currentToken.ToString();

                // skip until the first colon
                while (parseState.CurrentToken.TokenType != JSONTokenType.COLON)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // skip colon
                parseState.CurrentTokenIndex++;

                // get the type string
                currentToken = parseState.CurrentToken;

                // skip comma
                while (parseState.CurrentToken.TokenType != JSONTokenType.COMMA)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }
                parseState.CurrentTokenIndex++;

                Assert.AreEqual("$type", key);

                // now we are at the type identfier
                string typeAsString = currentToken.ToString();
                string trimmedTypeName = typeAsString.Substring(1, typeAsString.Length - 2);

                Type concreteType = Type.GetType(trimmedTypeName);

                return concreteType;
            }

            return null;
        }


        public static Type GetConcreteType(ParserState<JSONTokenType> parseState)
        {
            parseState.CurrentTokenIndex++;

            while (parseState.CurrentTokenIndex < parseState.Tokens.Count)
            {
                Token<JSONTokenType> currentToken = parseState.CurrentToken;

                // skip spaces
                if (currentToken.TokenType == JSONTokenType.SPACE)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // exit if object ended
                if (currentToken.TokenType == JSONTokenType.OBJECT_END)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // skip while no identitifer
                if (currentToken.TokenType != JSONTokenType.IDENTIFIER)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // key found
                string key = currentToken.ToString();

                // skip until the first colon
                while (parseState.CurrentToken.TokenType != JSONTokenType.COLON)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                parseState.CurrentTokenIndex++;

                currentToken = parseState.CurrentToken;

                Assert.AreEqual("$type", key);

                // now we are at the type identfier
                string typeAsString = currentToken.ToString();
                string trimmedTypeName = typeAsString.Substring(1, typeAsString.Length - 2);

                Type concreteType = Type.GetType(trimmedTypeName);

                return concreteType;
            }

            return null;
        }

        public static bool IsCachedOrNull(ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings, out object cached)
        {
            if (parseState.CurrentToken.TokenType == JSONTokenType.NULL)
            {
                cached = null;
                parseState.CurrentTokenIndex++;
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

        public static bool WriteIdFormatted(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
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

        public static bool WriteIdNonFormatted(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
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
