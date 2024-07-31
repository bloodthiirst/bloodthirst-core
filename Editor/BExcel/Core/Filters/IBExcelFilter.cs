using Bloodthirst.Core.Utils;
using OfficeOpenXml;
using System;
using System.IO;
using UnityEditor;

namespace Bloodthirst.Editor.BExcelEditor
{
    public struct BExcelContext
    {
        public UnityEngine.Object CurrentAsset;
        public ExcelPackage CurrentExcelFile;

        public static BExcelContext Create(UnityEngine.Object asset)
        {
            string fullPath = EditorUtils.RelativeToAbsolutePath(AssetDatabase.GetAssetPath(asset));
            byte[] textAsBytes = File.ReadAllBytes(fullPath);

            using (MemoryStream currentMemoryStream = new MemoryStream(textAsBytes))
            {
                ExcelPackage excel = new ExcelPackage(currentMemoryStream);
                excel.Save();

                return new BExcelContext()
                {
                    CurrentAsset = asset,
                    CurrentExcelFile = excel
                };
            }
        }
    }

    public interface IBExcelFilter
    {
        event Action<IBExcelFilter> OnFilterChanged;
        void Setup(BExcelContext excel);
        IBExcelFilterUI CreatetUI();
        bool IsValid(out string errText);
        void Execute();
        void Clean();

    }
}