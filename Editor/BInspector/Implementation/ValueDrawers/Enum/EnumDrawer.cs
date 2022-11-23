using Bloodthirst.Core.Utils;
using Bloodthirst.Editor.BInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class EnumDrawer : ValueDrawerBase
    {
        private const string PATH_UXML = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Implementation/ValueDrawers/Enum/EnumDrawer.uxml";
        private const string PATH_USS = "Packages/com.bloodthirst.bloodthirst-core/Editor/BInspector/Implementation/ValueDrawers/Enum/EnumDrawer.uss";
        private VisualElement UIContainer;

        private static VisualTreeAsset uxmlAsset => AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PATH_UXML);
        private static StyleSheet ussAsset => AssetDatabase.LoadAssetAtPath<StyleSheet>(PATH_USS);
        private Type EnumType { get; set; }

        private int[] vals;
        private string[] names;

        private PopupField<int> EnumPopUp { get; set; }

        public override object DefaultValue()
        {
            return 0;
        }

        public override void Tick()
        {
            if (EnumPopUp.focusController.focusedElement == EnumPopUp)
                return;

            int index = 0;
            for (int i =  0; i < vals.Length; i++)
            {
                if ((object) vals[i] == ValueProvider.Get())
                    index = i;
            }

            EnumPopUp.value = index;
        }


        protected override void PrepareData(IValueProvider drawerInfo, IValueDrawer parent)
        {
            EnumType = ReflectionUtils.GetMemberType(ValueProvider.MemberInfo);
        }

        protected override void GenerateDrawer(LayoutContext layoutContext)
        {
            // dropdown
            vals = Enum.GetValues(EnumType) as int[];
            names = Enum.GetNames(EnumType) as string[];

            EnumPopUp = new PopupField<int>(vals.ToList(), vals[0], (i) => names[i], (i) => names[i]);
            EnumPopUp.AddToClassList("grow-1");
            EnumPopUp.AddToClassList("shrink-1");

            // ui setup
            UIContainer = uxmlAsset.CloneTree();
            UIContainer.styleSheets.Add(ussAsset);
            UIContainer.AddToClassList("grow-1");
            UIContainer.Add(EnumPopUp);

            DrawerContainer.AddToClassList("row");
            DrawerContainer.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });
            DrawerContainer.Add(UIContainer);

            // generate getter/setter
            // if property
            if (ValueProvider.MemberInfo.MemberType == MemberTypes.Property)
            {
                PropertyInfo casted = (PropertyInfo)ValueProvider.MemberInfo;
                EnumPopUp.SetEnabled(casted.CanWrite);
            }

            EnumPopUp.RegisterValueChangedCallback(HandleValueChanged);
            EnumPopUp.SetValueWithoutNotify((int)ValueProvider.Get());
        }

        protected override void PostLayout() { }

        private void HandleValueChanged(ChangeEvent<int> evt)
        {
            DrawerValue = evt.newValue;
            ValueProvider.Set(evt.newValue);
            TriggerOnValueChangedEvent();
        }

        public override void Destroy()
        {
            EnumPopUp.UnregisterValueChangedCallback(HandleValueChanged);
            EnumPopUp = null;
        }
    }
}