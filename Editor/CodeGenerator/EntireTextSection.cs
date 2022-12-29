using System.Collections.Generic;

namespace Bloodthirst.Editor.CodeGenerator
{
    public class EntireTextSection : ITextSection
    {
        private List<ITextWriter> writers;
        public TemplateCodeBuilder Parent { get; set; }
        public IReadOnlyList<ITextWriter> Writers => writers;

        public EntireTextSection()
        {
            writers = new List<ITextWriter>();
        }

        public string Write(string sourceText)
        {
            for (int w = 0; w < writers.Count; w++)
            {
                sourceText = writers[w].Write(sourceText);
            }

            return sourceText;
        }

        public ITextSection AddWriter(ITextWriter writer)
        {
            writers.Add(writer);
            return this;
        }

        public TemplateCodeBuilder Done()
        {
            return Parent;
        }
    }

}