namespace Bloodthirst.Editor.CodeGenerator
{
    public class AppendTextWriter : ITextWriter
    {
        private readonly string text;

        public AppendTextWriter(string text)
        {
            this.text = text;
        }

        string ITextWriter.Write(string originalText)
        {
            return originalText + text;
        }
    }

}