using System;

namespace Bloodthirst.Editor.BInspector
{
    public interface IValueDrawerValidator
    {
        void Initialize();
        int Order { get; }

        bool CanDraw(Type type);

        IValueDrawer GetValueDrawer();
    }
}
