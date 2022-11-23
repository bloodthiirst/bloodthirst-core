using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public enum PathType
    {
        CUSTOM,
        ROOT,
        STATIC,
        FIELD,
        LIST_ENTRY,
        DICTIONARY_ENTRY
    }
    public struct ValuePath
    {
        public PathType PathType { get; set; }
        public object DictionaryKey { get; set; }
        public int ListIndex { get; set; }
        public string FieldName { get; set; }
        public string Custom { get; set; }

    }
    public interface IValueDrawer
    {
        event Action<IValueDrawer> OnDrawerValueChanged;
        object DrawerValue { get; set; }
        IValueDrawer ParentDrawer { get; set; }
        IList<IValueDrawer> SubDrawers { get; }
        IValueProvider ValueProvider { get; }
        object DefaultValue();
        VisualElement DrawerContainer { get; }
        void PrepareData(IValueProvider drawerInfo, IValueDrawer parent);
        void AddChild(IValueDrawer valueDrawer);
        void RemoveChild(IValueDrawer valueDrawer);
        void GenerateContainer();
        void GenerateDrawer(LayoutContext drawerContext);
        void PostLayout();
        void Tick();
        void Destroy();
    }
}
