using System.Text;

namespace Bloodthirst.BDeepCopy
{
    public interface IBJsonConverter : IBConverter
    {
        object From(string json, BConverterContext context, BConverterSettings settings);
        string To(object t, BConverterContext context, BConverterSettings settings);
        object Populate(object instance, string json, BConverterContext context, BConverterSettings settings);
    }

    public interface IBJsonConverterInternal : IBJsonConverter , IBConverterInternal
    {
        object CreateInstance_Internal();
        object From_Internal(object instance , ref ParserState<JSONTokenType> parseState, BConverterContext context, BConverterSettings settings);
        void To_Internal(object instance, StringBuilder jsonBuilder, BConverterContext context, BConverterSettings settings);
    }
}
