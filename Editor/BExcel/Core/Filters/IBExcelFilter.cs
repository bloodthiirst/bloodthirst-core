using OfficeOpenXml;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BSearch
{
    public interface IBExcelFilter
    {
        event Action<IBExcelFilter> OnFilterChanged;
        void Setup(ExcelPackage excelFile);
        IBExcelFilterUI CreatetUI();
        bool IsValid(out string errText);
        void Execute();
        void Clean();

    }
}