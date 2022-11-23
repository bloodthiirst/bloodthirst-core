using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public abstract class ValueDrawerBase : IValueDrawer
    {
        public const string VALUE_DRAWER_CONTAINER_CLASS = "value-drawer-container";
        public const string VALUE_LABEL_CONTAINER_CLASS = "value-label-container";
        public const string VALUE_LABEL_CLASS = "value-label";


        public event Action<IValueDrawer> OnDrawerValueChanged;
        
        
        private List<IValueDrawer> allChildren = new List<IValueDrawer>();
        public object DrawerValue { get; set; }
        public IValueDrawer ParentDrawer { get; set; }
        public IValueProvider ValueProvider { get; set; }
        public VisualElement DrawerContainer { get; protected set; }

        protected List<IValueDrawer> childrenValueDrawers = new List<IValueDrawer>();
        public IList<IValueDrawer> SubDrawers => childrenValueDrawers;
        protected void TriggerOnValueChangedEvent()
        {
            OnDrawerValueChanged?.Invoke(this);
        }

        public abstract void Tick();
        public abstract object DefaultValue();
        protected virtual void PrepareData(IValueProvider drawerInfo, IValueDrawer parent) { }
        protected abstract void GenerateDrawer(LayoutContext drawerContext);
        protected abstract void PostLayout();
        private static void HandleOnChange(GeometryChangedEvent evt)
        {
            IValueDrawer drawer = (evt.target as VisualElement).userData as IValueDrawer;

            if (drawer == null)
                return;

            ValueDrawerUtils.spacingStyle.Setup(drawer);
        }

        public abstract void Destroy();
        void IValueDrawer.PrepareData(IValueProvider valueProvider, IValueDrawer parent)
        {
            ParentDrawer = parent;
            ValueProvider = valueProvider;
            DrawerValue = ValueProvider.Get();

            PrepareData(valueProvider, parent);
        }


        public void AddChild(IValueDrawer child)
        {
            if(ValueProvider.ValuePath.PathType == PathType.ROOT)
            {
                child.DrawerContainer.userData = child;
                child.DrawerContainer.UnregisterCallback<GeometryChangedEvent>(HandleOnChange);
                child.DrawerContainer.RegisterCallback<GeometryChangedEvent>(HandleOnChange);
                allChildren.Add(child);

                ValueDrawerUtils.labelDrawer.Setup(child);
                ValueDrawerUtils.spacingStyle.Setup(child);
                return;
            }

            ParentDrawer.AddChild(child);
        }

        public void RemoveChild(IValueDrawer child)
        {
            if (ValueProvider.ValuePath.PathType == PathType.ROOT)
            {
                child.DrawerContainer.UnregisterCallback<GeometryChangedEvent>(HandleOnChange);
                allChildren.Remove(child);
                return;
            }

            ParentDrawer.RemoveChild(child);
        }


        public virtual void GenerateContainer()
        {
            DrawerContainer = new VisualElement();
            DrawerContainer.AddToClassList(VALUE_DRAWER_CONTAINER_CLASS);
            DrawerContainer.name = ValueProvider.ValuePath.ValuePathAsString();
        }

        void IValueDrawer.GenerateContainer()
        {
            GenerateContainer();
        }

        void IValueDrawer.GenerateDrawer(LayoutContext drawerContext)
        {
            GenerateDrawer(drawerContext);
        }
        void IValueDrawer.PostLayout()
        {
            PostLayout();
        }
        void IValueDrawer.Destroy()
        {
            Destroy();
        }
    }
}
