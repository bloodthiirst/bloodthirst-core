using Bloodthirst.Editor.BInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class EnumDrawer : IValueDrawer
    {
        private const string PATH_UXML =    "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Drawers/Value/Enum/EnumDrawer.uxml";
        private const string PATH_USS =     "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Drawers/Value/Enum/EnumDrawer.uss";

        private static VisualTreeAsset uxmlAsset => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PATH_UXML);
        private static StyleSheet ussAsset => AssetDatabase.LoadAssetAtPath<StyleSheet>(PATH_USS);

        public IDrawerInfo DrawerInfo { get; private set; }
        public object Value { get; set; } = 0;
        public Type EnumType { get; private set; }
        public VisualElement VisualElement { get; private set; }
        public VisualElement EnumSelectionZone { get; private set; }
        public Label CurrentValue { get; private set; }
        public VisualElement SelectionListContainer { get; private set; }
        public ListView EnumListView { get; private set; }
        private List<Tuple<string,int>> EnumNameToValue { get; set; }

        void IValueDrawer.Initialize()
        {

        }

        public object DefaultValue()
        {
            return 0;
        }

        public EnumDrawer()
        {

            // ui setup
            VisualElement ui = uxmlAsset.CloneTree();

            ui.styleSheets.Add(ussAsset);
            VisualElement = ui;
        }

        public void Setup(IDrawerInfo drawerInfo)
        {
            // init values & data
            DrawerInfo = drawerInfo;
            Value = drawerInfo.Get();

            // get the enum type
            EnumType = drawerInfo.DrawerType();

            // get the enum values
            Array vals = Enum.GetValues(EnumType);
            EnumNameToValue = new List<Tuple<string, int>>(vals.Length);

            foreach(int val in vals)
            {
                Tuple<string, int> curr = new Tuple<string, int>(Enum.GetName(EnumType, val), val);
                EnumNameToValue.Add(curr);
            }


            Value = drawerInfo.Get();

            // current value
            CurrentValue = VisualElement.Q<Label>(nameof(CurrentValue));
            CurrentValue.text = Value.ToString();

            // click to open the selection list
            EnumSelectionZone = VisualElement.Q<VisualElement>(nameof(EnumSelectionZone));
            EnumSelectionZone.RegisterCallback<ClickEvent>(HandleClickDropdown);

            // contains the enum list
            SelectionListContainer = VisualElement.Q<VisualElement>(nameof(SelectionListContainer));

            /*
            EnumListView = new ListView(EnumNameToValue , 20 , CreateEnumElement, SetupEnumElement );
            EnumListView.styleSheets.Add(ussAsset);
            EnumListView.AddToClassList("SelectionList");


            VisualElement dropdownParent = GetDropdownContainer();

            dropdownParent.Add(EnumListView);

            Vector2 point = SelectionListContainer.ChangeCoordinatesTo(dropdownParent, Vector2.zero);

            EnumListView.transform.position = SelectionListContainer.transform.position;

            EnumListView.Refresh();
            */

        }

        private VisualElement GetDropdownContainer()
        {
            string name = BInspectorDefault.MainContentContainer;

            VisualElement parent = VisualElement.parent;

            while(parent != null && !parent.name.Equals(name))
            {
                parent = parent.parent;
            }

            if (parent == null)
                return null;


            return parent.parent.Q<VisualElement>(BInspectorDefault.DropdownContainer);
        }

        private void HandleClickDropdown(ClickEvent evt)
        {

        }

        private void SetupEnumElement(VisualElement createdElem, int elemIndex)
        {
            EnumElementDrawer elem = createdElem.userData as EnumElementDrawer;
            Tuple<string, int> curr = EnumNameToValue[elemIndex];
            elem.Setup(curr.Item1 , curr.Item2);
        }

        private VisualElement CreateEnumElement()
        {
            EnumElementDrawer elem = new EnumElementDrawer();
            elem.VisualElement.userData = elem;
            return elem.VisualElement;
        }

        public void Clean()
        {

        }
    }
}