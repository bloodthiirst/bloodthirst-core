using System.Collections.Generic;

public struct TokenizerState<TTokenType> where TTokenType : System.Enum
{
    public TokenizerState(string text)
    {
        Text = text;
        ArrayBrackets = 0;
        ObjectBrackets = 0;
        StartIndex = 0;
        CurrentIndex = 0;
        IsInsideString = false;
        Tokens = new List<Token<TTokenType>>();
    }

    public string Text { get; set; }
    public int ArrayBrackets { get; set; }
    public int ObjectBrackets { get; set; }
    public int StartIndex { get; set; }
    public int CurrentIndex { get; set; }
    public bool IsInsideString { get; set; }
    public List<Token<TTokenType>> Tokens { get; set; }
    public bool IsValidJson()
    {
        return
            !IsInsideString &&
            ArrayBrackets == 0 &&
            ObjectBrackets == 0 &&
            CurrentIndex == StartIndex &&
            CurrentIndex == Text.Length;
    }
}
