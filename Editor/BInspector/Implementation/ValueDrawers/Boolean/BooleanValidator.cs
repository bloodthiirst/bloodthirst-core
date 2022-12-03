using Bloodthirst.Editor.BInspector;
#if ODIN_INSPECTOR
	using Sirenix.Utilities;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class BooleanValidator : IValueDrawerValidator
    {
        int IValueDrawerValidator.Order => 0;

        public IValueDrawer GetValueDrawer()
        {
            var ui = new BooleanDrawer();
            return ui;
        }

        public void Initialize()
        {
        }

        bool IValueDrawerValidator.CanDraw(Type type)
        {
            return type == typeof(bool);
        }
    }
}