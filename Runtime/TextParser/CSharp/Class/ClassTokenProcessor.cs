namespace Bloodthirst.Core.Tokenizer.CSharp
{
    public class ClassToken : TokenBase
    {


    }
    public class ClassTokenProcessor : FixedTermTokenProcessorBase<ClassToken, TokenBase>
    {
        private const string FIXED_TERM = "class";

        public override string FixedTerm => FIXED_TERM;
    }
}
