using System;
using System.Text;

namespace Bloodthirst.BJson
{
    internal class BJsonTypeConverter : BJsonBase<Type>
    {
        internal override void Initialize()
        {

        }
        public override object CreateInstance_Internal()
        {
            return null;
        }

        public override object Deserialize_Internal(ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            string t = parseState.CurrentToken.ToString();

            parseState.CurrentTokenIndex++;

            if (t == BJsonUtils.NULL_IDENTIFIER)
            {
                return null;
            }

            string typeFullName = t.Substring(1, t.Length - 2);

            return Type.GetType(typeFullName);
        }

        public override object Populate_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings)
        {
            string t = parseState.CurrentToken.ToString();

            parseState.CurrentTokenIndex++;

            if (t == BJsonUtils.NULL_IDENTIFIER)
            {
                return null;
            }

            string typeFullName = t.Substring(1, t.Length - 2);

            return Type.GetType(typeFullName);
        }

        public override void Serialize_NonFormatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (instance == null)
            {
                jsonBuilder.Append(BJsonUtils.NULL_IDENTIFIER);
                return;
            }

            jsonBuilder.Append('"');
            jsonBuilder.Append(((Type)instance).AssemblyQualifiedName);
            jsonBuilder.Append('"');

        }
        public override void Serialize_Formatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings)
        {
            if (instance == null)
            {
                jsonBuilder.Append(BJsonUtils.NULL_IDENTIFIER);
                return;
            }

            jsonBuilder.Append('"');
            jsonBuilder.Append(((Type)instance).AssemblyQualifiedName);
            jsonBuilder.Append('"');
        }
    }
}
