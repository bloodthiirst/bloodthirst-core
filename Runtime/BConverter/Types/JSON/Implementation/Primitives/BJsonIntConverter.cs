using Bloodthirst.Core.Utils;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Bloodthirst.BDeepCopy
{
    internal class BJsonIntConverter : BJsonBase<int>
    {
        public override void Initialize()
        {
            
        }

        public override object CreateInstance_Internal()
        {
            return 0;
        }

        public override object From_Internal( object instance, ref ParserState<JSONTokenType> parseState, BConverterContext context, BConverterSettings settings)
        {
            Token<JSONTokenType> t = parseState.CurrentToken;
            string str = t.AsString();
            parseState.CurrentTokenIndex++;

            return int.Parse(str);
        }

        public override string To_Internal(object instance, BConverterContext context, BConverterSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
