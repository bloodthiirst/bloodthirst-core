using Bloodthirst.Editor.BInspector;
using Bloodthirst.Runtime.BRecorder;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BRecorder
{
    public class BRecorderInfoUI : VisualElement
    {
        public IBRecorderCommand Command { get; private set; }

        public BRecorderInfoUI(IBRecorderCommand command)
        {
            Command = command;

            IBInspectorDrawer drawer = BInspectorProvider.DefaultInspector;

            VisualElement inspector = drawer.CreateInspectorGUI(Command).RootContainer;
            Add(inspector);

            AddToClassList("cmd-main");
            style.position = new StyleEnum<Position>(Position.Absolute);
        }

    }
}
