using System;

namespace Bloodthirst.BDeepCopy
{
    internal abstract class BJsonBase<TFrom> : BConverter<TFrom, string>, IBJsonConverterInternal
    {
        public abstract object From_Internal(object instance, ref ParserState<JSONTokenType> parseState, BConverterContext context, BConverterSettings settings);
        public abstract string To_Internal(object instance, BConverterContext context, BConverterSettings settings);
        public abstract object CreateInstance_Internal();

        public object From(string json)
        {
            TokenizerState<JSONTokenType> tokens = JSONParser.Instance.TokenizeString(json);
            ParserState<JSONTokenType> state = new ParserState<JSONTokenType>(tokens);

            object instance = CreateInstance_Internal();

            return From_Internal( instance, ref state, new BConverterContext(), new BConverterSettings());
        }

        protected override TFrom ConvertFrom(string t, BConverterContext context, BConverterSettings settings)
        {
            TokenizerState<JSONTokenType> tokens = JSONParser.Instance.TokenizeString(t);
            ParserState<JSONTokenType> state = new ParserState<JSONTokenType>(tokens);
            object instance = CreateInstance_Internal();
            return (TFrom)From_Internal( instance, ref state, context, settings);
        }

        public object Populate( object instance, string json)
        {
            TokenizerState<JSONTokenType> tokens = JSONParser.Instance.TokenizeString(json);
            ParserState<JSONTokenType> state = new ParserState<JSONTokenType>(tokens);

            return From_Internal( instance, ref state, new BConverterContext(), new BConverterSettings());
        }

        public string To(object t)
        {
            return To_Internal(t, new BConverterContext() , new BConverterSettings());
        }


        protected override string ConvertTo(TFrom t, BConverterContext context, BConverterSettings settings)
        {
            return To_Internal(t, context, settings);
        }

    }
}
