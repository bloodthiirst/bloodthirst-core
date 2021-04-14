namespace Bloodthirst.Core.Tokenizer
{
    public abstract class TokenProcessorBase<TToken , TBase> : TokenProcessorBase<TBase> where TToken : TBase where TBase : IToken
    {
        public override TBase GetBaseToken(string text, int currentIndex, out int indexOffset)
        {
            return GetToken(text, currentIndex, out indexOffset);
        }

        protected abstract TToken GetToken(string text, int currentIndex, out int indexOffset);
    }

    public abstract class TokenProcessorBase<TBase> : ITokenProcessor where TBase : IToken
    {
        public abstract bool IsValid(string text, int currentIndex);
        public abstract TBase GetBaseToken(string text, int currentIndex, out int indexOffset);

        IToken ITokenProcessor.GetToken(string text, int currentIndex, out int indexOffset)
        {
            return GetBaseToken(text, currentIndex, out indexOffset);
        }

        bool ITokenProcessor.IsValid(string text, int currentIndex)
        {
            return IsValid(text, currentIndex);
        }
    }
}
