namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class NewToken : TokenBase
    {


    }
    public class NewTokenTokenProcessor : FixedTermTokenProcessorBase<NewToken, TokenBase>
    {
        private const string FIXED_TERM = "new";

        public override string FixedTerm => FIXED_TERM;
    }
}
