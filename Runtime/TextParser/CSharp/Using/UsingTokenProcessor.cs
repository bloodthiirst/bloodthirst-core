namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class UsingToken : TokenBase
    {


    }
    public class UsingTokenProcessor : FixedTermTokenProcessorBase<UsingToken, TokenBase>
    {
        private const string FIXED_TERM = "using";

        public override string FixedTerm => FIXED_TERM;
    }
}
