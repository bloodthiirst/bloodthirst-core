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
    public class IntDrawer : ValueDrawerBase
    {
        private IntegerField IntField { get; set; }

        public IntDrawer()
        {

        }

        protected override void PrepareUI(VisualElement root)
        {
            root.AddToClassList("row");
            root.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });

            IntField = new IntegerField();
            IntField.AddToClassList("grow-1");
            root.Add(IntField);
        }

        public override object DefaultValue()
        {
            return 0;
        }

        protected override void Postsetup()
        {
            IntField.SetValueWithoutNotify((int)DrawerInfo.Get());
            IntField.RegisterValueChangedCallback(HandleValueChanged);
        }

        private void HandleValueChanged(ChangeEvent<int> evt)
        {
            DrawerInfo.Set(evt.newValue);
            TriggerOnValueChangedEvent();
        }

        public  override void Clean()
        {
            IntField.RegisterValueChangedCallback(HandleValueChanged);
            IntField = null;
        }
    }
}