namespace Bloodthirst.BDeepCopy
{
    internal interface IBJsonConverter : IBConverter
    {
        object From(string json);
        string To(object t);
        object Populate(object instance, string json);
    }

    internal interface IBJsonConverterInternal : IBJsonConverter , IBConverterInternal
    {
        object CreateInstance_Internal();
        object From_Internal(object instance , ref ParserState<JSONTokenType> parseState, BConverterContext context, BConverterSettings settings);
        string To_Internal(object instance, BConverterContext context, BConverterSettings settings);
    }
}
