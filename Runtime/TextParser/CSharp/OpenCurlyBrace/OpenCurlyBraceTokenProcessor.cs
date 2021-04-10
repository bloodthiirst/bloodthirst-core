namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class OpenCurlyBraceToken : TokenBase
    {

    }

    public class OpenCurlyBraceTokenProcessor : FixedTermTokenProcessorBase<OpenCurlyBraceToken, TokenBase>
    {
        private const string FIXED_TERM = "{";

        public override string FixedTerm => FIXED_TERM;
    }
}
