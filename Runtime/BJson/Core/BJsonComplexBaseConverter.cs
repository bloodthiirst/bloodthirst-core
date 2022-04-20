using System;
using System.Text;

namespace Bloodthirst.BJson
{
    public abstract class BJsonComplexBaseConverter : IBJsonConverterInternal
    {
        protected Type ConvertType { get; set; }

        public BJsonProvider Provider { get; set; }
        BJsonProvider IBJsonConverterInternal.Provider { get => Provider; set => Provider = value; }
        public BJsonComplexBaseConverter(Type t)
        {
            this.ConvertType = t;
        }

        public abstract object CreateInstance_Internal();
        public abstract void Initialize();

        #region Object To JSON

        public string Serialize(object t, BJsonContext context, BJsonSettings settings)
        {
            StringBuilder sb = new StringBuilder();

            if (settings.Formated)
            {
                Serialize_Formatted_Internal(t, sb, context, settings);
            }
            else
            {
                Serialize_NonFormatted_Internal(t, sb, context, settings);
            }

            return sb.ToString();
        }
        public abstract void Serialize_Formatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings);
        public abstract void Serialize_NonFormatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings);

        #endregion

        #region JSON to Object
        public object Deserialize(string json, BJsonContext context, BJsonSettings settings)
        {
            TokenizerState<JSONTokenType> tokens = JSONParser.Instance.TokenizeString(json);
            ParserState<JSONTokenType> state = new ParserState<JSONTokenType>(tokens);
            ParserUtils.RemoveSpaces(ref state);

            object instance = CreateInstance_Internal();

            return Deserialize_Internal(ref state, context, settings);
        }

        public object Populate(object instance, string json, BJsonContext context, BJsonSettings settings)
        {
            TokenizerState<JSONTokenType> tokens = JSONParser.Instance.TokenizeString(json);
            ParserState<JSONTokenType> state = new ParserState<JSONTokenType>(tokens);
            ParserUtils.RemoveSpaces(ref state);

            return Populate_Internal(instance, ref state, context, settings);
        }

        public abstract object Deserialize_Internal(ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings);
        public abstract object Populate_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings);
        #endregion
    }
}
