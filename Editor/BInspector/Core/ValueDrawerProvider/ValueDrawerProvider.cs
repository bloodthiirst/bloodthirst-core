using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Bloodthirst.Editor.BInspector
{
    [InitializeOnLoad]
    public static class ValueDrawerProvider
    {
        private static List<IValueDrawerValidator> valueDrawerValidators;
        internal static IReadOnlyList<IValueDrawerValidator> ValueDrawerValidators => valueDrawerValidators;

        static ValueDrawerProvider()
        {
            if (!EditorConsts.ON_ASSEMBLY_RELOAD_BINSPECTOR_EDITOR)
                return;

            valueDrawerValidators = new List<IValueDrawerValidator>();

            // valid override type
            IEnumerable<Type> validatorTypes = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => TypeUtils.IsSubTypeOf(t, typeof(IValueDrawerValidator)))
                .Where(t => t != typeof(IValueDrawerValidator))
                .Where(t => t != typeof(ComplexValueValidator));

            foreach (Type t in validatorTypes)
            {
                IValueDrawerValidator i = (IValueDrawerValidator)Activator.CreateInstance(t);
                valueDrawerValidators.Add(i);
            }

            // order the list of validators
            valueDrawerValidators.Sort((a, b) => a.Order.CompareTo(b.Order));

            // valid inspector type
            IEnumerable<Type> drawerTypes = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => TypeUtils.IsSubTypeOf(t, typeof(IValueDrawer)))
                .Where(t => t != typeof(IValueDrawer))
                .Where(t => t != typeof(ComplexValueDrawer));


            // initialize
            foreach (IValueDrawerValidator v in valueDrawerValidators)
            {
                v.Initialize();
            }

        }

        public static IValueDrawer Get(Type type)
        {
            for (int i = 0; i < valueDrawerValidators.Count; i++)
            {
                IValueDrawerValidator validator = valueDrawerValidators[i];

                if (validator.CanDraw(type))
                {
                    return validator.GetValueDrawer();
                }
            }

            return new ComplexValueDrawer();
        }

    }
}
