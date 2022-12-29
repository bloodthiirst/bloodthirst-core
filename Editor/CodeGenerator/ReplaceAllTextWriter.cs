namespace Bloodthirst.Editor.CodeGenerator
{

    public class ReplaceAllTextWriter : ITextWriter
    {
        private readonly string text;

        public ReplaceAllTextWriter(string text)
        {
            this.text = text;
        }

        string ITextWriter.Write(string originalText)
        {
            return text;
        }
    }

}