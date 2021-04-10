namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class SemiColonToken : TokenBase
    {

    }

    public class SemiColonTokenProcessor : FixedTermTokenProcessorBase<SemiColonToken, TokenBase>
    {
        private const string FIXED_TERM = ";";

        public override string FixedTerm => FIXED_TERM;
    }
}
