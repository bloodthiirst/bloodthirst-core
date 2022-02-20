using Bloodthirst.BJson;
using Bloodthirst.BType;
using Bloodthirst.Core.Utils;
using Bloodthirst.Runtime.BInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class ComplexValueDrawer : ValueDrawerBase
    {
        private TypeSelector TypeSelector { get; set; }
        private VisualElement UIContainer { get; set; }
        private VisualElement UIHeader { get; set; }

        public override object DefaultValue()
        {
            Type fieldType = ReflectionUtils.GetMemberType(DrawerInfo.MemberInfo);
            return ReflectionUtils.GetDefaultValue(fieldType);
        }

        public ComplexValueDrawer()
        {

        }

        protected override void PrepareUI(VisualElement root)
        {
            root.AddToClassList("column");

            UIHeader = new VisualElement();
            UIHeader.AddToClassList("row");
            UIHeader.AddToClassList("grow-1");

            UIHeader.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });

            
            UIContainer = new VisualElement();
            UIContainer.AddToClassList("column");
            UIContainer.AddToClassList("grow-1");
            UIContainer.RegisterCallback<GeometryChangedEvent>(HandleeometryChanged);

            root.Add(UIHeader);
            root.Add(UIContainer);
        }

        private void HandleeometryChanged(GeometryChangedEvent evt)
        {
            // label spacing
            LabelSpacing labelSpacing = new LabelSpacing();
            labelSpacing.Setup(UIContainer);
        }

        protected override void Postsetup()
        {
            Clean();
            Draw();
        }

        private void Clean()
        {
            if (TypeSelector != null)
            {
                UIHeader.Remove(TypeSelector);
                TypeSelector.UnregisterValueChangedCallback(HandleValueTypeChanged);
                TypeSelector = null;
            }

            foreach (IValueDrawer c in ChildrenValueDrawers)
            {
                c.Destroy();
            }

            ChildrenValueDrawers.Clear();
            UIContainer.Clear();
        }

        private void Draw()
        {
            // in case root or struct
            if (DrawerInfo.MemberInfo != null)
            {
                Type type = ReflectionUtils.GetMemberType(DrawerInfo.MemberInfo);
                
                if (type.IsClass || type.IsInterface)
                {
                    InstanceCreator();
                }
            }

            if (Value != null)
            {
                HasValue();
            }

        }

        private void InstanceCreator()
        {
            Type type = ReflectionUtils.GetMemberType(DrawerInfo.MemberInfo);

            TypeSelector = new TypeSelector(type);
            TypeSelector.AddToClassList("grow-1");

            TypeSelector.SetValueWithoutNotify(Value == null ? null : Value.GetType());

            TypeSelector.UnregisterValueChangedCallback(HandleValueTypeChanged);
            TypeSelector.RegisterValueChangedCallback(HandleValueTypeChanged);

            UIHeader.Add(TypeSelector);
        }

        private void HandleValueTypeChanged(ChangeEvent<Type> evt)
        {
            Type newType = evt.newValue;

            object newInstance = null;

            if (newType != null)
            {
                newInstance = Activator.CreateInstance(newType);
            }

            DrawerInfo.Set(newInstance);

            TriggerOnValueChangedEvent();

            Clean();
            Draw();
        }

        private void HasValue()
        {
            // get the type data of the inspector component
            Type type = Value.GetType();

            // filter all the fields that should be drawn
            BTypeData typeData = BInspectorPropertyFilterProvider.GetFilteredProperties(type);

            // fields
            List<BMemberData> validFields = typeData.MemberDatas;

            // style
            LabelDrawer labelDrawer = new LabelDrawer();

            // draw sub fields
            foreach (BMemberData m in validFields)
            {
                Type fieldType = ReflectionUtils.GetMemberType(m.MemberInfo);
                IValueDrawer fieldDrawer = ValueDrawerProvider.Get(fieldType);
                ChildrenValueDrawers.Add(fieldDrawer);

                // draw info
                ValueDrawerInfoBasic info = new ValueDrawerInfoBasic() { ContainingInstance = Value, MemberData = m };

                // indented context
                DrawerContext cpy = DrawerContext;
                cpy.AllDrawers.Add(fieldDrawer);
                cpy.IndentationLevel++;

                // setup
                fieldDrawer.Setup(info, this, cpy);

                // add to parent layout
                UIContainer.Add(fieldDrawer.DrawerRoot);

                // add label
                labelDrawer.Setup(fieldDrawer);
            }

            // methods
            List<MethodInfo> methods =  TypeUtils.GetAllMethods(type);

            methods = methods
                .Where( m => m.ReturnType == typeof(void) )
                .Where( m => m.GetParameters().Length == 0)
                .Where( m => m.GetCustomAttribute<BButton>() != null)
                .ToList();

            // button methods
            foreach(MethodInfo m in methods)
            {
                Button btn = new Button(() => m.Invoke(Value , null) );
                btn.text = m.Name;

                UIContainer.Add(btn);
            }

            // indentation space
            TabSpacing tabSpacing = new TabSpacing();
            tabSpacing.Setup(this);
        }

        public override void Destroy()
        {
            if (TypeSelector != null)
            {
                UIHeader.Remove(TypeSelector);
                TypeSelector.UnregisterValueChangedCallback(HandleValueTypeChanged);
                TypeSelector = null;
            }

            foreach (IValueDrawer c in ChildrenValueDrawers)
            {
                c.Destroy();
            }

            ChildrenValueDrawers.Clear();
            
            DrawerRoot.Remove(UIHeader);
            DrawerRoot.Remove(UIContainer);

            UIContainer = null;
            UIHeader = null;
        }
    }
}