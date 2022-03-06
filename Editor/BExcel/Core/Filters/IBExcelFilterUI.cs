using OfficeOpenXml;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BSearch
{
    public interface IBExcelFilterUI
    {
        VisualElement Bind(IBExcelFilter filter, ExcelPackage excelFile);
        void Unbind();

    }
}