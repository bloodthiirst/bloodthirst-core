using System.Collections.Generic;

namespace Bloodthirst.Editor.CodeGenerator
{
    public interface ITextSection
    {
        TemplateCodeBuilder Parent { get; set; }
        IReadOnlyList<ITextWriter> Writers { get; }
        ITextSection AddWriter(ITextWriter writer);
        string Write(string resultText);
    }

}