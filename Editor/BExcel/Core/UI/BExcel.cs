using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using OfficeOpenXml;
using System.IO;
using Bloodthirst.Core.Utils;
using Bloodthirst.Editor.BSearch;
using System.Collections.Generic;
using System;

namespace Bloodthirst.Editor.BExcel
{
    public class BExcel : EditorWindow
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BExcel/Core/UI/BExcel.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BExcel/Core/UI/BExcel.uss";

        [MenuItem("Bloodthirst Tools/BExcel")]
        public static void ShowExample()
        {
            BExcel wnd = GetWindow<BExcel>();
            wnd.titleContent = new GUIContent(nameof(BExcel));
        }

        private VisualElement Root { get; set; }

        #region ui
        private ObjectField ExcelAsset => Root.Q<ObjectField>(nameof(ExcelAsset));
        private VisualElement FilterDropdownContainer => Root.Q<VisualElement>(nameof(FilterDropdownContainer));
        private PopupField<IBExcelFilter> FilterDropdown { get; set; }
        private VisualElement FilterUIContainer => Root.Q<VisualElement>(nameof(FilterUIContainer));
        private Button ExecuteBtn => Root.Q<Button>(nameof(ExecuteBtn));
        private HelpBox ErrorDisplay => Root.Q<HelpBox>(nameof(ErrorDisplay));
        #endregion

        #region states
        private IBExcelFilterUI filterUi;
        private IBExcelFilter filter;
        private IBExcelFilter Filter
        {
            get => filter;
            set
            {
                if (filter == value)
                    return;

                // clean
                if (filter != null)
                {
                    filterUi.Unbind();
                    filter.OnFilterChanged -= HandleFilterChanged;
                    filter.Clean();

                    FilterUIContainer.Clear();
                }

                filter = value;
                FilterUIContainer.Display(false);

                // setup
                if (filter != null)
                {
                    // filter
                    filter.Setup(CurrentExcelFile);

                    filter.OnFilterChanged -= HandleFilterChanged;
                    filter.OnFilterChanged += HandleFilterChanged;

                    // ui
                    filterUi = filter.CreatetUI();
                    VisualElement filterUiElement = filterUi.Bind(filter , CurrentExcelFile);

                    FilterUIContainer.Display(true);
                    FilterUIContainer.Add(filterUiElement);
                }

                Refresh();
            }
        }
        private ExcelPackage currentExcelFile;
        private ExcelPackage CurrentExcelFile
        {
            get => currentExcelFile;
            set
            {
                if (currentExcelFile == value)
                    return;

                if (currentExcelFile != null)
                    currentExcelFile.Dispose();

                currentExcelFile = value;
                Refresh();
            }
        }
        #endregion

        public void CreateGUI()
        {
            Root = rootVisualElement;

            // UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            visualTree.CloneTree(Root);

            // USS
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);
            Root.styleSheets.Add(styleSheet);

            InitializeUI();
            InitializeState();
            Refresh();
        }
        private bool IsExcelFileValid()
        {
            return CurrentExcelFile != null;
        }

        private bool IsFilterValid(out string err)
        {
            err = string.Empty;

            if (!Filter.IsValid(out err))  
                return false;

            return true;
        }

        private void Refresh()
        {
            FilterDropdownContainer.Display(false);
            ErrorDisplay.Display(false);
            ExecuteBtn.SetEnabled(false);

            if (!IsExcelFileValid())
            {
                Filter = null;
                return;
            }

            FilterDropdownContainer.Display(true);

            if (Filter == null)
                return;

            if (!IsFilterValid(out string err))
            {
                ErrorDisplay.Display(true);
                ErrorDisplay.messageType = HelpBoxMessageType.Info;
                ErrorDisplay.text = err;
                return;
            }

            ExecuteBtn.SetEnabled(true);
        }
        private void Execute()
        {
            Filter.Execute();
        }
        private string FormatString(IBExcelFilter searchFilter)
        {
            if (searchFilter == null)
            {
                return "NULL";
            }

            Type filterType = searchFilter.GetType();
            BExcelFilterNameAttribute attr = (BExcelFilterNameAttribute)Attribute.GetCustomAttribute(filterType, typeof(BExcelFilterNameAttribute));

            if (attr == null)
            {
                return filterType.Name;
            }

            return attr.Name;
        }

        #region init
        private void InitializeUI()
        {
            ExcelAsset.objectType = typeof(UnityEngine.Object);

            // filter dropdown
            ExcelAsset.UnregisterValueChangedCallback(HandleExcelAssetChanged);
            ExcelAsset.RegisterValueChangedCallback(HandleExcelAssetChanged);

            List<IBExcelFilter> filters = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => TypeUtils.IsSubTypeOf(t, typeof(IBExcelFilter)))
                .Select(t => (IBExcelFilter)Activator.CreateInstance(t))
                .ToList();

            filters.Insert(0, null);

            // selection
            FilterDropdown = new PopupField<IBExcelFilter>("Filter", filters, 0, FormatString, FormatString);
            FilterDropdown.UnregisterValueChangedCallback(HandleFilterDropdownChanged);
            FilterDropdown.RegisterValueChangedCallback(HandleFilterDropdownChanged);

            FilterDropdownContainer.Add(FilterDropdown);


            ExecuteBtn.clickable.clicked -= HandleExecuteClicked;
            ExecuteBtn.clickable.clicked += HandleExecuteClicked;

            FilterUIContainer.Display(false);
            ErrorDisplay.Display(false);
        }
        private void InitializeState()
        {
            CurrentExcelFile = null;
            ExecuteBtn.SetEnabled(false);
        }
        #endregion

        #region event listening
        private void HandleFilterDropdownChanged(ChangeEvent<IBExcelFilter> evt)
        {
            Filter = evt.newValue;
        }
        private void HandleFilterChanged(IBExcelFilter filter)
        {
            Refresh();
        }
        private void HandleExecuteClicked()
        {
            Execute();
        }
        private void HandleExcelAssetChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            if(evt.newValue == null)
            {
                CurrentExcelFile = null;
                return;
            }

            UnityEngine.Object excelFile = evt.newValue;

            string fullPath = EditorUtils.RelativeToAbsolutePath(AssetDatabase.GetAssetPath(excelFile));
            byte[] textAsBytes = File.ReadAllBytes(fullPath);

            MemoryStream memoryStream = new MemoryStream(textAsBytes);

            try
            {
                CurrentExcelFile = new ExcelPackage(memoryStream);
            }
            catch
            {
                CurrentExcelFile = null;
            }
        }
        #endregion

        #region close
        private void Clean()
        {
            if (CurrentExcelFile != null)
            {
                CurrentExcelFile = null;
            }

            if(filter != null)
            {
                filter.OnFilterChanged -= HandleFilterChanged;
                filterUi.Unbind();
                filterUi = null;
                filter = null;
            }

            ExcelAsset.UnregisterValueChangedCallback(HandleExcelAssetChanged);
            ExecuteBtn.clickable.clicked -= HandleExecuteClicked;
        }
        private void OnDestroy()
        {
            Clean();
        }
        #endregion
    }
}