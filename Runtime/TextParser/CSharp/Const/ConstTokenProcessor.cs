namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class ConstToken : TokenBase
    {

    }

    public class ConstTokenProcessor : FixedTermTokenProcessorBase<ConstToken, TokenBase>
    {
        private const string FIXED_TERM = "const";

        public override string FixedTerm => FIXED_TERM;
    }
}
