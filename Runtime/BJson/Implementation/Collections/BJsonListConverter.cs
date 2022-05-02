using Bloodthirst.Core.Utils;
using System;
using System.Collections;
using System.Text;

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

            if (BJsonUtils.WriteIdFormatted(instance, jsonBuilder, context, settings))
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
                if (elem == null)
                {
                    jsonBuilder.Append(BJsonUtils.NULL_IDENTIFIER);
                }
                else
                {
                    Type elemType = elem.GetType();

                    if (!settings.HasCustomConverter(ElementType, out IBJsonConverterInternal cnv))
                    {
                        cnv = Provider.GetConverter(ElementType);
                    }

                    cnv.Serialize_NonFormatted_Internal(elem, jsonBuilder, context, settings);
                }

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

                if (elem == null)
                {
                    jsonBuilder.Append(BJsonUtils.NULL_IDENTIFIER);
                }
                else
                {
                    Type elemType = elem.GetType();

                    if (!settings.HasCustomConverter(ElementType, out IBJsonConverterInternal cnv))
                    {
                        cnv = Provider.GetConverter(ElementType);
                    }

                    cnv.Serialize_Formatted_Internal(elem, jsonBuilder, context, settings);
                }

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

            // skip [
            parseState.CurrentTokenIndex++;

            // skip $type
            // OR if the array is empty , then just skip to the end of the array
            // so basically skip until the first ',' or ']'
            parseState.SkipUntil(ParserUtils.IsPropertyEndInArray);

            IList resultList = CreateEmpty();

            int registerId = context.Register(resultList);

            if (parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
            {
                parseState.CurrentTokenIndex++;
                return resultList;
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

                object elem = cnv.Deserialize_Internal(ref parseState, context, settings);

                resultList.Add(elem);
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


            return resultList;

        }

        public override object Populate_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            if (BJsonUtils.IsCachedOrNull(ref parseState, context, settings, out object cached))
            {
                return cached;
            }

            // skip [
            parseState.CurrentTokenIndex++;

            // skip $type
            // OR if the array is empty , then just skip to the end of the array
            // so basically skip until the first ',' or ']'
            parseState.SkipUntil(ParserUtils.IsPropertyEndInArray);

            IList originalList = (IList)instance;
            IList resultList = originalList;

            if (resultList == null)
            {
                resultList = CreateEmpty();
            }

            int registerId = context.Register(resultList);

            if (parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
            {
                parseState.CurrentTokenIndex++;
                return resultList;
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

                if (originalList != null && currentIndex < originalList.Count)
                {
                    currElement = originalList[currentIndex];
                }

                object elem = cnv.Populate_Internal(currElement, ref parseState, context, settings);

                if (currentIndex < resultList.Count)
                {
                    resultList[currentIndex] = elem;
                }
                else
                {
                    resultList.Add(elem);
                }
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


            return resultList;

        }

        private IList CreateEmpty()
        {
            IList lst = (IList)Constructor();

            return lst;
        }


    }
}
