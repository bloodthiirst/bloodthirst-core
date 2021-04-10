namespace Bloodthirst.Core.Tokenizer
{
    public abstract class TokenBase : IToken
    {
        public int StartIndex { get; set; }
        public string SourceText { get; set; }
    }
}
