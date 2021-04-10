namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class OpenParenthesisToken : TokenBase
    {

    }

    public class OpenParenthesisTokenProcessor : FixedTermTokenProcessorBase<OpenParenthesisToken, TokenBase>
    {
        private const string FIXED_TERM = "(";

        public override string FixedTerm => FIXED_TERM;
    }
}
