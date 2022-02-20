namespace Bloodthirst.BJson
{
    internal static class ParserUtils
    {
        #region utils
        internal static void RemoveSpaces(ref ParserState<JSONTokenType> state)
        {
            // remove spaces
            for (int i = state.Tokens.Count - 1; i >= 0; i--)
            {
                Token<JSONTokenType> token = state.Tokens[i];

                if (token.TokenType == JSONTokenType.SPACE || token.TokenType == JSONTokenType.NEW_LINE)
                {
                    state.Tokens.RemoveAt(i);
                }
            }
        }
        internal static bool IsPropertyEndInArray(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.ARRAY_END || token.TokenType == JSONTokenType.COMMA;
        }

        internal static bool IsPropertyEndObject(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.OBJECT_END || token.TokenType == JSONTokenType.COMMA;
        }

        internal static bool IsSkippableSpace(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.SPACE || token.TokenType == JSONTokenType.NEW_LINE;
        }

        internal static bool IsComma(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.COMMA;
        }

        internal static bool IsColon(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.COLON;
        }

        #endregion
    }
}
