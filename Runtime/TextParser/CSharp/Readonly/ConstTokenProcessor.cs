namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class ReadonlyToken : TokenBase
    {

    }

    public class ReadonlyTokenProcessor : FixedTermTokenProcessorBase<ReadonlyToken, TokenBase>
    {
        private const string FIXED_TERM = "readonly";

        public override string FixedTerm => FIXED_TERM;
    }
}
