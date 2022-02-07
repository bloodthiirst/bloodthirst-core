using System;
using System.Text;

namespace Bloodthirst.BDeepCopy
{
    internal abstract class BJsonBase<TFrom> : BConverter<TFrom, string>, IBJsonConverterInternal
    {
        public abstract object From_Internal(object instance, ref ParserState<JSONTokenType> parseState, BConverterContext context, BConverterSettings settings);
        public abstract void To_Internal(object instance, StringBuilder jsonBuilder, BConverterContext context, BConverterSettings settings);
        public abstract object CreateInstance_Internal();

        public object From(string json, BConverterContext context, BConverterSettings settings)
        {
            TokenizerState<JSONTokenType> tokens = JSONParser.Instance.TokenizeString(json);
            ParserState<JSONTokenType> state = new ParserState<JSONTokenType>(tokens);

            object instance = CreateInstance_Internal();

            return From_Internal(instance, ref state, context, settings);
        }

        protected override TFrom ConvertFrom(string t, BConverterContext context, BConverterSettings settings)
        {
            TokenizerState<JSONTokenType> tokens = JSONParser.Instance.TokenizeString(t);
            ParserState<JSONTokenType> state = new ParserState<JSONTokenType>(tokens);
            object instance = CreateInstance_Internal();
            return (TFrom)From_Internal(instance, ref state, context, settings);
        }

        public object Populate(object instance, string json, BConverterContext context, BConverterSettings settings)
        {
            TokenizerState<JSONTokenType> tokens = JSONParser.Instance.TokenizeString(json);
            ParserState<JSONTokenType> state = new ParserState<JSONTokenType>(tokens);

            return From_Internal(instance, ref state, context, settings);
        }

        public string To(object t, BConverterContext context, BConverterSettings settings)
        {
            StringBuilder sb = new StringBuilder();
            To_Internal(t, sb, context, settings);

            return sb.ToString();
        }


        protected override string ConvertTo(TFrom t, BConverterContext context, BConverterSettings settings)
        {
            StringBuilder sb = new StringBuilder();
            To_Internal(t, sb, context, settings);

            return sb.ToString();
        }

    }
}
