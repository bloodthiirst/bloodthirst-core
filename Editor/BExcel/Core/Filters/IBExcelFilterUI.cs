using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BExcelEditor
{
    public interface IBExcelFilterUI
    {
        VisualElement Bind(IBExcelFilter filter, BExcel editor);
        void Unbind();

    }
}