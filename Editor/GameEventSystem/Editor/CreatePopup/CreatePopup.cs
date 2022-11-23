using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Editor.Commands;
using Bloodthirst.Editor.CustomComponent;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.GameEventSystem
{
    public class CreatePopup : EditorWindow
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/GameEventSystem/Editor/CreatePopup/CreatePopup.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/GameEventSystem/Editor/CreatePopup/CreatePopup.uss";

        private TextField NamespaceTxt => root.Q<TextField>(nameof(NamespaceTxt));
        private TextField EnumName => root.Q<TextField>(nameof(EnumName));

        private TextField GameEventPathTxt => root.Q<TextField>(nameof(GameEventPathTxt));
        private Button GameEventPathBtn => root.Q<Button>(nameof(GameEventPathBtn));
        internal GameEventSystemEditor FromWindow { get; set; }

        private Button CreateBtn => root.Q<Button>(nameof(CreateBtn));

        private VisualElement root;

        private void CreateGUI()
        {
            root = rootVisualElement;

            // import USS
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            visualTree.CloneTree(root);

            root.styleSheets.Add(styleSheet);
            if (!root.styleSheets.Contains(EditorConsts.GlobalStyleSheet))
            {
                root.styleSheets.Add(EditorConsts.GlobalStyleSheet);
            }

            InitializeUI();

            ListenUI();
        }

        private void ListenUI()
        {
            NamespaceTxt.RegisterValueChangedCallback(HandleDataChanged);
            EnumName.RegisterValueChangedCallback(HandleDataChanged);

            GameEventPathTxt.RegisterValueChangedCallback(HandleDataChanged);
            GameEventPathBtn.clickable.clicked += HandleScriptPathClicked;

            CreateBtn.clickable.clicked += HandleCreateClicked;
        }

        private void HandleScriptPathClicked()
        {
            string filePath = EditorUtility.SaveFolderPanel("Select Enum File Path", "Assets", EnumName.value);
            GameEventPathTxt.value = filePath;
        }

        private void HandleCreateClicked()
        {
            GameEventSystemAsset asset = CreateInstance<GameEventSystemAsset>();
            asset.enumName = EnumName.value;
            asset.namespaceValue = NamespaceTxt.value;

            AssetDatabase.CreateAsset(asset, EditorUtils.AbsoluteToRelativePath(GameEventPathTxt.value + "/" + EnumName.value + ".asset"));

            CreateGameEventSystemCoreFiles cmd = new CreateGameEventSystemCoreFiles(asset, GameEventPathTxt.value);

            CommandManagerEditor.RunInstant(cmd);

            Close();

            EditorUtility.DisplayDialog("Success", "GameEventSystem created successfully!", "Ok");

            FromWindow.UpdateDropdownOptions();

            IndexWrapper indexOf = FromWindow.DropdownUI
                .AllValues
                .FirstOrDefault(v => (object)v.Value == asset);

            FromWindow.DropdownUI.SetCurrentValueWithoutNotify(indexOf.Index);
            FromWindow.GameEventAsset = asset;

        }

        private void HandleDataChanged(ChangeEvent<string> evt)
        {
            Refresh();
        }

        private bool CanCreate()
        {
            if (string.IsNullOrWhiteSpace(NamespaceTxt.value))
                return false;

            if (string.IsNullOrWhiteSpace(EnumName.value))
                return false;

            return true;
        }

        private void Refresh()
        {
            bool val = CanCreate();

            CreateBtn.SetEnabled(val);
        }

        private void InitializeUI()
        {
            GameEventPathTxt.SetEnabled(false);
            CreateBtn.SetEnabled(false);
        }


    }
}
