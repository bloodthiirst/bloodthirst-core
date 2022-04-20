using Bloodthirst.Core.Utils;
using System;
using System.Collections;
using System.Text;

namespace Bloodthirst.BJson
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
            KeyConverter = Provider.GetConverter(KeyType);
            ValueConverter = Provider.GetConverter(ValueType);
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

            // open dict
            jsonBuilder.Append('[');

            BJsonUtils.WriteTypeInfoNonFormatted(instance, jsonBuilder, context, settings);

            IDictionary dict = (IDictionary)instance;

            object[] keys = new object[dict.Count];
            object[] vals = new object[dict.Count];

            dict.Keys.CopyTo(keys, 0);
            dict.Values.CopyTo(vals, 0);

            // write the key/value paris
            for (int i = 0; i < dict.Count; i++)
            {
                jsonBuilder.Append('{');

                KeyConverter.Serialize_NonFormatted_Internal(keys[i], jsonBuilder, context, settings);

                jsonBuilder.Append(',');

                ValueConverter.Serialize_NonFormatted_Internal(vals[i], jsonBuilder, context, settings);

                jsonBuilder.Append('}');

                jsonBuilder.Append(',');
            }

            // close the dictionary
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


            IDictionary dict = (IDictionary)instance;

            object[] keys = new object[dict.Count];
            object[] vals = new object[dict.Count];

            dict.Keys.CopyTo(keys, 0);
            dict.Values.CopyTo(vals, 0);

            for (int i = 0; i < dict.Count; i++)
            {
                jsonBuilder.Append(Environment.NewLine);

                jsonBuilder.AddIndentation(context.Indentation);

                jsonBuilder.Append('{');

                context.Indentation++;
                jsonBuilder.Append(Environment.NewLine);
                jsonBuilder.AddIndentation(context.Indentation);

                KeyConverter.Serialize_Formatted_Internal(keys[i], jsonBuilder, context, settings);

                jsonBuilder.Append(',');
                jsonBuilder.Append(Environment.NewLine);
                jsonBuilder.AddIndentation(context.Indentation);

                ValueConverter.Serialize_Formatted_Internal(vals[i], jsonBuilder, context, settings);

                context.Indentation--;
                jsonBuilder.Append(Environment.NewLine);
                jsonBuilder.AddIndentation(context.Indentation);

                jsonBuilder.Append('}');

                jsonBuilder.Append(',');
            }

            context.Indentation--;

            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);

            jsonBuilder.Append(Environment.NewLine);

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

            IDictionary dict = (IDictionary)Constructor();

            context.Register(dict);


            if (parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
            {
                parseState.CurrentTokenIndex++;
                return dict;
            }

            // skip the comma
            parseState.CurrentTokenIndex++;

            while (parseState.CurrentTokenIndex < parseState.Tokens.Count)
            {
                // skip the pair object start
                parseState.CurrentTokenIndex++;

                // get key
                object defaultKey = KeyConverter.CreateInstance_Internal();
                object key = KeyConverter.Deserialize_Internal(ref parseState, context, settings);

                // skip the comma
                parseState.CurrentTokenIndex++;

                // get value
                object defaultVal = ValueConverter.CreateInstance_Internal();
                object val = ValueConverter.Deserialize_Internal(ref parseState, context, settings);

                // add
                dict.Add(key, val);

                // skip until the first comma or object end
                parseState.SkipUntil(ParserUtils.IsPropertyEndInArray);

                if (parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
                {
                    parseState.CurrentTokenIndex++;
                    break;
                }

                parseState.CurrentTokenIndex++;
            }

            return dict;
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

            IDictionary dict = (IDictionary)instance;
            IDictionary resultDict = dict;

            if (resultDict == null)
            {
                resultDict = (IDictionary)Constructor();
            }
            else
            {
                resultDict.Clear();
            }

            context.Register(dict);


            if (parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
            {
                parseState.CurrentTokenIndex++;
                return dict;
            }

            // skip the comma
            parseState.CurrentTokenIndex++;

            while (parseState.CurrentTokenIndex < parseState.Tokens.Count)
            {
                // skip the { for pair object start
                parseState.CurrentTokenIndex++;

                // get key
                //object defaultKey = KeyConverter.CreateInstance_Internal();
                object key = KeyConverter.Deserialize_Internal(ref parseState, context, settings);

                // skip the comma
                parseState.CurrentTokenIndex++;

                // get value
                //object defaultVal = ValueConverter.CreateInstance_Internal();
                object val = ValueConverter.Deserialize_Internal(ref parseState, context, settings);

                // add
                dict.Add(key, val);

                // skip until the first comma or object end
                parseState.SkipUntil(ParserUtils.IsPropertyEndInArray);

                if (parseState.CurrentToken.TokenType == JSONTokenType.ARRAY_END)
                {
                    parseState.CurrentTokenIndex++;
                    break;
                }

                parseState.CurrentTokenIndex++;
            }

            return dict;
        }


    }
}
