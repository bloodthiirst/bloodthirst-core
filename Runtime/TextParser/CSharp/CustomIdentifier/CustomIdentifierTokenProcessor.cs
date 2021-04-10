namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class CustomIdentifierToken : TokenBase
    {


    }
    public class CustomIdentifierTokenProcessor : TokenProcessorBase<CustomIdentifierToken, TokenBase>
    {
        public override bool IsValid(string text, int currentIndex)
        {
            return true;
        }

        protected override CustomIdentifierToken GetToken(string text, int currentIndex, out int indexOffset)
        {
            indexOffset = 0;
            return null;
            //return new CustomIdentifierToken() { SourceText = text.Substring(cu)}
        }
    }
}
