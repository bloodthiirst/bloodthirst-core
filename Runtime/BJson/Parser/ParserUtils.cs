namespace Bloodthirst.BJson
{
    public static class ParserUtils
    {
        #region utils
        public static void RemoveSpaces(ref ParserState<JSONTokenType> state)
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
        public static bool IsPropertyEndInArray(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.ARRAY_END || token.TokenType == JSONTokenType.COMMA;
        }

        public static bool IsPropertyEndObject(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.OBJECT_END || token.TokenType == JSONTokenType.COMMA;
        }

        public static bool IsSkippableSpace(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.SPACE || token.TokenType == JSONTokenType.NEW_LINE;
        }

        public static bool IsComma(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.COMMA;
        }

        public static bool IsColon(Token<JSONTokenType> token)
        {
            return token.TokenType == JSONTokenType.COLON;
        }

        #endregion
    }
}
