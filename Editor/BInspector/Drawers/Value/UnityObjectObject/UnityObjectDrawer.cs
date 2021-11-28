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
    public class UnityObjectDrawer : IValueDrawer
    {
        public object Value { get; set; }

        public VisualElement VisualElement { get; private set; }

        private ObjectField ObjectField { get; set; }

        public UnityObjectDrawer()
        {
            ObjectField = new ObjectField();
            VisualElement = ObjectField;
        }
        public void Initialize()
        {

        }

        void IValueDrawer.Setup(IDrawerInfo drawerInfo)
        {
            ObjectField.objectType = drawerInfo.DrawerType();
            ObjectField.SetValueWithoutNotify((UnityEngine.Object)drawerInfo.Get());
            ObjectField.userData = drawerInfo;
            ObjectField.RegisterValueChangedCallback(HandleValueChanged);

        }

        private void HandleValueChanged(ChangeEvent<UnityEngine.Object> evt)
        {
            IDrawerInfo info = (IDrawerInfo)((VisualElement)evt.currentTarget).userData;
            info.Set(evt.newValue);
        }


        public void Clean()
        {
            
        }

        public object DefaultValue()
        {
            return null;
        }
    }
}