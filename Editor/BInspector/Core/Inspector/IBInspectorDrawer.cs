using UnityEngine.UIElements;
using static Bloodthirst.Editor.BInspector.BInspectorDefault;

namespace Bloodthirst.Editor.BInspector
{
    public interface IBInspectorDrawer
    {
        void Initialize();
        RootEditor CreateInspectorGUI(object instance);
    }
}
