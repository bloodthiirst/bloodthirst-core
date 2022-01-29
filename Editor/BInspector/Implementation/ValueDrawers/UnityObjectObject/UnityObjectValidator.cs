using Bloodthirst.Core.Utils;
using System;

namespace Bloodthirst.Editor.BInspector
{
    public class UnityObjectValidator : IValueDrawerValidator
    {
        int IValueDrawerValidator.Order => 0;

        public IValueDrawer GetValueDrawer()
        {
            return new UnityObjectDrawer();
        }

        public void Initialize()
        {
            
        }

        bool IValueDrawerValidator.CanDraw(Type type)
        {
            return TypeUtils.IsSubTypeOf(type, typeof(UnityEngine.Object));
        }

    }
}