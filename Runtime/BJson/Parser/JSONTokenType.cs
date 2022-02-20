namespace Bloodthirst.BJson
{
    public enum JSONTokenType
    {
        OBJECT_START,
        OBJECT_END,

        ARRAY_START,
        ARRAY_END,

        COLON,

        COMMA,

        STRING,

        IDENTIFIER,
        NULL,

        SPACE,
        NEW_LINE
    }
}