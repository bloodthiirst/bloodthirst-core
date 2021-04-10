namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class ClosingCurlyBraceToken : TokenBase
    {

    }

    public class ClosingCurlyBraceTokenProcessor : FixedTermTokenProcessorBase<ClosingCurlyBraceToken, TokenBase>
    {
        private const string FIXED_TERM = "}";

        public override string FixedTerm => FIXED_TERM;
    }
}
