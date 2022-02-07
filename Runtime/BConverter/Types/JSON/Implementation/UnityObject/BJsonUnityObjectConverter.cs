using Bloodthirst.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bloodthirst.BDeepCopy
{
    internal class BJsonUnityObjectConverter : BJsonComplexBaseConverter
    {
        private Func<object> Constructor { get; set; }

        private IBJsonConverterInternal DefaultJsonConverter { get; set; }
        private IBJsonConverterInternal IntJsonConverter { get; set; }
        public BJsonUnityObjectConverter(Type t) : base(t)
        {
            DefaultJsonConverter = new BJsonComplexConverter(t);
        }

        public override void Initialize()
        {
            IntJsonConverter = (IBJsonConverterInternal) Provider.Get(typeof(int));
        }

        public override object CreateInstance_Internal()
        {
            return Constructor();
        }

        public override void To_Internal(object instance, StringBuilder jsonBuilder, BConverterContext context, BConverterSettings settings)
        {
            throw new NotImplementedException();
        }

        public override object From_Internal(object instance, ref ParserState<JSONTokenType> parseState, BConverterContext context, BConverterSettings settings)
        {
            if(parseState.CurrentTokenIndex == 0)
            {
                return DefaultJsonConverter.From_Internal(instance , ref parseState , context , settings);
            }

            UnityObjectContext ctx = (UnityObjectContext)settings.CustomContext;

            int id = (int) IntJsonConverter.From_Internal(instance, ref parseState , context , settings);

            return ctx.UnityObjects[id];
        }

    }
}
