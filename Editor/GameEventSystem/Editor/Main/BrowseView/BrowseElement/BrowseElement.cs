using Bloodthirst.Core.Utils;
using Bloodthirst.Editor;
using Bloodthirst.Editor.CustomComponent;
using System;
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
            if (!styleSheets.Contains(EditorConsts.GlobalStyleSheet))
            {
                styleSheets.Add(EditorConsts.GlobalStyleSheet);
            }

            EnumScript.objectType = typeof(TextAsset);
            ClassScript.objectType = typeof(TextAsset);

            EnumValue.SetEnabled(false);
            EnumScript.SetEnabled(false);
            ClassScript.SetEnabled(false);
        }

        internal class AssemblyDefinitionJsonContent
        {
            public string name;
            public string rootNamespace;
            public string[] references;
            public string[] includePlatforms;
            public string[] excludePlatforms;
            public bool allowUnsafeCode;
            public bool overrideReferences;
            public string[] precompiledReferences;
            public bool autoReferenced;
            public string[] defineConstraints;
            public string[] versionDefines;
            public bool noEngineReferences;
        }

        public void Setup(GameEventSystemEditor editor, IndexWrapper indexWrapper)
        {
            AssemblyDefinitionJsonContent asJson = new AssemblyDefinitionJsonContent();
            EditorJsonUtility.FromJsonOverwrite(editor.GameEventAsset.assemblyDefinition.text, asJson);

            GameEventSystemAsset.EnumClassPair casted = (GameEventSystemAsset.EnumClassPair)indexWrapper.Value;

            // enum value
            EnumValue.value = casted.enumValue;

            // enum script
            Type enumAsType = Type.GetType($"{editor.GameEventAsset.namespaceValue}.{editor.GameEventAsset.enumName}, {asJson.name}");
            TextAsset enumS = AdvancedTypeCache.CacheAsset.Cache[enumAsType].unityScript;
            EnumScript.value = enumS;

            // event script
            Type classAsType = Type.GetType($"{editor.GameEventAsset.namespaceValue}.{casted.className}, {asJson.name}");
            TextAsset classS = AdvancedTypeCache.CacheAsset.Cache[classAsType].unityScript;
            ClassScript.value = classS;

        }
    }
}
