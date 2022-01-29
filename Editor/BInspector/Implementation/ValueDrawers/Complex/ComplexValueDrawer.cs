using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class ComplexValueDrawer : ValueDrawerBase
    {
        private PopupField<Type> typeSelector;
        public VisualElement UIContainer { get; private set; }
        public VisualElement UIHeader { get; private set; }

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
            Redraw();
        }

        private void ResetUI()
        {
            foreach (IValueDrawer c in ChildrenValueDrawers)
            {
                c.Clean();
            }

            ChildrenValueDrawers.Clear();

            if (typeSelector != null)
            {
                typeSelector.UnregisterValueChangedCallback(HandleValueChanged);
            }

            if (typeSelector != null)
            {
                UIHeader.Remove(typeSelector);
            }
            
            UIContainer.Clear();
        }

        private void Redraw()
        {
            ResetUI();

            InstanceCreator();

            if (Value != null)
            {
                HasValue();
            }

        }

        private string TypeSelectionFormat(Type t)
        {
            if (t == null)
            {
                return "Null";
            }

            return TypeUtils.GetNiceName(t);
        }

        private void InstanceCreator()
        {
            Type type = ReflectionUtils.GetMemberType(DrawerInfo.MemberInfo);

            List<Type> validTypes = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => TypeUtils.IsSubTypeOf(t, type))
                .ToList();

            validTypes.Insert(0, null);

            typeSelector = new PopupField<Type>(validTypes, 0, TypeSelectionFormat, TypeSelectionFormat);
            typeSelector.AddToClassList("grow-1");

            typeSelector.SetValueWithoutNotify(Value == null ? null : Value.GetType());

            typeSelector.UnregisterValueChangedCallback(HandleValueChanged);
            typeSelector.RegisterValueChangedCallback(HandleValueChanged);

            UIHeader.Add(typeSelector);
        }

        private void HandleValueChanged(ChangeEvent<Type> evt)
        {
            Type newType = evt.newValue;

            object newInstance = null;

            if (newType != null)
            {
                newInstance = Activator.CreateInstance(newType);
            }

            DrawerInfo.Set(newInstance);

            TriggerOnValueChangedEvent();

            Redraw();
        }

        private void HasValue()
        {
            // get the type data of the inspector component
            Type type = Value.GetType();
            TypeData typeData = TypeDataProvider.Get(type);

            // fields
            List<MemberData> validFields = typeData.MemberDatas.Where(m => !m.MemberInfo.Name.EndsWith("k__BackingField")).ToList();

            // style
            LabelDrawer labelDrawer = new LabelDrawer();


            // draw sub fields
            foreach (MemberData m in validFields)
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


            // indentation space
            TabSpacing tabSpacing = new TabSpacing();
            tabSpacing.Setup(this);
        }

        public override void Clean()
        {
            ResetUI();

            DrawerRoot.Clear();

            UIContainer = null;

            if (typeSelector != null)
            {
                typeSelector.UnregisterValueChangedCallback(HandleValueChanged);
                typeSelector = null;
            }
        }
    }
}