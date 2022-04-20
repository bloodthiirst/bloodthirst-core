using Bloodthirst.BType;
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
    internal class BJsonRefMonoBehaviourConverter : BJsonComplexBaseConverter
    {
        private IBJsonConverterInternal RefMonoBehaviourConverter { get; set; }

        public BJsonRefMonoBehaviourConverter() : base(typeof(Component))
        {

        }

        public override void Initialize()
        {
            RefMonoBehaviourConverter = Provider.GetConverter(typeof(BJsonRefMonoBehaviourData));
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

            BJsonRefMonoBehaviourData refData = BJsonRefMonoBehaviourData.CreateRef(instance);

            RefMonoBehaviourConverter.Serialize_NonFormatted_Internal(refData, jsonBuilder, context, settings);
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

            BJsonRefMonoBehaviourData refData = BJsonRefMonoBehaviourData.CreateRef(instance);

            RefMonoBehaviourConverter.Serialize_Formatted_Internal(refData, jsonBuilder, context, settings);
        }

        public override object Deserialize_Internal(ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            if (BJsonUtils.IsCachedOrNull(ref parseState, context, settings, out object cached))
            {
                return cached;
            }

            if (parseState.CurrentToken.TokenType == JSONTokenType.NULL)
            {
                parseState.CurrentTokenIndex++;
                return null;
            }

            BJsonRefMonoBehaviourData componentRef = (BJsonRefMonoBehaviourData) RefMonoBehaviourConverter.Deserialize_Internal(ref parseState , context , settings);

            object component = BJsonRefMonoBehaviourData.LoadRef(componentRef , settings);

            context.Register(component);

            return componentRef;
        }
        public override object Populate_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            if (BJsonUtils.IsCachedOrNull(ref parseState, context, settings, out object cached))
            {
                return cached;
            }

            if (parseState.CurrentToken.TokenType == JSONTokenType.NULL)
            {
                parseState.CurrentTokenIndex++;
                return null;
            }

            BJsonRefMonoBehaviourData componentRef = (BJsonRefMonoBehaviourData)RefMonoBehaviourConverter.Populate_Internal(null , ref parseState, context, settings);

            object component = BJsonRefMonoBehaviourData.LoadRef(componentRef , settings);

            context.Register(component);

            return instance;
        }


    }
}
