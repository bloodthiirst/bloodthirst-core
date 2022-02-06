using System;

namespace Bloodthirst.BDeepCopy
{
    internal abstract class BJsonComplexBaseConverter : IBJsonConverterInternal
    {
        protected Type ConvertType { get; set; }

        public IBConverterProvider Provider { get; set; }

        public BJsonComplexBaseConverter(Type t)
        {
            this.ConvertType = t;
        }

        Type IBConverter.FromType => ConvertType;

        Type IBConverter.ToType => typeof(string);

        public abstract object CreateInstance_Internal();
        public abstract void Initialize();

        #region Object To JSON
        public object ConvertTo(object t)
        {
            return To_Internal(t, new BConverterContext(), new BConverterSettings());
        }
        public object ConvertTo_Internal(object t, BConverterContext context, BConverterSettings settings)
        {
            return To_Internal(t, context, settings);
        }

        public string To(object t)
        {
            return To_Internal(t, new BConverterContext(), new BConverterSettings());
        }
        public abstract string To_Internal(object instance, BConverterContext context, BConverterSettings settings);

        #endregion

        #region JSON to Object
        public object ConvertFrom(object t)
        {
            string json = (string)t;

            TokenizerState<JSONTokenType> tokens = JSONParser.Instance.TokenizeString(json);
            ParserState<JSONTokenType> state = new ParserState<JSONTokenType>(tokens);
            RemoveSpaces(ref state);

            object instance = CreateInstance_Internal();

            return From_Internal(instance, ref state, new BConverterContext(), new BConverterSettings());
        }

        private static void RemoveSpaces(ref ParserState<JSONTokenType> state)
        {
            // remove spaces
            for (int i = state.Tokens.Count - 1; i >= 0; i--)
            {
                Token<JSONTokenType> token = state.Tokens[i];

                if (token.TokenType == JSONTokenType.SPACE || token.TokenType == JSONTokenType.NEW_LINE)
                {
                    state.Tokens.RemoveAt(i);
                }
            }
        }

        public object ConvertFrom_Internal(object t, BConverterContext context, BConverterSettings settings)
        {
            string json = (string)t;

            TokenizerState<JSONTokenType> tokens = JSONParser.Instance.TokenizeString(json);
            ParserState<JSONTokenType> state = new ParserState<JSONTokenType>(tokens);
            RemoveSpaces(ref state);

            object instance = CreateInstance_Internal();

            return From_Internal(instance, ref state, context, settings);
        }
        public object From(string json)
        {
            TokenizerState<JSONTokenType> tokens = JSONParser.Instance.TokenizeString(json);
            ParserState<JSONTokenType> state = new ParserState<JSONTokenType>(tokens);
            RemoveSpaces(ref state);

            object instance = CreateInstance_Internal();

            return From_Internal(instance, ref state, new BConverterContext(), new BConverterSettings());
        }

        public object Populate(object instance, string json)
        {
            TokenizerState<JSONTokenType> tokens = JSONParser.Instance.TokenizeString(json);
            ParserState<JSONTokenType> state = new ParserState<JSONTokenType>(tokens);
            RemoveSpaces(ref state);

            return From_Internal(instance, ref state, new BConverterContext(), new BConverterSettings());
        }

        public abstract object From_Internal(object instance, ref ParserState<JSONTokenType> parseState, BConverterContext context, BConverterSettings settings);
        #endregion
    }
}
