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
    internal class BJsonComplexConverter : BJsonComplexBaseConverter
    {
        private BTypeData TypeData { get; set; }

        private Dictionary<string, BMemberData> MemberToDataDictionary { get; set; }
        private IBJsonConverterInternal IntConverter { get; set; }
        private IBJsonConverterInternal TypeConverter { get; set; }
        private int MemberCount { get; set; }

        public BJsonComplexConverter(Type t) : base(t)
        {

        }

        public override void Initialize()
        {
            // filter props
            TypeData = BJsonPropertyFilterProvider.GetFilteredProperties(ConvertType);

            MemberToDataDictionary = TypeData.MemberDatas.ToDictionary(k => k.MemberInfo.Name, k => k);
            IntConverter = Provider.GetConverter(typeof(int));
            TypeConverter = Provider.GetConverter(typeof(Type));
            MemberCount = MemberToDataDictionary.Count;
        }
        public override object CreateInstance_Internal()
        {
            return TypeData.Constructor();
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

            jsonBuilder.Append('{');

            BJsonUtils.WriteTypeInfoNonFormatted(instance, jsonBuilder, context, settings);

            foreach (KeyValuePair<string, BMemberData> kv in MemberToDataDictionary)
            {
                jsonBuilder.Append(kv.Key);

                jsonBuilder.Append(':');

                IBJsonConverterInternal valueConv = Provider.GetConverter(kv.Value.Type);
                object val = kv.Value.MemberGetter(instance);
                valueConv.Serialize_NonFormatted_Internal(val, jsonBuilder, context, settings);

                jsonBuilder.Append(',');
            }

            jsonBuilder[jsonBuilder.Length - 1] = '}';
        }

        public override void Serialize_Formatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (context.Indentation != 0)
            {
                jsonBuilder.Append(Environment.NewLine);
            }

            jsonBuilder.AddIndentation(context.Indentation);

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

            jsonBuilder.Append('{');

            jsonBuilder.Append(Environment.NewLine);

            context.Indentation++;

            BJsonUtils.WriteTypeInfoFormatted(instance, jsonBuilder, context, settings);
            jsonBuilder.Append(Environment.NewLine);

            foreach (KeyValuePair<string, BMemberData> kv in MemberToDataDictionary)
            {
                jsonBuilder.AddIndentation(context.Indentation);
                jsonBuilder.Append(kv.Key);
                jsonBuilder.Append(" : ");

                IBJsonConverterInternal valueConv = Provider.GetConverter(kv.Value.Type);
                object val = kv.Value.MemberGetter(instance);
                valueConv.Serialize_Formatted_Internal(val, jsonBuilder, context, settings);

                jsonBuilder.Append(',');
                jsonBuilder.Append(Environment.NewLine);

            }

            context.Indentation--;

            // remove the extra last ','
            // we do -3 because "NewLine" is actually two characters \r\n
            jsonBuilder.Remove(jsonBuilder.Length - 3, 1);

            jsonBuilder.AddIndentation(context.Indentation);

            jsonBuilder.Append('}');
        }

        public override object Deserialize_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            if (BJsonUtils.IsCachedOrNull(instance, ref parseState, context, settings, out object cached))
            {
                return cached;
            }

            if (instance == null)
            {
                instance = CreateInstance_Internal();
            }

            context.Register(instance);

            // skip the first object start
            parseState.CurrentTokenIndex++;

            while (parseState.CurrentTokenIndex < parseState.Tokens.Count)
            {
                Token<JSONTokenType> currentToken = parseState.Tokens[parseState.CurrentTokenIndex];

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

                // stp while no identitifer
                if (currentToken.TokenType != JSONTokenType.IDENTIFIER)
                {
                    parseState.CurrentTokenIndex++;
                    continue;
                }

                // key found
                string key = currentToken.ToString();

                // skip until the first colon
                parseState.SkipUntil(ParserUtils.IsColon);

                parseState.CurrentTokenIndex++;
                currentToken = parseState.Tokens[parseState.CurrentTokenIndex];

                // skip space
                parseState.SkipWhile(ParserUtils.IsSkippableSpace);

                if (MemberToDataDictionary.TryGetValue(key, out BMemberData memData))
                {
                    IBJsonConverterInternal c = Provider.GetConverter(memData.Type);

                    object oldVal = instance == null ? null : memData.MemberGetter(instance);
                    object newVal = c.Deserialize_Internal(oldVal, ref parseState, context, settings);

                    memData.MemberSetter(instance, newVal);
                }

                // skip until the first comma or object end
                parseState.SkipUntil(ParserUtils.IsPropertyEndObject);

                if (parseState.CurrentToken.TokenType == JSONTokenType.OBJECT_END)
                {
                    parseState.CurrentTokenIndex++;
                    break;
                }
            }

            return instance;
        }


    }
}
