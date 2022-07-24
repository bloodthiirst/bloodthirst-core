using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Editor.CustomComponent;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.GameEventSystem
{
    public class BrowseElement : VisualElement
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/GameEventSystem/Editor/Main/BrowseView/BrowseElement/BrowseElement.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/GameEventSystem/Editor/Main/BrowseView/BrowseElement/BrowseElement.uss";
        private TextField EnumValue => this.Q<TextField>(nameof(EnumValue));
        private ObjectField EnumScript => this.Q<ObjectField>(nameof(EnumScript));
        private ObjectField ClassScript => this.Q<ObjectField>(nameof(ClassScript));


        public BrowseElement()
        {
            // import USS
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            visualTree.CloneTree(this);

            styleSheets.Add(styleSheet);
            styleSheets.Add(EditorConsts.GlobalStyleSheet);

            EnumScript.objectType = typeof(TextAsset);
            ClassScript.objectType = typeof(TextAsset);

            EnumValue.SetEnabled(false);
            EnumScript.SetEnabled(false);
            ClassScript.SetEnabled(false);
        }

        public void Setup(GameEventSystemEditor editor , IndexWrapper indexWrapper)
        {
            GameEventSystemAsset.EnumClassPair casted = (GameEventSystemAsset.EnumClassPair)indexWrapper.Value;

            TextAsset enumS = AdvancedTypeCache.CacheAsset.Cache.FirstOrDefault(kv => kv.Key.Name == editor.GameEventAsset.enumName).Value.unityScript;

            EnumScript.value = enumS;
            EnumValue.value = casted.enumValue;

            TextAsset classS = EditorUtils.FindScriptAssets()
                .Where(s => s.GetClass() != null)
                .FirstOrDefault(s => s.GetClass().Name == casted.className);

            ClassScript.value = classS;

        }
    }
}
