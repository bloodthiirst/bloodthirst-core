using Bloodthirst.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bloodthirst.BDeepCopy
{
    internal class BJsonDictionaryConverter : BJsonComplexBaseConverter
    {
        private Func<object> Constructor { get; set; }

        private Type KeyType { get; set; }
        private Type ValueType { get; set; }

        private IBJsonConverterInternal KeyConverter { get; set; }
        private IBJsonConverterInternal ValueConverter { get; set; }
        public BJsonDictionaryConverter(Type t) : base(t)
        {
       
        }

        public override void Initialize()
        {
            KeyType = ConvertType.GetGenericArguments()[0];
            ValueType = ConvertType.GetGenericArguments()[1];
            Constructor = ReflectionUtils.GetParameterlessConstructor(ConvertType);
            KeyConverter = (IBJsonConverterInternal)Provider.Get(KeyType);
            ValueConverter = (IBJsonConverterInternal)Provider.Get(ValueType);
        }

        public override object CreateInstance_Internal()
        {
            return Constructor();
        }

        public override void To_Internal(object instance, StringBuilder jsonBuilder, BConverterContext context, BConverterSettings settings)
        {
            throw new NotImplementedException();
        }

        public override object From_Internal(object instance, ref ParserState<JSONTokenType> parseState, BConverterContext context, BConverterSettings settings)
        {
            // skip the first array start
            parseState.CurrentTokenIndex++;

            IDictionary dict = null;

            if (instance == null)
            {
                dict = (IDictionary)Constructor();
            }
            else
            {
                dict = (IDictionary)instance;
            }

            while (parseState.CurrentTokenIndex < parseState.Tokens.Count)
            {
                // skip the pair object start
                parseState.CurrentTokenIndex++;

                // get key
                object defaultKey = KeyConverter.CreateInstance_Internal();
                object key = KeyConverter.From_Internal(defaultKey, ref parseState, context, settings);
                
                // skip the comma
                parseState.CurrentTokenIndex++;

                // get value
                object defaultVal = ValueConverter.CreateInstance_Internal();
                object val = ValueConverter.From_Internal(defaultVal, ref parseState, context, settings);

                // add
                dict.Add(key, val);

                // skip until the first comma or object end
                parseState.SkipUntil(IsPropertyEnd);
                
                if(parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
                {
                    parseState.CurrentTokenIndex++;
                    break;
                }

                parseState.CurrentTokenIndex++;
            }

            return dict;
        }

        #region utils
        private static bool IsPropertyEnd(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.ARRAY_END || token.TokenType == JSONTokenType.COMMA;
        }

        #endregion


    }
}
