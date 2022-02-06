
using System.Diagnostics;

[DebuggerDisplay("{AsString(),nq}")]
public struct Token<TTokenType> where TTokenType : System.Enum
{
    public string Text { get; set; }
    public int StartIndex { get; set; }
    public int Length { get; set; }
    public TTokenType TokenType { get; set; }

    public string AsString()
    {
        return Text.Substring(StartIndex, Length);
    }
}