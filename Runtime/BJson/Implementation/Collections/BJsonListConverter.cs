using Bloodthirst.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bloodthirst.BJson
{
    internal class BJsonListConverter : BJsonComplexBaseConverter
    {
        private Func<object> Constructor { get; set; }

        private Type ElementType { get; set; }

        private IBJsonConverterInternal ElementConverter { get; set; }
        public BJsonListConverter(Type t) : base(t)
        {

        }

        public override void Initialize()
        {
            Constructor = ReflectionUtils.GetParameterlessConstructor(ConvertType);
            ElementType = ConvertType.GetGenericArguments()[0];
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

            if (BJsonUtils.WriteIdFormatter(instance, jsonBuilder, context, settings))
            {
                return;
            }

            context.Register(instance);

            // start the array
            jsonBuilder.Append('[');

            // set list type
            BJsonUtils.WriteTypeInfoNonFormatted(instance, jsonBuilder, context, settings);

            IList lst = (IList)instance;

            // start writing the elemes
            foreach (object elem in lst)
            {
                Type elemType = elem.GetType();
                IBJsonConverterInternal c = Provider.GetConverter(elemType);
                c.Serialize_NonFormatted_Internal(elem, jsonBuilder, context, settings);

                jsonBuilder.Append(',');
            }

            // end the list
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

            jsonBuilder.Append(Environment.NewLine);

            IList lst = (IList)instance;

            foreach (object elem in lst)
            {
                Type elemType = elem.GetType();
                IBJsonConverterInternal c = Provider.GetConverter(elemType);
                c.Serialize_Formatted_Internal(elem, jsonBuilder, context, settings);

                jsonBuilder.Append(',');
                jsonBuilder.Append(Environment.NewLine);
            }
            context.Indentation--;
            // remove the last ','
            jsonBuilder.Remove(jsonBuilder.Length - 3, 1);
            jsonBuilder.AddIndentation(context.Indentation);
            jsonBuilder.Append(']');
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
                object elem = ElementConverter.Deserialize_Internal(null, ref parseState, context, settings);

                lst.Add(elem);

                // skip until the first comma or object end
                parseState.SkipUntil(ParserUtils.IsPropertyEndInArray);

                try
                {
                    if (parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
                    {
                        parseState.CurrentTokenIndex++;
                        break;
                    }
                }
                catch (Exception ex)
                {

                }

                parseState.CurrentTokenIndex++;
            }

            return lst;
        }


    }
}
