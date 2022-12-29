using System.Collections.Generic;

namespace Bloodthirst.Editor.CodeGenerator
{
    public class TemplateCodeBuilder
    {
        private readonly string templateTxt;
        private List<ITextSection> sections;

        public TemplateCodeBuilder(string templateTxt)
        {
            this.templateTxt = templateTxt;
            sections = new List<ITextSection>();
        }

        public TTextSection CreateSection<TTextSection>(TTextSection section) where TTextSection : ITextSection
        {
            section.Parent = this;
            sections.Add(section);
            return section;
        }

        public string Build()
        {
            string resultText = templateTxt;

            for (int i = 0; i < sections.Count; i++)
            {
                ITextSection section = sections[i];
                resultText = section.Write(resultText);

            }

            return resultText;
        }
    }
}

