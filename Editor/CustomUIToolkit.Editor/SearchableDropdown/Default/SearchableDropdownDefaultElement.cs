using Bloodthirst.Editor;
using Bloodthirst.Editor.CustomComponent;
using UnityEditor;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.GameEventSystem
{
    public class SearchableDropdownDefaultElement : VisualElement
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/CustomUIToolkit.Editor/SearchableDropdown/Default/SearchableDropdownDefaultElement.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/CustomUIToolkit.Editor/SearchableDropdown/Default/SearchableDropdownDefaultElement.uss";

        private Label IndexLabel => this.Q<Label>(nameof(IndexLabel));
        private Label ValueLabel => this.Q<Label>(nameof(ValueLabel));

        public SearchableDropdownDefaultElement()
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
            IndexLabel.text = indexWrapper.Index.ToString();
            ValueLabel.text = indexWrapper.Value.ToString();
        }
    }
}
