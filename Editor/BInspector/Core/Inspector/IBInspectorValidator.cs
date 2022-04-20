using System;

namespace Bloodthirst.Editor.BInspector
{
    public interface IBInspectorValidator
    {
        void Initialize();
        int Order { get; }

        bool CanInspect(Type type , object instance);

        IBInspectorDrawer GetDrawer();
    }
}
