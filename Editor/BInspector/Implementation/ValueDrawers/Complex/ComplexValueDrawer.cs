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
        private VisualElement UIValue { get; set; }
        private VisualElement UIContainer { get; set; }
        private VisualElement UIHeader { get; set; }

        private LayoutContext cachedLayoutContext;

        public override object DefaultValue()
        {
            Type fieldType = ValueProvider.DrawerType();
            return ReflectionUtils.GetDefaultValue(fieldType);
        }

        public override void Tick()
        {
            object currentValue = ValueProvider.Get();

            if (currentValue == DrawerValue)
            {
                for (int i = 0; i < SubDrawers.Count; i++)
                {
                    IValueDrawer s = SubDrawers[i];
                    s.Tick();
                }
            }
            else
            {
                DrawerValue = currentValue;
                Clean();
                Draw(cachedLayoutContext);
            }
        }

        protected override void GenerateDrawer(LayoutContext layoutContext)
        {
            DrawerContainer.AddToClassList("row");

            // label
            VisualElement labelContainer = new VisualElement() { name = VALUE_LABEL_CONTAINER_CLASS };

            UIValue = new VisualElement();
            UIValue.AddToClassList("column");
            UIValue.AddToClassList("grow-1");
            UIValue.AddToClassList("shrink-1");
            
            // header
            UIHeader = new VisualElement();
            UIHeader.AddToClassList("row");
            UIHeader.AddToClassList("grow-1");
            UIHeader.AddToClassList("shrink-1");
            
            // container
            UIContainer = new VisualElement();
            UIContainer.AddToClassList("column");
            UIContainer.AddToClassList("grow-1");
            UIContainer.AddToClassList("shrink-1");

            UIValue.Add(UIHeader);
            UIValue.Add(UIContainer);

            DrawerContainer.Add(labelContainer);
            DrawerContainer.Add(UIValue);

            cachedLayoutContext = layoutContext;
            Draw(layoutContext);
        }

        protected override void PostLayout()
        {

        }

        private void Clean()
        {
            if (TypeSelector != null)
            {
                UIHeader.Remove(TypeSelector);
                TypeSelector.UnregisterValueChangedCallback(HandleValueTypeChanged);
                TypeSelector = null;
            }

            for (int i = 0; i < SubDrawers.Count; i++)
            {
                IValueDrawer c = SubDrawers[i];
                cachedLayoutContext.AllDrawers.Remove(c);
                c.Destroy();
            }

            SubDrawers.Clear();
            UIContainer.Clear();
            UIHeader.Clear();
        }

        private void Draw(LayoutContext layoutContext)
        {
            Type type = ValueProvider.DrawerType();

            // if we're not in a root object and we are a class or interface type (not a struct or enum)
            if (ValueProvider.ValuePath.PathType != PathType.ROOT && (type.IsClass || type.IsInterface))
            {
                InstanceCreator();
            }

            if (ValueProvider.Get() != null)
            {
                GenerateSubFields(layoutContext);
            }

        }

        private void InstanceCreator()
        {
            Type type = ValueProvider.DrawerType();

            TypeSelector = new TypeSelector(type);
            TypeSelector.AddToClassList("grow-1");

            Type instanceType = ValueProvider.Get() == null ? null : ValueProvider.Get().GetType();

            TypeSelector.SetValueWithoutNotify(instanceType);
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

            DrawerValue = newInstance;
            ValueProvider.Set(newInstance);
            TriggerOnValueChangedEvent();

            Clean();
            Draw(cachedLayoutContext);
        }

        private void GenerateSubFields(LayoutContext layoutContext)
        {
            // todo : put all the fields in a foldout parent

            // get the type data of the inspector component
            Type type = ValueProvider.Get().GetType();

            // filter all the fields that should be drawn
            BTypeData typeData = BInspectorPropertyFilterProvider.GetFilteredProperties(type);

            // fields
            List<BMemberData> validFields = typeData.MemberDatas;

            // style
            LabelDrawer labelDrawer = new LabelDrawer();

            // draw sub fields
            for (int i = 0; i < validFields.Count; i++)
            {
                BMemberData m = validFields[i];
                Type fieldType = ReflectionUtils.GetMemberType(m.MemberInfo);
                IValueDrawer fieldDrawer = ValueDrawerProvider.Get(fieldType);
                SubDrawers.Add(fieldDrawer);

                // draw info
                ValueDrawerInfoBasic info = new ValueDrawerInfoBasic() 
                { 
                    ValuePath = new ValuePath() { PathType = PathType.FIELD, FieldName = m.MemberInfo.Name }, 
                    ContainingInstance = ValueProvider.Get(), 
                    MemberData = m 
                };

                // indented context
                layoutContext.AllDrawers.Add(fieldDrawer);
                layoutContext.IndentationLevel++;

                // setup
                ValueDrawerUtils.DoLayout(fieldDrawer , this , info);

                // todo : make the padding a styling pass
                fieldDrawer.DrawerContainer.AddToClassList("m-5");

                // add to parent layout
                UIContainer.Add(fieldDrawer.DrawerContainer);
            }

            // methods
            List<MethodInfo> methods = TypeUtils.GetAllMethods(type);

            methods = methods
                .Where(m => m.ReturnType == typeof(void))
                .Where(m => m.GetParameters().Length == 0)
                .Where(m => m.GetCustomAttribute<BButton>() != null)
                .ToList();

            // button methods
            foreach (MethodInfo m in methods)
            {
                Button btn = new Button(() => m.Invoke(ValueProvider.Get(), null));
                btn.text = m.Name;

                UIContainer.Add(btn);
            }


        }

        public override void Destroy()
        {
            if (TypeSelector != null)
            {
                UIHeader.Remove(TypeSelector);
                TypeSelector.UnregisterValueChangedCallback(HandleValueTypeChanged);
                TypeSelector = null;
            }

            foreach (IValueDrawer c in SubDrawers)
            {
                c.Destroy();
            }

            SubDrawers.Clear();

            UIValue.Remove(UIHeader);
            UIValue.Remove(UIContainer);

            DrawerContainer.Remove(UIValue);

            UIContainer = null;
            UIHeader = null;
            UIValue = null;
        }
    }
}