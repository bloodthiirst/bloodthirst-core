using Bloodthirst.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.CustomComponent
{
    public class TabButton : VisualElement
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/CustomComponent/TabUI/TabButton/TabButton.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/CustomComponent/TabUI/TabButton/TabButton.uss";

        private Label TabTitle => this.Q<Label>(nameof(TabTitle));
        private VisualElement TabContainer => this.Q<VisualElement>(nameof(TabContainer));

        public TabUI Parent { get; }
        public string Title { get; set; }
        public int TabIndex { get; set; }

        public TabButton(TabUI parent, string title, int tabIndex)
        {
            Parent = parent;
            Title = title;
            TabIndex = tabIndex;
            SetupElement();
            
        }

        public void Select()
        {
            AddToClassList("selected");
            RemoveFromClassList("unselected");
        }

        public void UnSelected()
        {
            AddToClassList("unselected");
            RemoveFromClassList("selected");
        }

        private void SetupElement()
        {
            // import USS
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            visualTree.CloneTree(this);

            styleSheets.Add(styleSheet);
            styleSheets.Add(EditorConsts.GlobalStyleSheet);

            TabTitle.text = Title;

            TabContainer.RegisterCallback<ClickEvent>(HandleClick);
        }

        private void HandleClick(ClickEvent evt)
        {
            Parent.Select(TabIndex);
        }
    }
}
