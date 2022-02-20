using Bloodthirst.BType;
using Bloodthirst.Core.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Assertions;

namespace Bloodthirst.BJson
{
    internal class BJsonInterfaceOrAbstractConverter : BJsonComplexBaseConverter
    {
        private IBJsonConverterInternal TypeConverter { get; set; }
        public BJsonInterfaceOrAbstractConverter(Type t) : base(t)
        {

        }

        public override void Initialize()
        {
            TypeConverter = Provider.GetConverter(typeof(Type));
        }

        public override object CreateInstance_Internal()
        {
            return null;
        }

        public override void Serialize_NonFormatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (instance == null)
            {
                jsonBuilder.Append(BJsonUtils.NULL_IDENTIFIER);
                return;
            }

            Type t = instance.GetType();

            IBJsonConverterInternal c = Provider.GetConverter(t);

            c.Serialize_NonFormatted_Internal(instance, jsonBuilder, context, settings);
        }

        public override void Serialize_Formatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (instance == null)
            {
                jsonBuilder.Append(BJsonUtils.NULL_IDENTIFIER);
                return;
            }

            Type t = instance.GetType();

            IBJsonConverterInternal c = Provider.GetConverter(t);

            c.Serialize_Formatted_Internal(instance, jsonBuilder, context, settings);
        }

        public override object Deserialize_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            // todo : check if null in JSON
            if(parseState.CurrentToken.TokenType == JSONTokenType.NULL)
            {
                return null;
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
                    context.TryGetCached(id, out var cached);

                    // todo : make sure we do the "jump over" thing in the rest of the converters
                    // just over the value
                    tmpIndex++;
                    // just over the }
                    tmpIndex++;

                    parseState.CurrentTokenIndex = tmpIndex;

                    return cached;
                }

                Assert.AreEqual("$type" , key);

                // now we are at the type identfier
                string typeAsString = currentToken.ToString();
                string trimmedTypeName = typeAsString.Substring(1, typeAsString.Length - 2);

                Type concreteType = Type.GetType(trimmedTypeName);

                IBJsonConverterInternal c =  Provider.GetConverter(concreteType);

                object actualInstance = c.Deserialize_Internal(instance, ref parseState, context, settings);

                return actualInstance;
            }

            throw new InvalidOperationException($"Couldn't correctly serialize the Interface or Abstract instance of type {ConvertType.FullName}");
        }


    }
}
