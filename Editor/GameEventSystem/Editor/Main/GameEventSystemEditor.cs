using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Bloodthirst.Editor;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Bloodthirst.Core.GameEventSystem
{

    public class GameEventSystemEditor : EditorWindow
    {
        private const string UXML_PATH =    "Packages/com.bloodthirst.bloodthirst-core/Editor/GameEventSystem/Editor/Main/GameEventSystemEditor.uxml";
        private const string USS_PATH =     "Packages/com.bloodthirst.bloodthirst-core/Editor/GameEventSystem/Editor/Main/GameEventSystemEditor.uss";

        [MenuItem("Bloodthirst Tools/GameEventSystem")]
        public static void ShowExample()
        {
            GameEventSystemEditor wnd = GetWindow<GameEventSystemEditor>();
            wnd.titleContent = new GUIContent("GameEventSystemEditor");
        }

        private VisualElement root;
        private VisualElement DropdownContainer => root.Q<VisualElement>(nameof(DropdownContainer));

        private PopupUI DropdownUI { get; set; }

        private TwoPaneSplitView PanelSplit => root.Q<TwoPaneSplitView>(nameof(PanelSplit));

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            root = rootVisualElement;

            // import USS
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            visualTree.CloneTree(root);

            root.styleSheets.Add(styleSheet);
            root.styleSheets.Add(EditorConsts.GlobalStyleSheet);

            InitializeUI();

            ListenEvents();
        }

        private enum TestEnum
        {
            First,
            Second,
            Third
        }

        private VisualElement MakeItem()
        {
            return new AssetPopupUI();
        }

        private void BindItem(VisualElement element , int index)
        {
            AssetPopupUI casted = (AssetPopupUI)element;

            IndexWrapper wrapped = DropdownUI.CurrentDropdown.FilteredValues[index];

            casted.Setup(wrapped);
        }

        private string SearchTerm(object item)
        {
            UnityEngine.Object casted = (UnityEngine.Object)((IndexWrapper)item).Value;

            return casted.name.ToLowerInvariant();
        }

        private string SelectedAsString(object item)
        {
            UnityEngine.Object casted = (UnityEngine.Object)((IndexWrapper)item).Value;

            return casted.name;
        }

        private void InitializeUI()
        {
            List<UnityEngine.Object> unityObjects = AssetDatabase.FindAssets("t:Object")
                .Select( g => AssetDatabase.GUIDToAssetPath(g))
                .Select( p => AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(p))
                .ToList();

            DropdownUI = new PopupUI("GameEvents Datasource" , 0 , unityObjects, this , MakeItem , BindItem , SearchTerm , SelectedAsString);
            DropdownContainer.Add(DropdownUI);

            PanelSplit.fixedPaneIndex = 1;
            PanelSplit.fixedPaneInitialDimension = 250;

        }

        private void ListenEvents()
        {

        }

        private void OnDestroy()
        {

        }
    }
}