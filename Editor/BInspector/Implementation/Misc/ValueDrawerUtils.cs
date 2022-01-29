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
    public static class ValueDrawerUtils
    {
        public static Label GetLabel(IValueDrawer valueDrawer)
        {
            VisualElement labelContainer = valueDrawer.DrawerRoot.Q<VisualElement>(ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS);

            if (labelContainer == null)
                return null;

            return labelContainer.Q<Label>();
        }
        public static float GetTextLength(string text)
        {
            return text.Length * 8;
        }
    }



}