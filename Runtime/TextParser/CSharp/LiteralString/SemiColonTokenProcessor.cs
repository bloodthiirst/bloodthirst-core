namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class LiteralStringToken : TokenBase
    {

    }

    // TODO : make interpolated string with $
    public class LiteralStringTokenProcessor : TokenProcessorBase<LiteralStringToken, TokenBase>
    {
        public override bool IsValid(string text, int currentIndex)
        {
            if (text[currentIndex] != '"')
                return false;

            for (int i = currentIndex + 1; i < text.Length; i++)
            {
                if (text[i] == '"')
                {
                    return true;
                }
            }

            return false;
        }

        protected override LiteralStringToken GetToken(string text, int currentIndex, out int indexOffset)
        {
            int length = 1;

            for (int i = currentIndex + 1; i < text.Length; i++)
            {
                length++;

                if (text[i] == '"')
                {
                    break;
                }
            }

            indexOffset = length;
            return new LiteralStringToken() { SourceText = text.Substring(currentIndex, length), StartIndex = currentIndex };
        }
    }
}
