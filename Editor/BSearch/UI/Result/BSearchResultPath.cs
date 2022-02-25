using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BSearch
{
    public class BSearchResultPath : VisualElement
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BSearch/UI/Result/BSearchResultPath.uxml";
        public Label  FieldPath => this.Q<Label>(nameof(FieldPath));
        public Label FieldType => this.Q<Label>(nameof(FieldType));
        public ObjectField FieldValue => this.Q<ObjectField>(nameof(FieldValue));

        public BSearchResultPath()
        {
            AddToClassList("shrink-0");
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH).CloneTree(this);
        }
    }
}