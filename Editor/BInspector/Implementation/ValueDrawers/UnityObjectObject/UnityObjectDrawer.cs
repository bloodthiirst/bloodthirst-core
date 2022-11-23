using Bloodthirst.Core.Utils;
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
    public class UnityObjectDrawer : ValueDrawerBase
    {
        private ObjectField ObjectField { get; set; }

        protected override void GenerateDrawer(LayoutContext layoutContext)
        {
            ObjectField = new ObjectField();
            ObjectField.AddToClassList("grow-1");
            ObjectField.AddToClassList("shrink-1");

            ObjectField.objectType = ValueProvider.DrawerType();
            object val = ValueProvider.Get();
            ObjectField.SetValueWithoutNotify((UnityEngine.Object)val);
            ObjectField.RegisterValueChangedCallback(HandleValueChanged);

            DrawerContainer.AddToClassList("row");
            DrawerContainer.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });
            DrawerContainer.Add(ObjectField);
        }

        public override object DefaultValue()
        {
            return null;
        }

        public override void Tick()
        {
            if (ObjectField.focusController.focusedElement == ObjectField)
                return;

            ObjectField.value =(UnityEngine.Object)ValueProvider.Get();
        }

        protected override void PostLayout() { }

        private void HandleValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            DrawerValue = evt.newValue;
            ValueProvider.Set(evt.newValue);
            TriggerOnValueChangedEvent();
        }

        public override void Destroy()
        {
            ObjectField.UnregisterValueChangedCallback(HandleValueChanged);
        }
    }
}