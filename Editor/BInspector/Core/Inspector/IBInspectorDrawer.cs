using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public interface IBInspectorDrawer
    {
        void Initialize();
        VisualElement CreateInspectorGUI(object instance);
    }
}
