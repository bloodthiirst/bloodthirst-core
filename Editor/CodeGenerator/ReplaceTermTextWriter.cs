namespace Bloodthirst.Editor.CodeGenerator
{
    public class ReplaceTermTextWriter : ITextWriter
    {
        public string Term { get; }
        public string ReplaceBy { get; }
        public ReplaceTermTextWriter(string term, string replaceBy)
        {
            Term = term;
            ReplaceBy = replaceBy;
        }

        string ITextWriter.Write(string originalText)
        {
            return originalText.Replace(Term, ReplaceBy);
        }
    }
}