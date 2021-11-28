using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    public interface IValueDrawer
    {
        void Initialize();
        
        object Value { get; set; }

        void Setup(IDrawerInfo drawerInfo);

        VisualElement VisualElement { get; }

        object DefaultValue();

        void Clean();
    }
}
