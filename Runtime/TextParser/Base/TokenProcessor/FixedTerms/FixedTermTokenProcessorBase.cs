namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public abstract class FixedTermTokenProcessorBase<TToken, TBase> : TokenProcessorBase<TToken, TBase>
        where TToken : TBase, new()
        where TBase : IToken
    {
        public abstract string FixedTerm { get; }

        protected override TToken GetToken(string text, int currentIndex, out int indexOffset)
        {
            indexOffset = FixedTerm.Length;

            return new TToken() { SourceText = text.Substring(currentIndex, indexOffset), StartIndex = currentIndex };
        }
        public override TBase GetBaseToken(string text, int currentIndex, out int indexOffset)
        {
            return GetToken(text, currentIndex, out indexOffset);
        }

        public override bool IsValid(string text, int currentIndex)
        {
            if (text.Length - currentIndex < FixedTerm.Length)
                return false;

            for (int i = 0; i < FixedTerm.Length; i++)
            {
                if (text[currentIndex + i] != FixedTerm[i])
                    return false;
            }

            return true;
        }
    }
}
