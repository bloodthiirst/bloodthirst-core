namespace Bloodthirst.Core.Tokenizer
{
    public interface IToken
    {
        public int StartIndex { get; set; }
        public string SourceText { get; set; }
    }
}
