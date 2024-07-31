using Bloodthirst.Core.Utils;
using NUnit.Framework;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace Bloodthirst.Editor.BExcelEditor
{

    /// <summary>
    /// <para>Excel filter made for merging results from multiple sheets into a single one</para>
    /// <para>The filter groups all the rows by key</para>
    /// </summary>
    [BExcelFilterName("Excel Export")]
    public class BExcelExport : IBExcelFilter
    {
        public event Action<IBExcelFilter> OnFilterChanged = null;

        public BExcelContext Editor { get; private set; }

        /// <summary>
        /// Copy of the input Excel file in bytes
        /// </summary>
        private MemoryStream SourceAsStream { get; set; }
        public string AssetExportPath { get; set; }
        public string ScriptExportPath { get; set; }
        public string Namespace { get; set; }

        public void NotifyFilterChanged()
        {
            OnFilterChanged?.Invoke(this);
        }

        void IBExcelFilter.Setup(BExcelContext editor)
        {
            this.Editor = editor;
            SourceAsStream = new MemoryStream((int)Editor.CurrentExcelFile.Stream.Length);
            Editor.CurrentExcelFile.SaveAs(SourceAsStream);
        }

        bool IBExcelFilter.IsValid(out string errText)
        {
            if (string.IsNullOrEmpty(AssetExportPath))
            {
                errText = "Please set a valid export path";
                return false;
            }
            if (string.IsNullOrEmpty(Namespace))
            {
                errText = "Please set a valid namespace";
                return false;
            }

            errText = string.Empty;
            return true;
        }

        void IBExcelFilter.Execute()
        {
            using (ExcelPackage excel = new ExcelPackage(SourceAsStream))
            {
                excel.Save();

                BExcelOutput asset = ExcelToAsset(excel);

                ExportAsset(asset);
                ExportScript(asset);

                EditorUtility.DisplayDialog("Export result", "Success", "Ok");
            }
        }

        private BExcelOutput ExcelToAsset(ExcelPackage excel)
        {
            BExcelOutput asset = ScriptableObject.CreateInstance<BExcelOutput>();

            asset.scriptNamespace = Namespace;
            asset.scriptPath = ScriptExportPath;

            using (ListPool<string>.Get(out List<string> cols))
            using (ListPool<string>.Get(out List<string> uniqueCols))
            using (ListPool<string>.Get(out List<string> entries))
            using (ListPool<BExcelOutput.Row>.Get(out List<BExcelOutput.Row> rows))
            using (ListPool<BExcelOutput.Tab>.Get(out List<BExcelOutput.Tab> tabs))
            {
                int tabsCount = excel.Workbook.Worksheets.Count;

                foreach (ExcelWorksheet tab in excel.Workbook.Worksheets)
                {
                    for (int i = 0; i < tab.Dimension.Columns; ++i)
                    {
                        string headerCell = tab.Cells[1, i + 1].Text;
                        cols.Add(headerCell);
                    }
                }

                uniqueCols.AddRange(cols.Distinct());
                uniqueCols.Remove("Key");
                uniqueCols.Remove("Translated");
                uniqueCols.Remove("Notes");

                asset.languages = uniqueCols.ToArray();

                Assert.AreEqual(uniqueCols.Count, (cols.Count / tabsCount) - 3);

                foreach (ExcelWorksheet tab in excel.Workbook.Worksheets)
                {
                    BExcelOutput.Tab assetTab = new BExcelOutput.Tab() { tabName = tab.Name };
                    tabs.Add(assetTab);

                    rows.Clear();

                    for (int rowIndex = 0; rowIndex < tab.Dimension.Rows - 1; ++rowIndex)
                    {
                        string keyString = tab.Cells[rowIndex + 2, 1].Text;

                        if (string.IsNullOrEmpty(keyString))
                            continue;

                        BExcelOutput.Row row = new BExcelOutput.Row();
                        row.key = keyString;

                        rows.Add(row);

                        entries.Clear();
                        for (int colIndex = 0; colIndex < uniqueCols.Count; colIndex++)
                        {
                            string entryValue = tab.Cells[rowIndex + 2, colIndex + 2].Text;
                            entries.Add(entryValue);
                        }

                        row.entries = entries.ToArray();
                    }

                    assetTab.rows = rows.ToArray();
                }

                asset.tabs = tabs.ToArray();
            }

            return asset;
        }

        private void ExportScript(BExcelOutput output)
        {
            string relativePath = ScriptExportPath;
            string absolute = EditorUtils.RelativeToAbsolutePath(relativePath);

            string code = AssetToScript.Convert(output);
            File.WriteAllText(absolute, code);

            AssetDatabase.Refresh();
        }
        private void ExportAsset(BExcelOutput output)
        {
            string relativePath = AssetExportPath;

            AssetDatabase.CreateAsset(output, relativePath);

            AssetDatabase.Refresh();
        }

        IBExcelFilterUI IBExcelFilter.CreatetUI()
        {
            return new BExcelExportUI();
        }

        void IBExcelFilter.Clean()
        {
            SourceAsStream.Dispose();
            OnFilterChanged = null;
            SourceAsStream = null;
        }
    }
}