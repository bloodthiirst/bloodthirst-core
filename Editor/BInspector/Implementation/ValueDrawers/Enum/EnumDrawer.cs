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
        private PopupField<int> EnumPopUp { get; set; }

        public EnumDrawer()
        {
            
        }
        protected override void PrepareUI(VisualElement root)
        {
            // ui setup
            root.AddToClassList("row");
            root.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });

            UIContainer = uxmlAsset.CloneTree();

            UIContainer.styleSheets.Add(ussAsset);
            UIContainer.AddToClassList("grow-1");
            root.Add(UIContainer);
        }

        public override object DefaultValue()
        {
            return 0;
        }


        protected override void Postsetup()
        {
            Type t = ReflectionUtils.GetMemberType(DrawerInfo.MemberInfo);

            int[] vals = Enum.GetValues(t) as int[];
            string[] names = Enum.GetNames(t) as string[];

            EnumPopUp = new PopupField<int>(vals.ToList(), vals[0], (i) => names[i], (i) => names[i]);
            EnumPopUp.AddToClassList("m-0");
            UIContainer.Add(EnumPopUp);


            // generate getter/setter
            // if property
            if (DrawerInfo.MemberInfo.MemberType == MemberTypes.Property)
            {
                PropertyInfo casted = (PropertyInfo)DrawerInfo.MemberInfo;

                EnumPopUp.SetEnabled(casted.CanWrite);
            }

            EnumPopUp.RegisterValueChangedCallback(HandleValueChanged);
            EnumPopUp.SetValueWithoutNotify((int)DrawerInfo.Get());

        }

        private void HandleValueChanged(ChangeEvent<int> evt)
        {
            DrawerInfo.Set(evt.newValue);
            TriggerOnValueChangedEvent();
        }

        public override void Clean()
        {
            EnumPopUp.UnregisterValueChangedCallback(HandleValueChanged);
            EnumPopUp = null;
        }
    }
}