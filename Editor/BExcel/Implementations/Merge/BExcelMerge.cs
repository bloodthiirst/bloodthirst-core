using Bloodthirst.Core.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Bloodthirst.Editor.BSearch
{

    [BExcelFilterName("Excel Merger")]
    public class BExcelMerge : IBExcelFilter
    {
        private const string OLD_POST_FIX = "_old";

        public event Action<IBExcelFilter> OnFilterChanged = null;

        private byte[] ExcelFileAsData { get; set; }

        internal string ExportPath { get; set; }

        internal string OriginalTab { get; set; }

        internal List<string> DuplicateTabs { get; set; } = new List<string>();

        internal void NotifyFilterChanged()
        {
            OnFilterChanged?.Invoke(this);
        }

        void IBExcelFilter.Setup(ExcelPackage excelFile)
        {;
            ExcelFileAsData = excelFile.GetAsByteArray();
        }
        bool IBExcelFilter.IsValid(out string errText)
        {
            if (string.IsNullOrEmpty(ExportPath))
            {
                errText = "Please set a valid export path";
                return false;
            }

            if (DuplicateTabs.Count == 0)
            {
                errText = "You need to have at lease 1 duplicate tab to merge";
                return false;
            }

            List<string> tabsNames = new List<string>();
            tabsNames.Add(OriginalTab);
            tabsNames.AddRange(DuplicateTabs);

            List<string> unique = tabsNames.Distinct().ToList();

            if (unique.Count != tabsNames.Count)
            {
                errText = "All the tabs used have to be unique";
                return false;
            }

            errText = string.Empty;
            return true;
        }

        void IBExcelFilter.Execute()
        {
            byte[] result = GenerateCleanExcel();
            ExportExcelFile(result);
        }

        private void ExportExcelFile(byte[] bytesCopy)
        {
            string absolutePath = EditorUtils.RelativeToAbsolutePath(ExportPath);

            using (FileStream fileStream = new FileStream(absolutePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fileStream.Write(bytesCopy, 0, bytesCopy.Length);
            }

            AssetDatabase.Refresh();

            string relativePath = ExportPath;

            EditorUtility.DisplayDialog("Merge result", "Success", "Ok");

            if (relativePath == null)
            {
                return;
            }

            UnityEngine.Object fileInProject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(relativePath);

            EditorGUIUtility.PingObject(fileInProject);
        }

        private byte[] GenerateCleanExcel()
        {
            byte[] bytesCopy = new byte[ExcelFileAsData.Length];
            ExcelFileAsData.CopyTo(bytesCopy, 0);

            MemoryStream memCopy = new MemoryStream(bytesCopy);
            ExcelPackage safeCopy = new ExcelPackage(memCopy);

            ExcelWorksheet[] worksheets = new ExcelWorksheet[DuplicateTabs.Count + 1];

            worksheets[0] = safeCopy.Workbook.Worksheets[OriginalTab];

            ExcelWorksheet originalSheet = worksheets[0];

            for (int i = 0; i < DuplicateTabs.Count; i++)
            {
                string tabName = DuplicateTabs[i];
                worksheets[i + 1] = safeCopy.Workbook.Worksheets[tabName];
            }


            // stack all the key/values here
            Dictionary<string, List<List<ExcelRange>>> keysValueContainer = new Dictionary<string, List<List<ExcelRange>>>();

            int totalColumns = originalSheet.Dimension.Columns;

            for (int i = 0; i < worksheets.Length; i++)
            {
                ExcelWorksheet curr = worksheets[i];

                // rows
                // start from 2 to skip header
                // note : indexes start from 1 apparently , fking dumb idea
                for (int row = 2; row < curr.Dimension.Rows + 1; row++)
                {
                    ExcelRange key = curr.Cells[row, 1];

                    string keyVal = key.Text;

                    // skip empty keys
                    if (string.IsNullOrEmpty(keyVal) || string.IsNullOrWhiteSpace(keyVal))
                        continue;

                    keyVal = keyVal.Trim();

                    // remove the _Old from the end of keys
                    while (keyVal.ToLower().EndsWith(OLD_POST_FIX))
                    {
                        keyVal = keyVal.Remove(keyVal.Length - 1 - OLD_POST_FIX.Length, OLD_POST_FIX.Length);
                    }

                    if (!keysValueContainer.TryGetValue(keyVal, out List<List<ExcelRange>> list))
                    {
                        list = new List<List<ExcelRange>>();
                        keysValueContainer.Add(keyVal, list);
                    }

                    // store all the row values
                    List<ExcelRange> rowCells = new List<ExcelRange>();

                    for (int x = 1; x < totalColumns + 1; x++)
                    {
                        rowCells.Add(curr.Cells[row, x]);
                    }

                    // add to the list of dulicate rows for the current key
                    list.Add(rowCells);
                }
            }

            int totalRows = keysValueContainer.Select(kv => kv.Value.Count).Sum();

            // create result worksheet
            string mergedsheetName = OriginalTab + "_Merged";
            ExcelWorksheet mergedSheet = safeCopy.Workbook.Worksheets.Add(mergedsheetName);
            safeCopy.Workbook.Worksheets.MoveBefore(mergedsheetName, OriginalTab);

            // create rows/cols
            mergedSheet.InsertRow(1, totalRows);
            mergedSheet.InsertColumn(1, totalColumns);

            // copy cols width
            for (int i = 1; i < totalColumns + 1; i++)
            {
                mergedSheet.Column(i).Width = originalSheet.Column(i).Width;
            }

            
            // copy tab color
            Color col = originalSheet.TabColor;
            mergedSheet.TabColor = col;
            

            // copy header
            for (int x = 1; x < totalColumns + 1; x++)
            {
                originalSheet.Cells[1, x].Copy(mergedSheet.Cells[1, x]);
            }

            int rowIndex = 2;

            // flatten the cells
            foreach (KeyValuePair<string, List<List<ExcelRange>>> kv in keysValueContainer)
            {
                // start with creating the key
                string key = kv.Key;

                for (int o = 0; o < kv.Value.Count; o++)
                {
                    List<ExcelRange> cellsToCopy = kv.Value[o];

                    // append the _Old at the end
                    for (int r = 0; r < o; r++)
                    {
                        key += "_Old";
                    }

                    // do the actual copying
                    for (int c = 0; c < cellsToCopy.Count; c++)
                    {
                        cellsToCopy[c].Copy(mergedSheet.Cells[rowIndex, c + 1]);
                    }

                    rowIndex++;
                }
            }

            mergedSheet.Cells[2, 1, totalRows , totalColumns].Sort(1 , true);

            bytesCopy = safeCopy.GetAsByteArray();

            // clean up
            memCopy.Close();
            memCopy.Dispose();

            safeCopy.Dispose();

            return bytesCopy;
        }

        IBExcelFilterUI IBExcelFilter.CreatetUI()
        {
            return new BExcelMergeUI();
        }

        void IBExcelFilter.Clean()
        {
            OnFilterChanged = null;
            ExcelFileAsData = null;
        }
    }
}