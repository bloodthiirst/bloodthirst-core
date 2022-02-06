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
        
        public StringDrawer()
        {

        }

        protected override void PrepareUI(VisualElement root)
        {
            root.AddToClassList("row");
            root.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });

            TextField = new TextField();
            TextField.AddToClassList("grow-1");
            root.Add(TextField);
        }
        public override object DefaultValue()
        {
            return string.Empty;
        }


        protected override void Postsetup()
        {
            TextField.SetValueWithoutNotify((string) Value);
            TextField.RegisterValueChangedCallback(HandleValueChanged);
        }

        private void HandleValueChanged(ChangeEvent<string> evt)
        {
            DrawerInfo.Set(evt.newValue);
            Value = evt.newValue;
            TriggerOnValueChangedEvent();
        }

        public override void Destroy()
        {
            TextField.UnregisterValueChangedCallback(HandleValueChanged);
            TextField = null;
        }
    }
}