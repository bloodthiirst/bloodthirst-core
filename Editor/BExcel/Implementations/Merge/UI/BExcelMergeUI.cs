using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BSearch
{
    public class BExcelMergeUI : IBExcelFilterUI
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BExcel/Implementations/Merge/UI/BExcelMergeUI.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BExcel/Implementations/Merge/UI/BExcelMergeUI.uss";

        private ExcelPackage ExcelFile { get; set; }
        private BExcelMerge ExcelMerge { get; set; }

        private VisualElement Root { get; set; }
        private TextField ExportPath => Root.Q<TextField>(nameof(ExportPath));
        private Button ChangeExportBtn => Root.Q<Button>(nameof(ChangeExportBtn));
        private VisualElement OriginalTabContainer => Root.Q<VisualElement>(nameof(OriginalTabContainer));
        private SourceTabDropdownUI OriginalTab {get; set; }
        private Button AddTabBtn => Root.Q<Button>(nameof(AddTabBtn));
        private VisualElement TabsContainer => Root.Q<VisualElement>(nameof(TabsContainer));
        private List<TabDropdownUI> TabsList { get; set; } = new List<TabDropdownUI>();

        private List<string> TabNames { get; set; }

        VisualElement IBExcelFilterUI.Bind(IBExcelFilter filter , ExcelPackage excelFile)
        {
            ExcelFile = excelFile;
            ExcelMerge = (BExcelMerge)filter;
            TabNames = ExcelFile.Workbook.Worksheets.Select( w => w.Name).ToList();
            Root = new VisualElement();

            // UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);
            visualTree.CloneTree(Root);

            // USS
            Root.styleSheets.Add(styleSheet);

            // export
            ChangeExportBtn.clickable.clicked += HandleChangeExportClicked;
            ExportPath.SetEnabled(false);
            ExportPath.RegisterValueChangedCallback(HandleExportPathChanged);

            // original
            OriginalTab = new SourceTabDropdownUI(TabNames);
            OriginalTab.OnValueChanged += HandleTabChanged;

            OriginalTabContainer.Add(OriginalTab);

            // add btn
            AddTabBtn.clickable.clicked += HandleAddClicked;

            Refresh();

            return Root;
        }

        private void HandleExportPathChanged(ChangeEvent<string> evt)
        {
            Refresh();
        }

        private void HandleChangeExportClicked()
        {
            string path = EditorUtility.SaveFilePanelInProject("Select export path", "Merge_Result", "xlsx" , String.Empty);

            ExportPath.value = path;
        }

        private void Refresh()
        {
            ExcelMerge.ExportPath = ExportPath.value;
            ExcelMerge.OriginalTab = OriginalTab.Value;

            ExcelMerge.DuplicateTabs.Clear();
            foreach (TabDropdownUI tabUi in TabsList)
            {
                ExcelMerge.DuplicateTabs.Add(tabUi.Value);
            }

            ExcelMerge.NotifyFilterChanged();
        }

        private void HandleAddClicked()
        {
            TabDropdownUI tabUi = new TabDropdownUI(TabNames);

            tabUi.OnValueChanged += HandleTabChanged;
            tabUi.OnRemoveTriggered += HandleTabRemove;

            TabsContainer.Add(tabUi);
            TabsList.Add(tabUi);

            Refresh();
        }

        private void HandleTabRemove(TabDropdownUI tabUi)
        {
            tabUi.OnRemoveTriggered -= HandleTabRemove;
            tabUi.OnValueChanged -= HandleTabChanged;

            TabsList.Remove(tabUi);
            TabsContainer.Remove(tabUi);

            Refresh();
        }

        private void HandleTabChanged(string tabName)
        {
            Refresh();
        }

        void IBExcelFilterUI.Unbind()
        {
            ChangeExportBtn.clickable.clicked -= HandleChangeExportClicked;
            ExportPath.UnregisterValueChangedCallback(HandleExportPathChanged);

            OriginalTab.OnValueChanged -= HandleTabChanged;

            AddTabBtn.clickable.clicked -= HandleAddClicked;

            foreach (TabDropdownUI tabUi in TabsList)
            {
                tabUi.OnRemoveTriggered -= HandleTabRemove;
                tabUi.OnValueChanged -= HandleTabChanged;
            }
        }
    }
}