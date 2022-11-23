using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public class LabelDrawer
    {
        public void Setup(IValueDrawer valueDrawer)
        {
            // todo : make the LabelDrawer actually create the labelcontainer instead of it searching for it in the actual drawer
            if (valueDrawer.ValueProvider.ValuePath.PathType == PathType.ROOT ||
                valueDrawer.ValueProvider.ValuePath.PathType == PathType.CUSTOM ||
                valueDrawer.ValueProvider.ValuePath.PathType == PathType.LIST_ENTRY )
                return;

            // find label placement
            VisualElement labelContainer = valueDrawer.DrawerContainer.Q<VisualElement>(ValueDrawerBase.VALUE_LABEL_CONTAINER_CLASS);

            if (labelContainer == null)
                return;

            // this is done to only pick up the label of the "current value drawer" and not pick one that belongs to its children
            if (labelContainer.parent != valueDrawer.DrawerContainer)
                return;

            // add label
            Label label = new Label();
            label.AddToClassList(ValueDrawerBase.VALUE_LABEL_CLASS);

            string asString = valueDrawer.ValueProvider.ValuePath.ValuePathAsString();
            label.name = $"Label_{asString}";
            label.text = asString;

            labelContainer.Add(label);
        }
    }
}