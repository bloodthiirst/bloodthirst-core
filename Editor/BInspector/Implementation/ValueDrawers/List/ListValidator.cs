using Bloodthirst.Core.Utils;
using System;
using System.Collections;

namespace Bloodthirst.Editor.BInspector
{
    public class ListValidator : IValueDrawerValidator
    {
        int IValueDrawerValidator.Order => 0;

        bool IValueDrawerValidator.CanDraw(Type type)
        {
            return TypeUtils.IsSubTypeOf(type, typeof(IList));
        }
        
        IValueDrawer IValueDrawerValidator.GetValueDrawer()
        {
            return new ListDrawer();
        }

        void IValueDrawerValidator.Initialize()
        {

        }
    }
}