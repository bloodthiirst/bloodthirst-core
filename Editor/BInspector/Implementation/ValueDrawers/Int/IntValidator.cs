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
    public class IntValidator : IValueDrawerValidator
    {
        int IValueDrawerValidator.Order => 0;

        public IValueDrawer GetValueDrawer()
        {
            var ui = new IntDrawer();
            return ui;
        }

        public void Initialize()
        {
        }

        bool IValueDrawerValidator.CanDraw(Type type)
        {
            return type == typeof(int);
        }
    }
}