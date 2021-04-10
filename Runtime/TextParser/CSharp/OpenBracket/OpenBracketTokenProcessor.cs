namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class OpenBracketToken : TokenBase
    {

    }

    public class OpenBracketTokenProcessor : FixedTermTokenProcessorBase<OpenBracketToken, TokenBase>
    {
        private const string FIXED_TERM = "[";

        public override string FixedTerm => FIXED_TERM;
    }
}
