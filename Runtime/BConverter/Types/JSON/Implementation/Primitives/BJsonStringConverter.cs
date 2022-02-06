using Bloodthirst.Core.Utils;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Bloodthirst.BDeepCopy
{
    internal class BJsonStringConverter : BJsonBase<string>
    {
        public override void Initialize()
        {

        }
        public override object CreateInstance_Internal()
        {
            return string.Empty;
        }

        public override object From_Internal( object instance, ref ParserState<JSONTokenType> parseState, BConverterContext context, BConverterSettings settings)
        {
            string t = parseState.CurrentToken.AsString();
            parseState.CurrentTokenIndex++;

            return t.Substring(1, t.Length - 2);
        }

        public override string To_Internal(object instance, BConverterContext context, BConverterSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
