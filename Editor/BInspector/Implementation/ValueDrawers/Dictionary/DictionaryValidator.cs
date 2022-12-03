using Bloodthirst.Core.Utils;
using Bloodthirst.Editor.BInspector;
#if ODIN_INSPECTOR
	using Sirenix.Utilities;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class DictionaryValidator : IValueDrawerValidator
    {
        int IValueDrawerValidator.Order => 0;

        bool IValueDrawerValidator.CanDraw(Type type)
        {
            return TypeUtils.IsSubTypeOf(type, typeof(IDictionary));
        }
        
        IValueDrawer IValueDrawerValidator.GetValueDrawer()
        {
            return new DictionaryDrawer();
        }

        void IValueDrawerValidator.Initialize()
        {

        }
    }
}