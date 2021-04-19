using Bloodthirst.Core.Tokenizer.CSharp;
using Bloodthirst.Core.Utils;
using System.Collections.Generic;
using System.Text;

namespace Bloodthirst.Core.Tokenizer
{
    public class TextTokenizer<TToken, TCustom> where TToken : IToken where TCustom : TToken, new()
    {
        private static List<char> SkippableWhiteSpace = new List<char>()
        {
            ' ',
            '\t',
            '\n',
            '\b',
            '\r'
        };

        public void Tokenize(string text, List<TokenProcessorBase<TToken>> processors, List<TToken> tokens)
        {
            tokens = tokens.CreateOrClear();
            int i = 0;
            int offset;

            StringBuilder customIdentifierStack = new StringBuilder();

            while (i < text.Length)
            {
                char currentchar = text[i];

                if (SkippableWhiteSpace.Contains(currentchar))
                {
                    if (customIdentifierStack.Length != 0)
                    {
                        TToken custom = new TCustom() { SourceText = customIdentifierStack.ToString(), StartIndex = i - customIdentifierStack.Length };
                        tokens.Add(custom);

                        customIdentifierStack.Clear();
                    }

                    i++;
                    continue;
                }

                bool tokenFound = false;

                foreach (TokenProcessorBase<TToken> pro in processors)
                {
                    if (!pro.IsValid(text, i))
                    {
                        continue;
                    }

                    if (customIdentifierStack.Length != 0)
                    {
                        TToken custom = new TCustom() { SourceText = customIdentifierStack.ToString(), StartIndex = i - customIdentifierStack.Length };
                        tokens.Add(custom);

                        customIdentifierStack.Clear();
                    }

                    TToken token = pro.GetBaseToken(text, i, out offset);
                    tokens.Add(token);

                    i += offset;

                    tokenFound = true;

                    break;
                }

                if (tokenFound)
                {
                    continue;
                }

                customIdentifierStack.Append(text[i]);
                i++;
            }


        }
    }
}
