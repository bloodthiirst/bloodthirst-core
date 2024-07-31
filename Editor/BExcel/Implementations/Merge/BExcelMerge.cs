using Bloodthirst.Core.Utils;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine.Pool;

namespace Bloodthirst.Editor.BExcelEditor
{

    /// <summary>
    /// <para>Excel filter made for merging results from multiple sheets into a single one</para>
    /// <para>The filter groups all the rows by key</para>
    /// </summary>
    [BExcelFilterName("Excel Merger")]
    public class BExcelMerge : IBExcelFilter
    {
        private const string OLD_POST_FIX = "_old";

        public event Action<IBExcelFilter> OnFilterChanged = null;

        public BExcelContext Editor { get; private set; }

        /// <summary>
        /// Copy of the input Excel file in bytes
        /// </summary>
        private MemoryStream SourceAsStream { get; set; }
        internal string ExportPath { get; set; }

        internal string OriginalTab { get; set; }
        internal UnityEngine.Color OriginalColor { get; set; }
        internal UnityEngine.Color DuplicateColor { get; set; }

        internal List<string> DuplicateTabs { get; set; } = new List<string>();

        internal void NotifyFilterChanged()
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

            using (ListPool<string>.Get(out List<string> tabsNames))
            using (ListPool<string>.Get(out List<string> uniqueNames))
            {
                tabsNames.Add(OriginalTab);
                tabsNames.AddRange(DuplicateTabs);

                uniqueNames.AddRange(tabsNames.Distinct());

                if (uniqueNames.Count != tabsNames.Count)
                {
                    errText = "All the tabs used have to be unique";
                    return false;
                }

                errText = string.Empty;
                return true;
            }
        }

        void IBExcelFilter.Execute()
        {
            using (ExcelPackage output = GenerateCleanExcel())
            {
                ExportExcelFile(output);
            }
        }

