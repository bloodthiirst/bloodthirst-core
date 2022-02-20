using Bloodthirst.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.BJson
{
    internal class BJsonUnityObjectConverter : BJsonComplexBaseConverter
    {
        private Func<object> Constructor { get; set; }

        private IBJsonConverterInternal DefaultJsonConverter { get; set; }
        private IBJsonConverterInternal IntJsonConverter { get; set; }
        public BJsonUnityObjectConverter(Type t) : base(t)
        {

        }

        public override void Initialize()
        {
            // if we don't do this
            // is will try to create custom converters for gameObject or transform
            // which we don't want
            if (TypeUtils.IsSubTypeOf(ConvertType, typeof(ScriptableObject)) || TypeUtils.IsSubTypeOf(ConvertType, typeof(MonoBehaviour)))
            {
                DefaultJsonConverter = new BJsonComplexConverter(ConvertType);
                DefaultJsonConverter.Provider = Provider;
                DefaultJsonConverter.Initialize();
            }

            IntJsonConverter = Provider.GetConverter(typeof(int));
        }

        public override object CreateInstance_Internal()
        {
            return null;
        }

        public override void Serialize_NonFormatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (jsonBuilder.Length == 0)
            {
                DefaultJsonConverter.Serialize_NonFormatted_Internal(instance, jsonBuilder, context, settings);
                return;
            }

            jsonBuilder.Append('{');

            BJsonUtils.WriteTypeInfoNonFormatted(instance, jsonBuilder, context, settings);

            // unity index
            UnityObjectContext ctx = (UnityObjectContext)settings.CustomContext;
            int index = ctx.UnityObjects.Count;
            ctx.UnityObjects.Add((UnityEngine.Object)instance);

            jsonBuilder.Append("index:");
            jsonBuilder.Append(index);
            jsonBuilder.Append('}');
        }

        public override void Serialize_Formatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (jsonBuilder.Length == 0)
            {
                DefaultJsonConverter.Serialize_Formatted_Internal(instance, jsonBuilder, context, settings);
                return;
            }

            if (context.Indentation != 0)
            {
                jsonBuilder.Append(Environment.NewLine);
            }

            jsonBuilder.AddIndentation(context.Indentation);

            jsonBuilder.Append('{');

            jsonBuilder.Append(Environment.NewLine);

            context.Indentation++;

            BJsonUtils.WriteTypeInfoFormatted(instance, jsonBuilder, context, settings);

            // unity index
            UnityObjectContext ctx = (UnityObjectContext)settings.CustomContext;
            int index = ctx.UnityObjects.Count;
            ctx.UnityObjects.Add((UnityEngine.Object)instance);

            jsonBuilder.Append(Environment.NewLine);

            jsonBuilder.AddIndentation(context.Indentation);
            jsonBuilder.Append("index : ");
            jsonBuilder.Append(index);

            context.Indentation--;
            jsonBuilder.Append(Environment.NewLine);
            jsonBuilder.AddIndentation(context.Indentation);
            jsonBuilder.Append('}');
        }

        public override object Deserialize_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            if (parseState.CurrentTokenIndex == 0)
            {
                return DefaultJsonConverter.Deserialize_Internal(instance, ref parseState, context, settings);
            }

            // skip {
            parseState.CurrentTokenIndex++;

            // skip the "$type" property
            parseState.SkipUntil(ParserUtils.IsComma);

            // skip the "index : " and go straight to the index value
            parseState.SkipUntil(ParserUtils.IsColon);

            // skip the colon
            parseState.CurrentTokenIndex++;

            UnityObjectContext ctx = (UnityObjectContext)settings.CustomContext;

            Assert.IsNotNull(ctx);

            int id = (int)IntJsonConverter.Deserialize_Internal(instance, ref parseState, context, settings);

            parseState.SkipUntil(ParserUtils.IsPropertyEndObject);
            parseState.CurrentTokenIndex++;

            return ctx.UnityObjects[id];
        }

    }
}
