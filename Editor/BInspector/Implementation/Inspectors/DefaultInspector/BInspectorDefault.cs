using System;
using System.Collections.Generic;
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

        VisualElement IBInspectorDrawer.CreateInspectorGUI(object instance)
        {
            return GetEditor(instance);
        }

        private VisualElement GetEditor(object instance)
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

            // context
            DrawerContext drawerContext = new DrawerContext();
            drawerContext.AllDrawers = new List<IValueDrawer>();
            drawerContext.IndentationLevel = 0;

            ValueDrawerInfoGeneric info = new ValueDrawerInfoGeneric(null, () => instance, null, instance.GetType() , null);

            drawer.Setup(info, null, drawerContext);

            container.Add(drawer.DrawerRoot);

            return root;
        }
    }
}