using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class TabSpacing
    {
        private const float DEFAULT_INDENTATION = 40;

        private void RecursivelyAddTabSpacing(IValueDrawer valueDrawer)
        {
            Label label = ValueDrawerUtils.GetLabel(valueDrawer);

            float padding = DEFAULT_INDENTATION;
            
            if(label != null)
            {
                /*
                Vector2 mesure = label.MeasureTextSize(label.text, label.resolvedStyle.width, VisualElement.MeasureMode.AtMost, label.resolvedStyle.height, VisualElement.MeasureMode.AtMost);
                padding = mesure.x;
                */

                padding = label.resolvedStyle.width;
            }

            foreach (IValueDrawer c in valueDrawer.ChildrenValueDrawers)
            {

                if (!c.DrawerRoot.ClassListContains(ValueDrawerBase.VALUE_DRAWER_CONTAINER_CLASS))
                    continue;
                
                c.DrawerRoot.style.paddingLeft = new StyleLength(/*tabDiff * */ padding);

                RecursivelyAddTabSpacing(c);
            }
            
        }

        public void Setup(IValueDrawer valueDrawer)
        {
            RecursivelyAddTabSpacing(valueDrawer);
            
        }
    }
}