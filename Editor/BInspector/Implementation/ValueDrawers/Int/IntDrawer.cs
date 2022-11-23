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

        public override object DefaultValue()
        {
            return 0;
        }

        public override void Tick()
        {
            if (IntField.focusController.focusedElement == IntField)
                return;

            IntField.value = (int)ValueProvider.Get();
        }

        protected override void GenerateDrawer(LayoutContext layoutContext)
        {
            IntField = new IntegerField();
            IntField.AddToClassList("grow-1");
            IntField.AddToClassList("shrink-1");
            IntField.SetValueWithoutNotify((int)ValueProvider.Get());
            IntField.RegisterValueChangedCallback(HandleValueChanged);

            DrawerContainer.AddToClassList("row");
            DrawerContainer.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });

            DrawerContainer.AddToClassList("grow-1");
            DrawerContainer.Add(IntField);
        }

        private void HandleValueChanged(ChangeEvent<int> evt)
        {
            DrawerValue = evt.newValue;
            ValueProvider.Set(evt.newValue);
            TriggerOnValueChangedEvent();
        }

        protected override void PostLayout() { }

        public  override void Destroy()
        {
            IntField.RegisterValueChangedCallback(HandleValueChanged);
            IntField = null;
        }
    }
}