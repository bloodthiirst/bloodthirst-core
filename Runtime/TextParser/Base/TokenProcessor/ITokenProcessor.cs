namespace Bloodthirst.Core.Tokenizer
{
    public interface ITokenProcessor
    {
        bool IsValid(string text, int currentIndex);

        IToken GetToken(string text, int currentIndex, out int indexOffset);
    }
}
