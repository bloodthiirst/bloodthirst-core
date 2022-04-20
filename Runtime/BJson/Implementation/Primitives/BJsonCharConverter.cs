using System.Text;

namespace Bloodthirst.BJson
{
    internal class BJsonCharConverter : BJsonBase<char>
    {
        internal override void Initialize()
        {

        }

        public override object CreateInstance_Internal()
        {
            return new char();
        }

        public override object Deserialize_Internal(ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            Token<JSONTokenType> t = parseState.CurrentToken;

            parseState.CurrentTokenIndex++;

            return t.Text[t.StartIndex];
        }
        public override object Populate_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            Token<JSONTokenType> t = parseState.CurrentToken;

            parseState.CurrentTokenIndex++;

            return t.Text[t.StartIndex];
        }

        public override void Serialize_Formatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            jsonBuilder.Append((char)instance);
        }

        public override void Serialize_NonFormatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            jsonBuilder.Append((char)instance);
        }
    }
}