        private void ExportExcelFile(ExcelPackage output)
        {
            string absolutePath = EditorUtils.RelativeToAbsolutePath(ExportPath);

            FileInfo fileInfo = new FileInfo(absolutePath);
            output.SaveAs(fileInfo);

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

        private ExcelPackage GenerateCleanExcel()
        {
            // copy the input excel sheet
            ExcelPackage editableSheet = new ExcelPackage(SourceAsStream);
            editableSheet.Save();

            // we get an array of all the worksheets in question
            // with the original sheet at index 0
            ExcelWorksheet[] allTabs = new ExcelWorksheet[DuplicateTabs.Count + 1];
            ExcelWorksheet originalTab = null;
            {
                allTabs[0] = editableSheet.Workbook.Worksheets[OriginalTab];
                originalTab = allTabs[0];

                for (int i = 0; i < DuplicateTabs.Count; i++)
                {
                    string tabName = DuplicateTabs[i];
                    allTabs[i + 1] = editableSheet.Workbook.Worksheets[tabName];
                }
            }

            // get the total width of the tab (in columns)
            int totalColumns = originalTab.Dimension.Columns;

            // stack all the key/values here
            // At this step , we iterate over all the tabs and scan all the keys
            // the key value consist of :
            // Key => Key in the sheet
            // Value => Collection of rows that have the same key
            Dictionary<string, List<List<ExcelRange>>> keyToDuplicateRowsContainer = new Dictionary<string, List<List<ExcelRange>>>();
            {
                for (int i = 0; i < allTabs.Length; i++)
                {
                    ExcelWorksheet curr = allTabs[i];

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

                        // remove annoying spaces
                        keyVal = keyVal.Trim();

                        // remove the _Old from the end of keys
                        while (keyVal.ToLower().EndsWith(OLD_POST_FIX))
                        {
                            keyVal = keyVal.Remove(keyVal.Length - 1 - OLD_POST_FIX.Length, OLD_POST_FIX.Length);
                        }

                        if (!keyToDuplicateRowsContainer.TryGetValue(keyVal, out List<List<ExcelRange>> duplicatesList))
                        {
                            duplicatesList = new List<List<ExcelRange>>();
                            keyToDuplicateRowsContainer.Add(keyVal, duplicatesList);
                        }

                        // store all the row values
                        List<ExcelRange> dupliocateRowCells = new List<ExcelRange>();

                        for (int x = 1; x < totalColumns + 1; x++)
                        {
                            dupliocateRowCells.Add(curr.Cells[row, x]);
                        }

                        // add to the list of duplicate rows for the current key
                        duplicatesList.Add(dupliocateRowCells);
                    }
                }
            }

            // get the total number of ALL the rows found
            int totalRows = keyToDuplicateRowsContainer.Select(kv => kv.Value.Count).Sum();

            // create result worksheet
            string mergedsheetName = OriginalTab + "_Merged";
            ExcelWorksheet mergedSheet = editableSheet.Workbook.Worksheets.Add(mergedsheetName);
            editableSheet.Workbook.Worksheets.MoveBefore(mergedsheetName, OriginalTab);

            // create rows/cols
            mergedSheet.InsertRow(1, totalRows);
            mergedSheet.InsertColumn(1, totalColumns);

            // copy tab color
            {
                Color col = originalTab.TabColor;
                mergedSheet.TabColor = col;
            }

            // copy header
            for (int x = 1; x < totalColumns + 1; x++)
            {
                originalTab.Cells[1, x].Copy(mergedSheet.Cells[1, x]);
            }

            // we keep incrementing this index during the iteration
            // to stack all the merge results one after the other
            int rowIndex = 2;

            // flatten the cells
            // the idea is to go from the same key being present in multiple tabs
            // to a single tab that has all the rows grouped WITH clear distinction to show the duplicated ones
            // example :
            // we go from having the same <Key> in [Tab1] [Tab2] and [Tab_3]
            // to having <Key> <Key_old> <Key_old_old> all of them in [Tab1_Merged]
            foreach (KeyValuePair<string, List<List<ExcelRange>>> kv in keyToDuplicateRowsContainer)
            {
                // start with creating the key
                string key = kv.Key;

                for (int duplicateIndex = 0; duplicateIndex < kv.Value.Count; duplicateIndex++)
                {
                    List<ExcelRange> cellsToCopy = kv.Value[duplicateIndex];

                    // append the _Old at the end
                    for (int r = 0; r < duplicateIndex; r++)
                    {
                        key += "_Old";
                    }

                    // we assign the original color only to the entries of the original tab (which are always are index 0)
                    // the rest get the duplicate color
                    UnityEngine.Color bgColor = duplicateIndex == 0 ? OriginalColor : DuplicateColor;

                    Color excelColor = Color.FromArgb((int)(bgColor.a * 255), (int)(bgColor.r * 255), (int)(bgColor.g * 255), (int)(bgColor.b * 255));

                    // do the actual copying
                    for (int c = 0; c < cellsToCopy.Count; c++)
                    {
                        ExcelRange resultCell = mergedSheet.Cells[rowIndex, c + 1];
                        cellsToCopy[c].Copy(resultCell);
                        //resultCell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        //resultcell.style.fill.backgroundcolor.setcolor(excelcolor);
                    }

                    // assign the key
                    mergedSheet.Cells[rowIndex, 1].Value = key;

                    rowIndex++;
                }
            }

            // copy columns width
            for (int i = 1; i < totalColumns + 1; i++)
            {
                ExcelColumn merged = mergedSheet.Column(i);
                ExcelColumn oringial = originalTab.Column(i);

                merged.Width = oringial.Width;
                merged.BestFit = oringial.BestFit;
                merged.ColumnMax = oringial.ColumnMax;
                merged.Hidden = oringial.Hidden;
            }

            // copy row height
            for (int i = 1; i < totalRows + 1; i++)
            {
                ExcelRow merged = mergedSheet.Row(i);
                ExcelRow oringial = originalTab.Row(i);

                merged.CustomHeight = oringial.CustomHeight;
                merged.Height = oringial.Height;
                merged.StyleID = oringial.StyleID;
            }


            // finally we take all the of the content of the stylesheet (except for the header)
            // and we sort of the lines by key (the first column)
            mergedSheet.Cells[2, 1, totalRows, totalColumns].Sort(1, true);

            // finally , at this point the styesheet should be correctly setup
            // so we read out the content as bytes to return
            // note : the stylesheet created here  is temporary and used only to manipulate the data
            // so all we need is the result as bytes , then we can dispose of the stylesheet
            editableSheet.Save();

            return editableSheet;

        }

        IBExcelFilterUI IBExcelFilter.CreatetUI()
        {
            return new BExcelMergeUI();
        }

        void IBExcelFilter.Clean()
        {
            SourceAsStream.Dispose();
            OnFilterChanged = null;
            SourceAsStream = null;
        }
    }
}