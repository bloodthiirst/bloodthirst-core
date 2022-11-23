using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class LabelSpacing
    {
        public void Setup(IValueDrawer drawer)
        {
            List<VisualElement> labelsToResize = new List<VisualElement>();
            float maxSize = 0;


            foreach (IValueDrawer curr in drawer.SubDrawers)
            {
                VisualElement currContainer = curr.DrawerContainer;

                // is value container ?
                if (!currContainer.ClassListContains(ValueDrawerBase.VALUE_DRAWER_CONTAINER_CLASS))
                    continue;

                Label label = currContainer.Q<VisualElement>(ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS)?.Q<Label>(className: ValueDrawerBase.VALUE_LABEL_CLASS);

                // has a label ?
                if (label == null)
                    continue;

                labelsToResize.Add(label);

                float width = label.resolvedStyle.width;
                float height = label.resolvedStyle.height;

                Vector2 mesure = label.MeasureTextSize(label.text,
                    width, VisualElement.MeasureMode.Undefined,
                    height, VisualElement.MeasureMode.Undefined);

                maxSize = Mathf.Max(maxSize, mesure.x);
            }

            
            foreach (VisualElement l in labelsToResize)
            {
                l.style.width = new StyleLength(new Length(maxSize, LengthUnit.Pixel));
            }

        }


    }
}