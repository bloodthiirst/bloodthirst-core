using System;
using System.Collections;
using UnityEditor;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;

namespace Bloodthirst.Editor.BInspector
{
    public class ListElementDrawer : ValueDrawerBase
    {
        private const string PATH_UXML = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Implementation/ValueDrawers/List/Element/ListElementDrawer.uxml";
        private const string PATH_USS = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Implementation/ValueDrawers/List/Element/ListElementDrawer.uss";
        private static VisualTreeAsset uxmlAsset => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PATH_UXML);
        private static StyleSheet ussAsset => AssetDatabase.LoadAssetAtPath<StyleSheet>(PATH_USS);
        public Type ElementType { get; private set; }
        public IValueDrawer ValueDrawer { get; private set; }
        public VisualElement ElementContainer => DrawerContainer.Q<VisualElement>(nameof(ElementContainer));
        public Label ElementIndex => DrawerContainer.Q<Label>(nameof(ElementIndex));
        
        public int Index { get; private set; }

        public override object DefaultValue()
        {
            return ValueDrawer.DefaultValue();
        }

        protected override void PrepareData(IValueProvider valueProvider, IValueDrawer parent)
        {
            ElementType = valueProvider.DrawerType();

            Index = valueProvider.ValuePath.ListIndex;
        }

        protected override void GenerateDrawer(LayoutContext drawerContext)
        {
            uxmlAsset.CloneTree(DrawerContainer);
            DrawerContainer.styleSheets.Add(ussAsset);

            ElementIndex.text = Index.ToString();
        }

        protected override void PostLayout()
        {
            ValueDrawer = ValueDrawerProvider.Get(ElementType);

            ValueDrawerUtils.DoLayout(ValueDrawer, this, ValueProvider);

            SubDrawers.Add(ValueDrawer);
            ElementContainer.Add(ValueDrawer.DrawerContainer);
        }

        public override void Tick()
        {
            ValueDrawer.Tick();
        }

        public override void Destroy() {

            ValueDrawer.Destroy();
            SubDrawers.Remove(ValueDrawer);
            ElementContainer.Remove(ValueDrawer.DrawerContainer);
        }
    }
}