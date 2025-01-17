using System.Text;

namespace Bloodthirst.BJson
{
    internal class BJsonStringConverter : BJsonBase<string>
    {
        internal override void Initialize()
        {

        }
        public override object CreateInstance_Internal()
        {
            return string.Empty;
        }

        public override object Deserialize_Internal(ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            string t = parseState.CurrentToken.ToString();

            parseState.CurrentTokenIndex++;

            if (t == BJsonUtils.NULL_IDENTIFIER)
            {
                return null;
            }

            return t.Substring(1, t.Length - 2);
        }

        public override object Populate_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            string t = parseState.CurrentToken.ToString();

            parseState.CurrentTokenIndex++;

            if (t == BJsonUtils.NULL_IDENTIFIER)
            {
                return null;
            }

            return t.Substring(1, t.Length - 2);
        }

        public override void Serialize_Formatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (instance == null)
            {
                jsonBuilder.Append(BJsonUtils.NULL_IDENTIFIER);
                return;
            }

            jsonBuilder.Append('"');
            jsonBuilder.Append((string)instance);
            jsonBuilder.Append('"');
        }

        public override void Serialize_NonFormatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (instance == null)
            {
                jsonBuilder.Append(BJsonUtils.NULL_IDENTIFIER);
                return;
            }

            jsonBuilder.Append('"');
            jsonBuilder.Append((string)instance);
            jsonBuilder.Append('"');
        }
    }
}
