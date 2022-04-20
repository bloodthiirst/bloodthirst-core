using System;
using System.Text;
using UnityEngine.Assertions;

namespace Bloodthirst.BJson
{
    internal class BJsonInterfaceOrAbstractConverter : BJsonComplexBaseConverter
    {
        private IBJsonConverterInternal TypeConverter { get; set; }

        public BJsonInterfaceOrAbstractConverter(Type t) : base(t)
        {

        }

        public override void Initialize()
        {
            TypeConverter = Provider.GetConverter(typeof(Type));
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

            Type t = instance.GetType();

            IBJsonConverterInternal c = Provider.GetConverter(t);

            c.Serialize_NonFormatted_Internal(instance, jsonBuilder, context, settings);
        }

        public override void Serialize_Formatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (instance == null)
            {
                jsonBuilder.Append(BJsonUtils.NULL_IDENTIFIER);
                return;
            }

            Type t = instance.GetType();

            IBJsonConverterInternal c = Provider.GetConverter(t);

            c.Serialize_Formatted_Internal(instance, jsonBuilder, context, settings);
        }

        public override object Deserialize_Internal(ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            if (BJsonUtils.IsCachedOrNull(ref parseState, context, settings, out object cached))
            {
                return cached;
            }

            Type concreteType = BJsonUtils.GetConcreteType(parseState);

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

            Type concreteType = BJsonUtils.GetConcreteType(parseState);

            Assert.IsNotNull(concreteType);

            IBJsonConverterInternal c = Provider.GetConverter(concreteType);

            object actualInstance = c.Populate_Internal(instance, ref parseState, context, settings);

            return actualInstance;
        }


    }
}
