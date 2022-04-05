using Bloodthirst.Core.Utils;
using System;
using System.Text;

namespace Bloodthirst.BJson
{
    internal class BJsonFlagConverter : BJsonComplexBaseConverter
    {
        private IBJsonConverterInternal IntJsonConverter { get; set; }

        private readonly object[] enumValues;

        public BJsonFlagConverter(Type t) : base(t)
        {
            Array vals = Enum.GetValues(t);
            enumValues = new object[vals.Length];
            vals.CopyTo(enumValues, 0);
        }

        public override void Initialize()
        {
            IntJsonConverter = Provider.GetConverter(typeof(int));
        }

        public override object CreateInstance_Internal()
        {
            return enumValues[0];
        }

        public override void Serialize_Formatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            Enum casted = (Enum)instance;

            int flag = 0;

            for (int i = 0; i < enumValues.Length; i++)
            {
                Enum curr = (Enum)enumValues[i];

                bool isTrue = casted.HasFlag(curr);

                int asInt = Convert.ToInt32(isTrue);

                flag |= asInt << i;
            }

            IntJsonConverter.Serialize_Formatted_Internal(flag, jsonBuilder, context, settings);
        }

        public override void Serialize_NonFormatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            Enum casted = (Enum)instance;

            int flag = 0;

            for (int i = 0; i < enumValues.Length; i++)
            {
                Enum curr = (Enum)enumValues[i];

                bool isTrue = casted.HasFlag(curr);

                int asInt = Convert.ToInt32(isTrue);

                flag |= asInt << i;
            }

            IntJsonConverter.Serialize_NonFormatted_Internal(flag, jsonBuilder, context, settings);
        }


        public override object Deserialize_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            int flagValue = (int)IntJsonConverter.Deserialize_Internal(instance, ref parseState, context, settings);

            object val = Enum.ToObject(ConvertType, flagValue);

            return val;
        }

    }
}
