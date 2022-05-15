using Bloodthirst.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.GameEventSystem
{
    public class DefaultPopupUI : VisualElement
    {
        private const string UXML_PATH =    "Packages/com.bloodthirst.bloodthirst-core/Editor/GameEventSystem/Editor/Dropdown/Default/DefaultPopupUI.uxml";
        private const string USS_PATH =     "Packages/com.bloodthirst.bloodthirst-core/Editor/GameEventSystem/Editor/Dropdown/Default/DefaultPopupUI.uss";

        private Label IndexLabel => this.Q<Label>(nameof(IndexLabel));
        private Label ValueLabel => this.Q<Label>(nameof(ValueLabel));

        public DefaultPopupUI()
        {
            // import USS
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            visualTree.CloneTree(this);

            styleSheets.Add(styleSheet);
            styleSheets.Add(EditorConsts.GlobalStyleSheet);
        }

        public void Setup(IndexWrapper indexWrapper)
        {
            IndexLabel.text = indexWrapper.Index.ToString();
            ValueLabel.text = indexWrapper.Value.ToString();
        }
    }
}
