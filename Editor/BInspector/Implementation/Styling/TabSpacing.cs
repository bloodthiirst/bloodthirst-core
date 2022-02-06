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
            // if we are at the root level instance
            // then we don't add padding and we just skip the children
            if(valueDrawer.Parent == null)
            {
                foreach (IValueDrawer c in valueDrawer.ChildrenValueDrawers)
                {

                    if (!c.DrawerRoot.ClassListContains(ValueDrawerBase.VALUE_DRAWER_CONTAINER_CLASS))
                        continue;

                    RecursivelyAddTabSpacing(c);
                }

                return;
            }
            
            Label label = ValueDrawerUtils.GetLabel(valueDrawer);

            float padding = DEFAULT_INDENTATION;

            if (label != null)
            {
                padding = label.resolvedStyle.width;
            }

            foreach (IValueDrawer c in valueDrawer.ChildrenValueDrawers)
            {

                if (!c.DrawerRoot.ClassListContains(ValueDrawerBase.VALUE_DRAWER_CONTAINER_CLASS))
                    continue;
                
                c.DrawerRoot.style.paddingLeft = new StyleLength( padding);

                RecursivelyAddTabSpacing(c);
            }
            
        }

        public void Setup(IValueDrawer valueDrawer)
        {
            RecursivelyAddTabSpacing(valueDrawer);
            
        }
    }
}