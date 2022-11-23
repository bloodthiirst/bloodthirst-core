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
    public class StringDrawer : ValueDrawerBase
    {
        private TextField TextField { get; set; }

        public override object DefaultValue()
        {
            return string.Empty;
        }

        protected override void GenerateDrawer(LayoutContext layoutContext)
        {
            TextField = new TextField();
            TextField.AddToClassList("grow-1");
            TextField.SetValueWithoutNotify((string)ValueProvider.Get());
            TextField.RegisterValueChangedCallback(HandleValueChanged);

            DrawerContainer.AddToClassList("row");
            DrawerContainer.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });
            DrawerContainer.Add(TextField);
        }

        public override void Tick()
        {
            if (TextField.focusController.focusedElement == TextField)
                return;

            TextField.value = (string) ValueProvider.Get();
        }

        protected override void PostLayout() { }

        private void HandleValueChanged(ChangeEvent<string> evt)
        {
            DrawerValue = evt.newValue;
            ValueProvider.Set(evt.newValue);
            TriggerOnValueChangedEvent();
        }

        public override void Destroy()
        {
            TextField.UnregisterValueChangedCallback(HandleValueChanged);
            TextField = null;
        }
    }
}