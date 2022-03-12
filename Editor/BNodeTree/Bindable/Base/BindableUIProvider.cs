using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodthirst.Editor.BNodeTree
{
    public static class BindableUIProvider
    {
        private static List<IBindableUIFactory> uiFactory = BindableUIs();
        public static List<IBindableUIFactory> UIFactory => uiFactory;

        static List<IBindableUIFactory> BindableUIs()
        {
            return TypeUtils.AllTypes
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface)
                .Where(t => TypeUtils.IsSubTypeOf(t , (typeof(IBindableUIFactory))))
                .Select(t =>(IBindableUIFactory) Activator.CreateInstance(t))
                .ToList();
        }

    }
}
