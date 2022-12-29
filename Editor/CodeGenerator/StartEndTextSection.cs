using Bloodthirst.Core.Utils;
using System.Collections.Generic;

namespace Bloodthirst.Editor.CodeGenerator
{
    public class StartEndTextSection : ITextSection
    {
        private readonly string start;
        private readonly string end;
        private List<ITextWriter> writers;
        public TemplateCodeBuilder Parent { get; set; }
        public IReadOnlyList<ITextWriter> Writers => writers;

        public string Start => start;
        public string End => end;

        public StartEndTextSection(string start, string end)
        {
            this.start = start;
            this.end = end;
            writers = new List<ITextWriter>();
        }

        public string Write(string sourceText)
        {
            List<StringExtensions.SectionInfo> propsSections = sourceText.StringReplaceSection(start, end);

            int padding = 0;

            for (int i = 0; i < propsSections.Count - 1; i++)
            {
                StringExtensions.SectionInfo start = propsSections[i];
                StringExtensions.SectionInfo end = propsSections[i + 1];

                // if we have correct start and end
                // then do the replacing
                if (!(start.sectionEdge == SECTION_EDGE.START && end.sectionEdge == SECTION_EDGE.END))
                    continue;

                int length = end.startIndex - start.endIndex;
                string section = sourceText.Substring(start.endIndex, length);

                for (int w = 0; w < writers.Count; w++)
                {
                    section = writers[w].Write(section);
                }

                sourceText = sourceText.ReplaceBetween(start.endIndex, end.startIndex, section);

                int oldTextLength = end.startIndex - start.endIndex;

                padding += section.Length - oldTextLength;
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