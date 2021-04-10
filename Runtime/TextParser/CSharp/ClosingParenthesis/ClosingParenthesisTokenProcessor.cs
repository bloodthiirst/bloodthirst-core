namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class ClosingParenthesisToken : TokenBase
    {

    }

    public class ClosingParenthesisTokenProcessor : FixedTermTokenProcessorBase<ClosingParenthesisToken, TokenBase>
    {
        private const string FIXED_TERM = ")";

        public override string FixedTerm => FIXED_TERM;
    }
}
