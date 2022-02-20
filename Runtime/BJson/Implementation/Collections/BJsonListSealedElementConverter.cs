using Bloodthirst.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bloodthirst.BJson
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
            ElementConverter = Provider.GetConverter(ElementType);
        }

        public override object CreateInstance_Internal()
        {
            return Constructor();
        }

        public override void Serialize_NonFormatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (instance == null)
            {
                jsonBuilder.Append(BJsonUtils.NULL_IDENTIFIER);
                return;
            }

            if (BJsonUtils.WriteIdNonFormatter(instance, jsonBuilder, context, settings))
            {
                return;
            }

            context.Register(instance);

            jsonBuilder.Append('[');

            BJsonUtils.WriteTypeInfoNonFormatted(instance, jsonBuilder, context, settings);

            IList lst = (IList)instance;

            foreach (object elem in lst)
            {
                jsonBuilder.Append(Environment.NewLine);
                ElementConverter.Serialize_NonFormatted_Internal(elem, jsonBuilder, context, settings);

                jsonBuilder.Append(',');
            }

            jsonBuilder[jsonBuilder.Length - 1] = ']';
        }


        public override void Serialize_Formatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (instance == null)
            {
                jsonBuilder.Append(BJsonUtils.NULL_IDENTIFIER);
                return;
            }

            if (BJsonUtils.WriteIdFormatter(instance, jsonBuilder, context, settings))
            {
                return;
            }

            context.Register(instance);

            jsonBuilder.Append(Environment.NewLine);
            jsonBuilder.AddIndentation(context.Indentation);

            jsonBuilder.Append('[');

            jsonBuilder.Append(Environment.NewLine);
            context.Indentation++;

            BJsonUtils.WriteTypeInfoFormatted(instance, jsonBuilder, context, settings);

            IList lst = (IList)instance;

            foreach (object elem in lst)
            {
                jsonBuilder.Append(Environment.NewLine);
                ElementConverter.Serialize_Formatted_Internal(elem, jsonBuilder, context, settings);

                jsonBuilder.Append(',');
            }

            jsonBuilder[jsonBuilder.Length - 1] = ']';
        }

        public override object Deserialize_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            if (BJsonUtils.IsCachedOrNull(instance, ref parseState, context, settings, out object cached))
            {
                return cached;
            }

            // skip [
            parseState.CurrentTokenIndex++;

            // skip $type
            // OR if the array is empty , then just skip to the end of the array
            // so basically skip until the first ',' or ']'
            parseState.SkipUntil(ParserUtils.IsPropertyEndInArray);


            IList lst = null;

            if (instance == null)
            {
                lst = (IList)Constructor();
            }
            else
            {
                lst = (IList)instance;
                lst.Clear();
            }

            context.Register(lst);

            if (parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
            {
                parseState.CurrentTokenIndex++;
                return lst;
            }

            // skip the comma
            parseState.CurrentTokenIndex++;


            while (parseState.CurrentTokenIndex < parseState.Tokens.Count)
            {
                object elem = ElementConverter.Deserialize_Internal(instance , ref parseState, context, settings);
                
                lst.Add(elem);
                
                // skip until the first comma or object end
                parseState.SkipUntil(ParserUtils.IsPropertyEndInArray);
                
                if(parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
                {
                    parseState.CurrentTokenIndex++;
                    break;
                }

                parseState.CurrentTokenIndex++;
            }

            return lst;
        }


    }
}
