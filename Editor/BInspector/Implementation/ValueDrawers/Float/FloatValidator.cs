using System;

namespace Bloodthirst.Editor.BInspector
{
    public class FloatValidator : IValueDrawerValidator
    {
        int IValueDrawerValidator.Order => 0;

        public IValueDrawer GetValueDrawer()
        {
            FloatDrawer ui = new FloatDrawer();
            return ui;
        }

        public void Initialize()
        {
        }

        bool IValueDrawerValidator.CanDraw(Type type)
        {
            return type == typeof(float);
        }
    }
}