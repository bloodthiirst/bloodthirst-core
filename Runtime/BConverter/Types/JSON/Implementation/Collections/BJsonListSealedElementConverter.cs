using Bloodthirst.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bloodthirst.BDeepCopy
{
    internal class BJsonListSealedElementConverter : BJsonComplexBaseConverter
    {
        private Func<object> Constructor { get; set; }

        private Type ElementType { get; set; }

        private IBJsonConverterInternal ElementConverter { get; set; }
        public BJsonListSealedElementConverter(Type t) : base(t)
        {
       
        }

        public override void Initialize()
        {
            ElementType = ConvertType.GetGenericArguments()[0];
            Constructor = ReflectionUtils.GetParameterlessConstructor(ConvertType);
            ElementConverter = (IBJsonConverterInternal)Provider.Get(ElementType);
        }

        public override object CreateInstance_Internal()
        {
            return Constructor();
        }

        public override void To_Internal(object instance, StringBuilder jsonBuilder, BConverterContext context, BConverterSettings settings)
        {
            jsonBuilder.Append('[');

            IList lst = (IList)instance;

            foreach (object elem in lst)
            {
                ElementConverter.To_Internal(elem, jsonBuilder, context, settings);

                jsonBuilder.Append(',');
            }

            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);

            jsonBuilder.Append(']');
        }

        public override object From_Internal(object instance, ref ParserState<JSONTokenType> parseState, BConverterContext context, BConverterSettings settings)
        {
            // skip the first object start
            parseState.CurrentTokenIndex++;

            IList lst = null;

            if (instance == null)
            {
                lst = (IList)Constructor();
            }
            else
            {
                lst = (IList)instance;
            }

            while (parseState.CurrentTokenIndex < parseState.Tokens.Count)
            {
                object elem = ElementConverter.From_Internal(instance , ref parseState, context, settings);
                
                lst.Add(elem);
                
                // skip until the first comma or object end
                parseState.SkipUntil(IsPropertyEnd);
                
                if(parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
                {
                    parseState.CurrentTokenIndex++;
                    break;
                }

                parseState.CurrentTokenIndex++;
            }

            return lst;
        }

        #region utils
        private static bool IsPropertyEnd(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.ARRAY_END || token.TokenType == JSONTokenType.COMMA;
        }

        #endregion


    }
}
