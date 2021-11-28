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
    public class IntDrawer : IValueDrawer
    {
        public object Value { get; set; } = 0;

        public VisualElement VisualElement { get; private set; }

        private IntegerField IntField { get; set; }

        void IValueDrawer.Initialize()
        {

        }

        public object DefaultValue()
        {
            return 0;
        }

        public IntDrawer()
        {
            IntField = new IntegerField();
            VisualElement = IntField;
        }

        public void Setup(IDrawerInfo drawerInfo)
        {
            IntField.SetValueWithoutNotify((int)drawerInfo.Get());
            IntField.userData = drawerInfo;
            IntField.RegisterValueChangedCallback(HandleValueChanged);
        }

        private void HandleValueChanged(ChangeEvent<int> evt)
        {
            IDrawerInfo info = (IDrawerInfo)((VisualElement)evt.currentTarget).userData;
            info.Set(evt.newValue);
        }

        public void Clean()
        {

        }
    }
}