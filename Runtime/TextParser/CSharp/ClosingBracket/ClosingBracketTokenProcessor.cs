namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class ClosingBracketToken : TokenBase
    {

    }

    public class ClosingBracketTokenProcessor : FixedTermTokenProcessorBase<ClosingBracketToken, TokenBase>
    {
        private const string FIXED_TERM = "]";

        public override string FixedTerm => FIXED_TERM;
    }
}
