using Bloodthirst.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.CustomComponent
{
    public class TabContent : VisualElement
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/CustomComponent/TabUI/TabContent/TabContent.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/CustomComponent/TabUI/TabContent/TabContent.uss";

        private Label TabTitle => this.Q<Label>(nameof(TabTitle));

        public TabUI Parent { get; }
        public string Title { get; set; }
        public int TabIndex { get; set; }

        public TabContent(TabUI parent, int tabIndex)
        {
            Parent = parent;
            TabIndex = tabIndex;
        }
    }
}
