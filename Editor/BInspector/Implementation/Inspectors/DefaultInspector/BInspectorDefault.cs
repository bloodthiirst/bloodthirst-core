using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class BInspectorDefault : IBInspectorDrawer, IBInspectorValidator
    {
        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Implementation/Inspectors/DefaultInspector/BInspectorDefault.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Implementation/Inspectors/DefaultInspector/BInspectorDefault.uss";
        int IBInspectorValidator.Order => 0;

        private ScriptObjectFieldDrawer scriptObjectField;
        private VisualElement container;

        void IBInspectorValidator.Initialize()
        {

        }

        bool IBInspectorValidator.CanInspect(Type type, object instance)
        {
            return true;
        }
        IBInspectorDrawer IBInspectorValidator.GetDrawer()
        {
            return this;
        }

        void IBInspectorDrawer.Initialize()
        {
            scriptObjectField = new ScriptObjectFieldDrawer();
        }

        RootEditor IBInspectorDrawer.CreateInspectorGUI(object instance)
        {
            return GetEditor(instance);
        }

        public struct RootEditor
        {
            public VisualElement RootContainer { get; set; }
            public IValueDrawer RootDrawer { get; set; }
        }

        private RootEditor GetEditor(object instance)
        {

            // start creating the ui
            VisualTreeAsset prefab = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            TemplateContainer root = prefab.CloneTree();
            root.styleSheets.Add(style);
            root.styleSheets.Add(EditorConsts.GlobalStyleSheet);

            container = root.Q<VisualElement>(nameof(container));

            // script object picker
            if (instance is MonoBehaviour mb)
            {
                MonoScript monoScriptRef = MonoScript.FromMonoBehaviour(mb);
                VisualElement gui = scriptObjectField.CreateVisualElement(monoScriptRef);

                container.Add(gui);
            }

            ComplexValueDrawer drawer = new ComplexValueDrawer();

            
            ValueProviderGeneric info = new ValueProviderGeneric(
                
                // state
                null, 
                
                // vale path
                new ValuePath() { PathType = PathType.ROOT },
                
                // getter
                () => instance,
               
                // setter
                null,

                // type
                instance.GetType(), 
                
                // parent instance
                null , 
                
                // member info
                null);

            ValueDrawerUtils.DoLayoutRoot(drawer ,info);
                        
            container.Add(drawer.DrawerContainer);

            return new RootEditor()
            {
                RootContainer = root,
                RootDrawer = drawer
            };
        }
    }
}