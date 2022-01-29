using Bloodthirst.Editor.BInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public abstract class ValueDrawerBase : IValueDrawer
    {
        public const string VALUE_DRAWER_CONTAINER_CLASS = "value-drawer-container";
        public const string VALUE_LABEL_CONTAINER_CLASS = "value-label-container";
        public event Action<IValueDrawer> OnValueChanged;
        public string FieldPath { get; set; }
        public IValueDrawer Parent { get; set; }
        public IValueDrawerInfo DrawerInfo { get; set; }
        public DrawerContext DrawerContext { get; set; }
        public object Value { get; set; }
        public VisualElement DrawerRoot { get; protected set; }

        protected List<IValueDrawer> childrenValueDrawers = new List<IValueDrawer>();
        public IList<IValueDrawer> ChildrenValueDrawers => childrenValueDrawers;

        public void TriggerOnValueChangedEvent()
        {
            Value = DrawerInfo.Get();
            OnValueChanged?.Invoke(this);
        }

        public abstract void Clean();

        public abstract object DefaultValue();

        protected abstract void Postsetup();

        private void Initialize(IValueDrawerInfo drawerInfo, IValueDrawer parent, DrawerContext drawerContext)
        {
            Parent = parent;
            DrawerInfo = drawerInfo;
            Value = drawerInfo.Get();
            DrawerContext = drawerContext;

            if (string.IsNullOrEmpty(Parent?.FieldPath))
            {
                FieldPath = drawerInfo.MemberInfo.Name;
            }
            else
            {
                FieldPath += $"{Parent.FieldPath}/{drawerInfo.MemberInfo.Name}"; 
            }

            DrawerRoot = new VisualElement();
            DrawerRoot.AddToClassList(VALUE_DRAWER_CONTAINER_CLASS);
            DrawerRoot.name = FieldPath.Replace('/', '-');
        }

        protected abstract void PrepareUI(VisualElement root);


        public void Setup(IValueDrawerInfo drawerInfo, IValueDrawer parent , DrawerContext drawerContext)
        {
            Initialize(drawerInfo, parent , drawerContext);
            PrepareUI(DrawerRoot);
            Postsetup();
        }


    }
}
