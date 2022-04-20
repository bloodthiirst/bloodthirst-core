using System.Text;

namespace Bloodthirst.BJson
{
    internal class BJsonFloatConverter : BJsonBase<float>
    {
        internal override void Initialize()
        {

        }

        public override object CreateInstance_Internal()
        {
            return 0f;
        }

        public override object Populate_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            Token<JSONTokenType> t = parseState.CurrentToken;
            string str = t.ToString();
            parseState.CurrentTokenIndex++;

            return float.Parse(str);
        }
        public override object Deserialize_Internal(ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            Token<JSONTokenType> t = parseState.CurrentToken;
            string str = t.ToString();
            parseState.CurrentTokenIndex++;

            return float.Parse(str);
        }

        public override void Serialize_Formatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            jsonBuilder.Append(instance.ToString());
        }

        public override void Serialize_NonFormatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            jsonBuilder.Append(instance.ToString());
        }
    }
}
