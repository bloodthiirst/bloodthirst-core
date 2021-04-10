namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class NamespaceToken : TokenBase
    {


    }
    public class NamespaceTokenProcessor : FixedTermTokenProcessorBase<NamespaceToken, TokenBase>
    {
        private const string FIXED_TERM = "namespace";

        public override string FixedTerm => FIXED_TERM;
    }
}
