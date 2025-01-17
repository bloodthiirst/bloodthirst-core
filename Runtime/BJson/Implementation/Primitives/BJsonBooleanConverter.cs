using System.Text;

namespace Bloodthirst.BJson
{
    internal class BJsonBooleanConverter : BJsonBase<bool>
    {
        internal override void Initialize()
        {

        }

        public override object CreateInstance_Internal()
        {
            return false;
        }

        public override object Deserialize_Internal(ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            Token<JSONTokenType> t = parseState.CurrentToken;
            string str = t.ToString();
            parseState.CurrentTokenIndex++;

            return bool.Parse(str);
        }
        public override object Populate_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            Token<JSONTokenType> t = parseState.CurrentToken;
            string str = t.ToString();
            parseState.CurrentTokenIndex++;

            return bool.Parse(str);
        }

        public override void Serialize_Formatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            string val = ((bool)instance) == false ? "false" : "true";
            jsonBuilder.Append(val);
        }
        public override void Serialize_NonFormatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            string val = ((bool)instance) == false ? "false" : "true";
            jsonBuilder.Append(val);
        }
    }
}
