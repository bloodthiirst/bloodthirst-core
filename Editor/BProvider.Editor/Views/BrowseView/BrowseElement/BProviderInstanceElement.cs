using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Editor.BInspector;
using Bloodthirst.Editor.CustomComponent;
using Sirenix.Utilities;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Core.BProvider.Editor
{
    public class BProviderInstanceElement : VisualElement
    {
        private const string UXML_PATH = BProviderEditor.FOLDER_PATH + "Views/BrowseView/BrowseElement/BProviderInstanceElement.uxml";
        private const string USS_PATH = BProviderEditor.FOLDER_PATH + "Views/BrowseView/BrowseElement/BProviderInstanceElement.uss";
        private TextField TypeName => this.Q<TextField>(nameof(TypeName));
        private ObjectField TypeScript => this.Q<ObjectField>(nameof(TypeScript));
        private VisualElement Value => this.Q<VisualElement>(nameof(Value));
        private Foldout ValueFoldout => this.Q<Foldout>(nameof(ValueFoldout));
        private BInspectorDefault.RootEditor Editor { get; set; }

        public BProviderInstanceElement()
        {
            // import USS
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            // Import UXML
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            visualTree.CloneTree(this);

            AddToClassList("item-root");


            styleSheets.Add(styleSheet);
            if (!styleSheets.Contains(EditorConsts.GlobalStyleSheet))
            {
                styleSheets.Add(EditorConsts.GlobalStyleSheet);
            }

            TypeScript.objectType = typeof(TextAsset);


            TypeName.SetEnabled(false);
            TypeScript.SetEnabled(false);
        }

        public void Setup(BProviderEditor editor, IndexWrapper indexWrapper)
        {
            // enum script
            Type objectType = indexWrapper.Value.GetType();

            TextAsset typeS = null;

            // if the type is none locatable (like system.Object for examaple) then wejust leave as null
            if (AdvancedTypeCache.CacheAsset.Cache.TryGetValue(objectType, out AdvancedTypeCache.TypeInformation typeInfo))
            {
                typeS = typeInfo.unityScript;
            }

            TypeScript.value = typeS;

            TypeName.value = objectType.GetNiceName();

            IBInspectorDrawer drawer = BInspectorProvider.DefaultInspector;

            Editor = drawer.CreateInspectorGUI(indexWrapper.Value);

            Value.Add(Editor.RootContainer);
        }

        public void Tick()
        {
            Editor.RootDrawer.Tick();
        }
    }
}
