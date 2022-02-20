using Bloodthirst.Core.EnumLookup;
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
    internal class BJsonEnumConverter : BJsonComplexBaseConverter
    {
        private IBJsonConverterInternal IntJsonConverter { get; set; }

        private readonly int[] enumValues;

        public BJsonEnumConverter(Type t) : base(t)
        {
            enumValues = (int[])Enum.GetValues(t);
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
            IntJsonConverter.Serialize_Formatted_Internal((int) instance, jsonBuilder, context, settings);
        }

        public override void Serialize_NonFormatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            IntJsonConverter.Serialize_NonFormatted_Internal((int)instance, jsonBuilder, context, settings);
        }


        public override object Deserialize_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            int enumIndex = (int)IntJsonConverter.Deserialize_Internal(instance, ref parseState, context, settings);

            return enumValues[enumIndex];
        }

    }
}
