namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class PrivateToken : TokenBase
    {

    }

    public class PrivateTokenProcessor : FixedTermTokenProcessorBase<PrivateToken, TokenBase>
    {
        private const string FIXED_TERM = "private";

        public override string FixedTerm => FIXED_TERM;
    }
}
