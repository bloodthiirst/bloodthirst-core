using System;
using System.Text;
using UnityEngine.Assertions;

namespace Bloodthirst.BJson
{
    internal class BJsonObjectConverter : BJsonComplexBaseConverter
    {
        private IBJsonConverterInternal TypeConverter { get; set; }

        public BJsonObjectConverter() : base(typeof(object))
        {

        }

        public override void Initialize()
        {
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

            if (BJsonUtils.WriteIdNonFormatted(instance, jsonBuilder, context, settings))
            {
                return;
            }

            context.Register(instance);

            jsonBuilder.Append('{');

            BJsonUtils.WriteTypeInfoNonFormatted(instance, jsonBuilder, context, settings);

            Type t = instance.GetType();

            IBJsonConverterInternal c = Provider.GetConverter(t);

            // write the actual value
            c.Serialize_NonFormatted_Internal(instance, jsonBuilder, context, settings);

            jsonBuilder.Append('}');
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

            jsonBuilder.Append('{');

            jsonBuilder.Append(Environment.NewLine);

            context.Indentation++;

            BJsonUtils.WriteTypeInfoFormatted(instance, jsonBuilder, context, settings);
            jsonBuilder.Append(Environment.NewLine);

            Type t = instance.GetType();

            jsonBuilder.AddIndentation(context.Indentation);

            IBJsonConverterInternal c = Provider.GetConverter(t);

            // write the actual value
            c.Serialize_Formatted_Internal(instance, jsonBuilder, context, settings);

            context.Indentation--;

            jsonBuilder.Append(Environment.NewLine);

            jsonBuilder.AddIndentation(context.Indentation);

            jsonBuilder.Append('}');
        }

        public override object Deserialize_Internal(ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            if (BJsonUtils.IsCachedOrNull(ref parseState, context, settings, out object cached))
            {
                return cached;
            }

            Type concreteType = BJsonUtils.GetConcreteTypeRef(ref parseState);

            IBJsonConverterInternal c = Provider.GetConverter(concreteType);

            object actualInstance = c.Deserialize_Internal(ref parseState, context, settings);

            return actualInstance;
        }

        public override object Populate_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            if (BJsonUtils.IsCachedOrNull(ref parseState, context, settings, out object cached))
            {
                return cached;
            }

            Type concreteType = BJsonUtils.GetConcreteTypeRef(ref parseState);

            Assert.IsNotNull(concreteType);

            IBJsonConverterInternal c = Provider.GetConverter(concreteType);

            object actualInstance = c.Populate_Internal(instance, ref parseState, context, settings);

            return actualInstance;
        }


    }
}
