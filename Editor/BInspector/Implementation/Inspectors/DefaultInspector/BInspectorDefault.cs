using Bloodthirst.Editor.BInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
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
            root.styleSheets.Add(EditorConsts.GlobalStyleSheet);

            container = root.Q<VisualElement>(nameof(container));

            // script object picker
            if (instance is MonoBehaviour mb)
            {
                MonoScript monoScriptRef = MonoScript.FromMonoBehaviour(mb);
                VisualElement gui = scriptObjectField.CreateVisualElement(monoScriptRef);

                container.Add(gui);
            }


            // event
            container.RegisterCallback<GeometryChangedEvent>(HandleGeometryChanged);

            // context
            DrawerContext drawerContext = new DrawerContext();
            drawerContext.AllDrawers = new List<IValueDrawer>();
            drawerContext.IndentationLevel = 0;

            // style
            LabelDrawer labelDrawer = new LabelDrawer();

            // fields
            List<MemberData> validMembers = typeData.MemberDatas.Where( m => !m.MemberInfo.Name.EndsWith("k__BackingField")).ToList();

            foreach (MemberData m in validMembers)
            {
                ValueDrawerInfoBasic info = new ValueDrawerInfoBasic() { ContainingInstance = instance, MemberData = m };

                IValueDrawer fieldDrawer = ValueDrawerProvider.Get(info.DrawerType());

                fieldDrawer.Setup(info , null , drawerContext);

                // add to editor layout
                container.Add(fieldDrawer.DrawerRoot);

                // add label
                labelDrawer.Setup(fieldDrawer);
            }



            return root;
        }

        private void HandleGeometryChanged(GeometryChangedEvent evt)
        {
            // label spacing

            LabelSpacing labelSpacing = new LabelSpacing();
            labelSpacing.Setup(container);
        }
    }
}