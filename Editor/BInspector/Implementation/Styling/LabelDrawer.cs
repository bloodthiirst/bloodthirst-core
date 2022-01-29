using Bloodthirst.Editor.BInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class LabelDrawer
    {
        public void Setup(IValueDrawer valueDrawer)
        {
            // find label placement
            VisualElement labelContainer = valueDrawer.DrawerRoot.Q<VisualElement>(ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS);

            if (labelContainer == null)
                return;

            // add label
            Label label = new Label();
            label.AddToClassList("member-label");
            label.name = $"Label_{valueDrawer.DrawerInfo.MemberInfo.Name}";
            label.text = valueDrawer.DrawerInfo.MemberInfo.Name;

            labelContainer.Add(label);
        }
    }
}