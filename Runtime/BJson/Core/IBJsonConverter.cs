using System.Text;

namespace Bloodthirst.BJson
{
    public interface IBJsonConverter
    {
        object Deserialize(string json, BJsonContext context, BJsonSettings settings);
        string Serialize(object t, BJsonContext context, BJsonSettings settings);
        object Populate(object instance, string json, BJsonContext context, BJsonSettings settings);
    }

    public interface IBJsonConverterInternal : IBJsonConverter
    {
        BJsonProvider Provider { get; set; }
        void Initialize();
        object CreateInstance_Internal();
        object Deserialize_Internal(ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings);
        object Populate_Internal(object instance, ref ParserState<JSONTokenType> parseState, BJsonContext context, BJsonSettings settings);
        void Serialize_Formatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings);
        void Serialize_NonFormatted_Internal(object instance, StringBuilder jsonBuilder, BJsonContext context, BJsonSettings settings);
    }
}
