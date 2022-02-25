using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BSearch
{
    public class BSearchResult : VisualElement
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BSearch/UI/Result/BSearchResult.uxml";

        public VisualElement PathsContainer => this.Q<VisualElement>(nameof(PathsContainer));

        public BSearchResult()
        {
            AddToClassList("shrink-0");
            AddToClassList("grow-1");
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH).CloneTree(this);
        }
    }
}