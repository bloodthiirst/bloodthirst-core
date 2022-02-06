using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public interface IValueDrawer
    {
        string FieldPath { get; set; }

        event Action<IValueDrawer> OnValueChanged;
        IValueDrawer Parent { get; set; }
        IValueDrawerInfo DrawerInfo { get; set; }
        DrawerContext DrawerContext { get; set; }

        object Value { get; set; }

        IList<IValueDrawer> ChildrenValueDrawers { get; }
             
        VisualElement DrawerRoot { get; }

        object DefaultValue();

        void Setup(IValueDrawerInfo drawerInfo , IValueDrawer parent , DrawerContext drawerContext);

        void Destroy();
    }
}
