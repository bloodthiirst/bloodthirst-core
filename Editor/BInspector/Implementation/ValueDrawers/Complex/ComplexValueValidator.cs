using System;

namespace Bloodthirst.Editor.BInspector
{
    public class ComplexValueValidator : IValueDrawerValidator
    {
        int IValueDrawerValidator.Order => 0;

        public IValueDrawer GetValueDrawer()
        {
            return new ComplexValueDrawer();
        }

        public void Initialize()
        {
        }

        bool IValueDrawerValidator.CanDraw(Type type)
        {
            return true;
        }
    }
}