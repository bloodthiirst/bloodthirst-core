﻿using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Editor.CustomComponent;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.GameEventSystem
{
    public class SearchableDropdownAssetElement : VisualElement
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/CustomUIToolkit.Editor/SearchableDropdown/Asset/SearchableDropdownAssetElement.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/CustomUIToolkit.Editor/SearchableDropdown/Asset/SearchableDropdownAssetElement.uss";

        private Image Icon => this.Q<Image>(nameof(Icon));
        private Label Info => this.Q<Label>(nameof(Info));
        private Label Path => this.Q<Label>(nameof(Path));

        public SearchableDropdownAssetElement()
        {
            // import USS
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            visualTree.CloneTree(this);

            styleSheets.Add(styleSheet);
            if (!styleSheets.Contains(EditorConsts.GlobalStyleSheet))
            {
                styleSheets.Add(EditorConsts.GlobalStyleSheet);
            }
        }

        public void Setup(IndexWrapper indexWrapper)
        {
            Object casted = (Object)indexWrapper.Value;

            Texture tex = EditorUtils.GetIcon(casted);
            Info.text = casted.name;
            Path.text = AssetDatabase.GetAssetPath(casted);
            Icon.image = tex;
        }
    }
}
