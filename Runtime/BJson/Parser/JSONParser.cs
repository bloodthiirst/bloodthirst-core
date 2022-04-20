using Bloodthirst.BJson;

public class JSONParser
{
    private static JSONParser instance;
    public static JSONParser Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new JSONParser();
            }

            return instance;
        }
    }

    public bool SkipEmptySpace { get; set; }
    public TokenizerState<JSONTokenType> TokenizeString(string txt)
    {
        TokenizerState<JSONTokenType> state = new TokenizerState<JSONTokenType>(txt);

        bool ended = false;

        while (!ended)
        {
            ended = Tokenize(ref state);
        }

        return state;
    }

    private bool ConsumeIdentifier(ref TokenizerState<JSONTokenType> state, JSONTokenType tokenType = JSONTokenType.IDENTIFIER)
    {
        if (state.StartIndex == state.CurrentIndex)
            return false;

        int length = state.CurrentIndex - state.StartIndex;

        // null check
        if (length == 4 &&
            state.Text[state.StartIndex] == 'n' &&
            state.Text[state.StartIndex + 1] == 'u' &&
            state.Text[state.StartIndex + 2] == 'l' &&
            state.Text[state.StartIndex + 3] == 'l'
            )
        {
            tokenType = JSONTokenType.NULL;
        }

        Token<JSONTokenType> token = new Token<JSONTokenType>()
        {
            StartIndex = state.StartIndex,
            Length = length,
            Text = state.Text,
            TokenType = tokenType
        };

        state.Tokens.Add(token);

        state.StartIndex = state.CurrentIndex;

        return true;
    }

    public bool Tokenize(ref TokenizerState<JSONTokenType> state)
    {
        string txt = state.Text;

        // txt ended
        if (state.CurrentIndex > txt.Length - 1)
        {
            ConsumeIdentifier(ref state);
            return true;
        }

        char curr = txt[state.CurrentIndex];

        switch (curr)
        {
            case '{':
                if (state.IsInsideString)
                {
                    state.CurrentIndex++;
                    return false;
                }

                ConsumeIdentifier(ref state);


                state.Tokens.Add(new Token<JSONTokenType>() { Length = 1, StartIndex = state.CurrentIndex, Text = txt, TokenType = JSONTokenType.OBJECT_START });

                state.ObjectBrackets++;
                state.CurrentIndex++;
                state.StartIndex++;
                return false;
            case '}':

                if (state.IsInsideString)
                {
                    state.CurrentIndex++;
                    return false;

                }
                ConsumeIdentifier(ref state);



                state.Tokens.Add(new Token<JSONTokenType>() { Length = 1, StartIndex = state.CurrentIndex, Text = txt, TokenType = JSONTokenType.OBJECT_END });

                state.ObjectBrackets--;
                state.CurrentIndex++;
                state.StartIndex++;
                return false;
            case '[':

                if (state.IsInsideString)
                {
                    state.CurrentIndex++;
                    return false;
                }

                ConsumeIdentifier(ref state);


                state.Tokens.Add(new Token<JSONTokenType>() { Length = 1, StartIndex = state.CurrentIndex, Text = txt, TokenType = JSONTokenType.ARRAY_START });
                state.ArrayBrackets++;
                state.CurrentIndex++;
                state.StartIndex++;
                return false;
            case ']':

                if (state.IsInsideString)
                {
                    state.CurrentIndex++;
                    return false;
                }

                ConsumeIdentifier(ref state);

                state.Tokens.Add(new Token<JSONTokenType>() { Length = 1, StartIndex = state.CurrentIndex, Text = txt, TokenType = JSONTokenType.ARRAY_END });

                state.ArrayBrackets--;
                state.CurrentIndex++;
                state.StartIndex++;
                return false;
            case '"':
                state.IsInsideString = !state.IsInsideString;
                state.CurrentIndex++;

                if (state.IsInsideString)
                {
                    return false;
                }

                ConsumeIdentifier(ref state, JSONTokenType.STRING);
                return false;

            case ' ':
                if (state.IsInsideString)
                {
                    state.CurrentIndex++;
                    return false;
                }

                ConsumeIdentifier(ref state);

                state.Tokens.Add(new Token<JSONTokenType>() { Length = 1, StartIndex = state.CurrentIndex, Text = txt, TokenType = JSONTokenType.SPACE });

                state.CurrentIndex++;
                state.StartIndex++;
                return false;
            case '\t':

                if (state.IsInsideString)
                {
                    state.CurrentIndex++;
                    return false;
                }

                ConsumeIdentifier(ref state);


                state.Tokens.Add(new Token<JSONTokenType>() { Length = 1, StartIndex = state.CurrentIndex, Text = txt, TokenType = JSONTokenType.SPACE });

                state.CurrentIndex++;
                state.StartIndex++;
                return false;
            case '\n':
                if (state.IsInsideString)
                {
                    state.CurrentIndex++;
                    return false;
                }

                ConsumeIdentifier(ref state);

                state.Tokens.Add(new Token<JSONTokenType>() { Length = 1, StartIndex = state.CurrentIndex, Text = txt, TokenType = JSONTokenType.NEW_LINE });

                state.CurrentIndex++;
                state.StartIndex++;
                return false;
            case '\r':

                if (state.IsInsideString)
                {
                    state.CurrentIndex++;
                    return false;
                }

                ConsumeIdentifier(ref state);


                state.Tokens.Add(new Token<JSONTokenType>() { Length = 1, StartIndex = state.CurrentIndex, Text = txt, TokenType = JSONTokenType.NEW_LINE });

                state.CurrentIndex++;
                state.StartIndex++;
                return false;

            case ':':

                if (state.IsInsideString)
                {
                    state.CurrentIndex++;
                    return false;
                }

                ConsumeIdentifier(ref state);

                state.Tokens.Add(new Token<JSONTokenType>() { Length = 1, StartIndex = state.CurrentIndex, Text = txt, TokenType = JSONTokenType.COLON });

                state.CurrentIndex++;
                state.StartIndex++;
                return false;
            case ',':
                if (state.IsInsideString)
                {
                    state.CurrentIndex++;
                    return false;
                }

                ConsumeIdentifier(ref state);

                state.Tokens.Add(new Token<JSONTokenType>() { Length = 1, StartIndex = state.CurrentIndex, Text = txt, TokenType = JSONTokenType.COMMA });

                state.CurrentIndex++;
                state.StartIndex++;
                return false;
        }

        // any other character
        state.CurrentIndex++;

        return false;
    }

}
