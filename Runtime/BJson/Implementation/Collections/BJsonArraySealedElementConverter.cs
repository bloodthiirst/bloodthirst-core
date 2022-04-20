using Bloodthirst.Core.Utils;
using System;
using System.Collections;
using System.Text;

namespace Bloodthirst.BJson
{
    internal class BJsonArraySealedElementConverter : BJsonComplexBaseConverter
    {
        private Func<object> Constructor { get; set; }
        private Type ElementType { get; set; }

        private IBJsonConverterInternal ElementConverter { get; set; }
        public BJsonArraySealedElementConverter(Type t) : base(t)
        {

        }

        public override void Initialize()
        {
            Constructor = ReflectionUtils.GetParameterlessConstructor(ConvertType);
            ElementType = ConvertType.GetElementType();
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

            if (BJsonUtils.WriteIdNonFormatted(instance, jsonBuilder, context, settings))
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

            if (BJsonUtils.WriteIdFormatted(instance, jsonBuilder, context, settings))
            {
                return;
            }
            context.Register(instance);

            BJsonUtils.NewLineAtStart(jsonBuilder);
            jsonBuilder.AddIndentation(context.Indentation);

            jsonBuilder.Append('[');

            jsonBuilder.Append(Environment.NewLine);
            context.Indentation++;

            BJsonUtils.WriteTypeInfoFormatted(instance, jsonBuilder, context, settings);

            jsonBuilder.Append(Environment.NewLine);

            IList lst = (IList)instance;

            foreach (object elem in lst)
            {
                jsonBuilder.AddIndentation(context.Indentation);

                ElementConverter.Serialize_Formatted_Internal(elem, jsonBuilder, context, settings);

                jsonBuilder.Append(',');
                jsonBuilder.Append(Environment.NewLine);
            }

            context.Indentation--;

            // remove the last ','
            jsonBuilder.Remove(jsonBuilder.Length - 3, 1);
            jsonBuilder.AddIndentation(context.Indentation);
            jsonBuilder.Append(']');
        }

        public override object Deserialize_Internal(ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            if (BJsonUtils.IsCachedOrNull(ref parseState, context, settings, out object cached))
            {
                return cached;
            }

            ParserState<JSONTokenType> parseStateCpy = parseState;

            int elementsCount = BJsonUtils.GetElementCount(parseStateCpy) - 1;

            // skip [
            parseState.CurrentTokenIndex++;

            // skip $type
            // OR if the array is empty , then just skip to the end of the array
            // so basically skip until the first ',' or ']'
            parseState.SkipUntil(ParserUtils.IsPropertyEndInArray);

            Array resultArray = CreateArray(elementsCount);

            int registerId = context.Register(resultArray);


            if (parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
            {
                parseState.CurrentTokenIndex++;
                return resultArray;
            }

            // skip the comma
            parseState.CurrentTokenIndex++;

            int currentIndex = 0;

            while (parseState.CurrentTokenIndex < parseState.Tokens.Count)
            {
                object elem = ElementConverter.Deserialize_Internal(ref parseState, context, settings);

                resultArray.SetValue(elem, currentIndex);
                currentIndex++;

                // skip until the first comma or object end
                parseState.SkipUntil(ParserUtils.IsPropertyEndInArray);

                if (parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
                {
                    parseState.CurrentTokenIndex++;
                    break;
                }

                parseState.CurrentTokenIndex++;
            }


            return resultArray;
        }
        public override object Populate_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            if (BJsonUtils.IsCachedOrNull(ref parseState, context, settings, out object cached))
            {
                return cached;
            }

            ParserState<JSONTokenType> parseStateCpy = parseState;

            int elementsCount = BJsonUtils.GetElementCount(parseStateCpy) - 1;

            // skip [
            parseState.CurrentTokenIndex++;

            // skip $type
            // OR if the array is empty , then just skip to the end of the array
            // so basically skip until the first ',' or ']'
            parseState.SkipUntil(ParserUtils.IsPropertyEndInArray);

            Array originalArray = (Array)instance;
            Array resultArray = originalArray;

            if (originalArray == null || originalArray.Length != elementsCount)
            {
                resultArray = CreateArray(elementsCount);
            }

            int registerId = context.Register(resultArray);


            if (parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
            {
                parseState.CurrentTokenIndex++;
                return resultArray;
            }

            // skip the comma
            parseState.CurrentTokenIndex++;

            int currentIndex = 0;

            while (parseState.CurrentTokenIndex < parseState.Tokens.Count)
            {
                if (!settings.HasCustomConverter(ElementType, out IBJsonConverterInternal cnv))
                {
                    cnv = ElementConverter;
                }

                object currElement = null;

                if (originalArray != null && currentIndex < originalArray.Length)
                {
                    currElement = originalArray.GetValue(currentIndex);
                }

                object elem = cnv.Populate_Internal(currElement, ref parseState, context, settings);

                resultArray.SetValue(elem, currentIndex);
                currentIndex++;

                // skip until the first comma or object end
                parseState.SkipUntil(ParserUtils.IsPropertyEndInArray);

                if (parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
                {
                    parseState.CurrentTokenIndex++;
                    break;
                }

                parseState.CurrentTokenIndex++;
            }


            return resultArray;
        }

        private Array CreateArray(int length)
        {
            Array arr = Array.CreateInstance(ElementType, length);

            return arr;
        }


    }
}
