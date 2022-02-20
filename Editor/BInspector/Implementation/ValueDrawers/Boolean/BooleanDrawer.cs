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
    public class BooleanDrawer : ValueDrawerBase
    {
        private Toggle BoolField { get; set; }

        public BooleanDrawer()
        {

        }

        protected override void PrepareUI(VisualElement root)
        {
            root.AddToClassList("row");
            root.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });

            BoolField = new Toggle();
            BoolField.AddToClassList("grow-1");
            root.Add(BoolField);
        }

        public override object DefaultValue()
        {
            return 0;
        }

        protected override void Postsetup()
        {
            BoolField.SetValueWithoutNotify((bool) Value);
            BoolField.RegisterValueChangedCallback(HandleValueChanged);

        }

        private void HandleValueChanged(ChangeEvent<bool> evt)
        {
            DrawerInfo.Set(evt.newValue);
            TriggerOnValueChangedEvent();
        }

        public  override void Destroy()
        {
            BoolField.RegisterValueChangedCallback(HandleValueChanged);
            BoolField = null;
        }
    }
}