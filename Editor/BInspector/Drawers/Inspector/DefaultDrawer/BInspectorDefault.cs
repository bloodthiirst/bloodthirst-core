using Bloodthirst.Editor.BInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class BInspectorDefault : IBInspectorDrawer, IBInspectorValidator
    {
        public const string DropdownContainer = nameof(DropdownContainer);
        public const string MainContentContainer = nameof(MainContentContainer);

        private const string UXML_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Drawers/Inspector/DefaultDrawer/BInspectorDefault.uxml";
        private const string USS_PATH = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Drawers/Inspector/DefaultDrawer/BInspectorDefault.uss";
        int IBInspectorValidator.Order => 0;

        private ScriptObjectFieldDrawer scriptObjectField;

        void IBInspectorValidator.Initialize()
        {
            scriptObjectField = new ScriptObjectFieldDrawer();
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

        }

        VisualElement IBInspectorDrawer.CreateInspectorGUI(object instance)
        {
            // get the type data of the inspector component
            Type type = instance.GetType();
            TypeData typeData = TypeDataProvider.Get(type);

            // start creating the ui
            VisualTreeAsset prefab = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
            StyleSheet style = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);

            TemplateContainer root = prefab.CloneTree();
            root.styleSheets.Add(style);

            VisualElement mainContent = root.Q<VisualElement>(MainContentContainer);

            // script object picker
            MonoScript monoScriptRef = MonoScript.FromMonoBehaviour((MonoBehaviour)instance);
            VisualElement gui = scriptObjectField.CreateVisualElement(monoScriptRef);

            mainContent.Add(gui);

            // fields
            foreach (MemberData m in typeData.MemberDatas)
            {
                DrawerInfo info = new DrawerInfo() { ContainerInstance = instance, MemberData = m };

                IValueDrawer fieldDrawer = ValueDrawerProvider.Get(info.DrawerType());

                if (fieldDrawer == null)
                    continue;

                fieldDrawer.Initialize();
                VisualElement elem = fieldDrawer.VisualElement;

                mainContent.Add(elem);
                fieldDrawer.Setup(info);
            }



            return root;
        }
    }
}