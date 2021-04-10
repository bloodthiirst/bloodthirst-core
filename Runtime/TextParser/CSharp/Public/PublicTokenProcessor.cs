namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class PublicToken : TokenBase
    {

    }

    public class PublicTokenProcessor : FixedTermTokenProcessorBase<PublicToken , TokenBase>
    {
        private const string FIXED_TERM = "public";

        public override string FixedTerm => FIXED_TERM;
    }
}
