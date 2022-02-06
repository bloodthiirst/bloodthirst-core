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

        public UnityObjectDrawer()
        {

        }

        protected override void PrepareUI(VisualElement root)
        {
            root.AddToClassList("row");
            root.Add(new VisualElement() { name = ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS });

            ObjectField = new ObjectField();
            ObjectField.AddToClassList("grow-1");
            root.Add(ObjectField);
        }

        public override object DefaultValue()
        {
            return null;
        }

        protected override void Postsetup()
        {
            ObjectField.objectType = DrawerInfo.DrawerType();
            ObjectField.SetValueWithoutNotify((UnityEngine.Object)Value);
            ObjectField.RegisterValueChangedCallback(HandleValueChanged);

        }

        private void HandleValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            DrawerInfo.Set(evt.newValue);
            Value = evt.newValue;

            TriggerOnValueChangedEvent();
        }

        public override void Destroy()
        {
            ObjectField.UnregisterValueChangedCallback(HandleValueChanged);
        }
    }
}