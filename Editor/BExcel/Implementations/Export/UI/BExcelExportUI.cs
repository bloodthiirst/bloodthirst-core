using Bloodthirst.Core.Utils;
using System;
using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BExcelEditor
{
    public class BExcelExportUI : IBExcelFilterUI
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BExcel/Implementations/Export/UI/BExcelExportUI.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BExcel/Implementations/Export/UI/BExcelExportUI.uss";

        private BExcel Editor { get; set; }
        private BExcelExport ExcelExport { get; set; }

        private VisualElement Root { get; set; }
        private TextField AssetExportPath => Root.Q<TextField>(nameof(AssetExportPath));
        private Button ChangeAssetExportBtn => Root.Q<Button>(nameof(ChangeAssetExportBtn));
        private TextField ScriptExportPath => Root.Q<TextField>(nameof(ScriptExportPath));
        private Button ChangeScriptExportBtn => Root.Q<Button>(nameof(ChangeScriptExportBtn));
        private TextField Namespace => Root.Q<TextField>(nameof(Namespace));

        VisualElement IBExcelFilterUI.Bind(IBExcelFilter filter, BExcel editor)
        {
            Editor = editor;
            ExcelExport = (BExcelExport)filter;
            Root = new VisualElement();

            // UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);
            visualTree.CloneTree(Root);

            // USS
            Root.styleSheets.Add(styleSheet);

            // asset export
            ChangeAssetExportBtn.clickable.clicked += HandleChangeAssetExportClicked;
            AssetExportPath.SetEnabled(false);
            AssetExportPath.RegisterValueChangedCallback(HandleExportPathChanged);

            // script export
            ChangeScriptExportBtn.clickable.clicked += HandleChangeScriptExportClicked;
            ScriptExportPath.SetEnabled(false);
            ScriptExportPath.RegisterValueChangedCallback(HandleExportPathChanged);

            Namespace.RegisterValueChangedCallback(HandleNamespaceChanged);

            Refresh();

            return Root;
        }

        private void HandleNamespaceChanged(ChangeEvent<string> evt)
        {
            Refresh();
        }

        private void HandleExportPathChanged(ChangeEvent<string> evt)
        {
            Refresh();
        }

        private void HandleChangeScriptExportClicked()
        {
            string relativePath = AssetDatabase.GetAssetPath(Editor.CurrentAsset);
            string defaultPath = EditorUtils.RelativeToAbsolutePath(relativePath);

            string filename = Path.GetFileNameWithoutExtension(defaultPath);
            string folder = Path.GetDirectoryName(relativePath);

            string path = EditorUtility.SaveFilePanelInProject("Select export path", $"{filename}", "cs", String.Empty, folder);

            BExcelOutput existing = AssetDatabase.LoadAssetAtPath<BExcelOutput>(AssetExportPath.value);

            if (existing != null)
            {
                existing.scriptPath = path;
            }

            ScriptExportPath.value = path;
        }
        private void HandleChangeAssetExportClicked()
        {
            string relativePath = AssetDatabase.GetAssetPath(Editor.CurrentAsset);
            string defaultPath = EditorUtils.RelativeToAbsolutePath(relativePath);

            string filename = Path.GetFileNameWithoutExtension(defaultPath);
            string folder = Path.GetDirectoryName(relativePath);

            string path = EditorUtility.SaveFilePanelInProject("Select export path", $"{filename}", "asset", String.Empty, folder);

            BExcelOutput existing = AssetDatabase.LoadAssetAtPath<BExcelOutput>(path);

            if (existing != null)
            {
                ScriptExportPath.SetValueWithoutNotify(existing.scriptPath);
                Namespace.value = existing.scriptNamespace;
            }

            AssetExportPath.value = path;
        }

        private void Refresh()
        {
            ExcelExport.AssetExportPath = AssetExportPath.value;
            ExcelExport.Namespace = Namespace.value;
            ExcelExport.ScriptExportPath = ScriptExportPath.value;
            ExcelExport.NotifyFilterChanged();
        }

        void IBExcelFilterUI.Unbind()
        {
            ChangeAssetExportBtn.clickable.clicked -= HandleChangeAssetExportClicked;
            ChangeScriptExportBtn.clickable.clicked -= HandleChangeScriptExportClicked;

            AssetExportPath.UnregisterValueChangedCallback(HandleExportPathChanged);
            ScriptExportPath.UnregisterValueChangedCallback(HandleExportPathChanged);

            Namespace.UnregisterValueChangedCallback(HandleNamespaceChanged);
        }
    }
}