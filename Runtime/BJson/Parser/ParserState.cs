using System;
using System.Collections.Generic;

namespace Bloodthirst.BJson
{
    public struct ParserState<TTokenType> where TTokenType : Enum
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
            try
            {
                // skip space
                while (skip(Tokens[CurrentTokenIndex]))
                {
                    CurrentTokenIndex++;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void SkipUntil(Predicate<Token<TTokenType>> stopSkipping)
        {
            try
            {
                // skip space
                while (!stopSkipping(Tokens[CurrentTokenIndex]))
                {
                    CurrentTokenIndex++;
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
