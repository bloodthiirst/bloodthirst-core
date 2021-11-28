using Bloodthirst.Core.Utils;
using Bloodthirst.JsonUnityObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bloodthirst.Editor.BInspector
{
    [InitializeOnLoad]
    public static class ValueDrawerProvider
    {
        private static List<IValueDrawerValidator> valueDrawerValidators;
        internal static IReadOnlyList<IValueDrawerValidator> ValueDrawerValidators => valueDrawerValidators;

        private static List<IValueDrawer> valueDrawers;
        internal static IReadOnlyList<IValueDrawer> ValueDrawers => valueDrawers;

        static ValueDrawerProvider()
        {
            valueDrawerValidators = new List<IValueDrawerValidator>();
            valueDrawers = new List<IValueDrawer>();

            // valid override type
            IEnumerable<Type> validatorTypes = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => TypeUtils.IsSubTypeOf(t, typeof(IValueDrawerValidator)))
                .Where(t => t != typeof(IValueDrawerValidator));

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
                .Where(t => t != typeof(IValueDrawer));

            foreach (Type t in drawerTypes)
            {
                IValueDrawer drawer = (IValueDrawer)Activator.CreateInstance(t);
                valueDrawers.Add(drawer);
            }

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

            return null;
        }

    }
}
