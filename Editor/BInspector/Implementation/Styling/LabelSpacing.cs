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
    public class LabelSpacing
    {
        public void Setup(VisualElement drawer)
        {
            VisualElement elem = drawer;

            List<VisualElement> labelsToResize = new List<VisualElement>();
            float maxSize = -1;

            foreach (VisualElement curr in elem.Children())
            {

                if (!curr.ClassListContains(ValueDrawerBase.VALUE_DRAWER_CONTAINER_CLASS))
                    continue;

                Label label = curr.Q<VisualElement>(ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS)?.Q<Label>(className: "member-label");

                if (label == null)
                    continue;

                labelsToResize.Add(label);
                Vector2 mesure = label.MeasureTextSize(label.text, label.resolvedStyle.width, VisualElement.MeasureMode.Exactly, label.resolvedStyle.height, VisualElement.MeasureMode.Exactly);
                maxSize = Mathf.Max(maxSize, mesure.x);
            }

            foreach (VisualElement l in labelsToResize)
            {
                l.style.width = new StyleLength(new Length(maxSize, LengthUnit.Pixel));
            }

        }


    }
}