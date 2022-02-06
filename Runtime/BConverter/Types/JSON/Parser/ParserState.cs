using System;
using System.Collections.Generic;

public struct ParserState<TTokenType> where TTokenType : System.Enum
{
    public ParserState(TokenizerState<TTokenType> tokenizer)
    {
        Tokens = tokenizer.Tokens;
        CurrentTokenIndex = 0;
    }

    public int CurrentTokenIndex { get; set; }

    public List<Token<TTokenType>> Tokens { get; set; }

    public Token<TTokenType> CurrentToken
    {
        get => Tokens[CurrentTokenIndex];
    }

    public void SkipWhile(Predicate<Token<TTokenType>> skip)
    {
        // skip space
        while (skip(Tokens[CurrentTokenIndex]))
        {
            CurrentTokenIndex++;
        }
    }

    public void SkipUntil(Predicate<Token<TTokenType>> stopSkipping)
    {
        // skip space
        while (!stopSkipping(Tokens[CurrentTokenIndex]))
        {
            CurrentTokenIndex++;
        }
    }
}
