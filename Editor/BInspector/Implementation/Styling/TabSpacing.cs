using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class TabSpacing
    {
        private const float DEFAULT_INDENTATION = 40;

        public void Setup(IValueDrawer valueDrawer)
        {
            VisualElement currContainer = valueDrawer.DrawerContainer;

            // is value container ?
            if (!currContainer.ClassListContains(ValueDrawerBase.VALUE_DRAWER_CONTAINER_CLASS))
                return;

            Label label = currContainer.Q<VisualElement>(ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS)?.Q<Label>(className: ValueDrawerBase.VALUE_LABEL_CLASS);

            // has a label ?
            if (label == null)
                return;
            

            foreach (IValueDrawer c in valueDrawer.SubDrawers)
            {
                //c.DrawerContainer.style.paddingLeft = new StyleLength(label.resolvedStyle.width);
                c.DrawerContainer.style.paddingLeft = new StyleLength(0f);
            }
        }
    }
}