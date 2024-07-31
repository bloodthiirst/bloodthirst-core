using System;

namespace Bloodthirst.Editor.BExcelEditor
{
    public interface IBExcelFilter
    {
        event Action<IBExcelFilter> OnFilterChanged;
        void Setup(BExcel excel);
        IBExcelFilterUI CreatetUI();
        bool IsValid(out string errText);
        void Execute();
        void Clean();

    }
}