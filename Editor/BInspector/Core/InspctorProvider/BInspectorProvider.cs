using Bloodthirst.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Bloodthirst.Editor.BInspector
{
    [InitializeOnLoad]
    public static class BInspectorProvider
    {
        private static List<IBInspectorValidator> inspectorValidators;
        internal static IReadOnlyList<IBInspectorValidator> InspectorValidators => inspectorValidators;

        private static List<IBInspectorDrawer> inspectorDrawers;
        internal static IReadOnlyList<IBInspectorDrawer> InspectorDrawers => inspectorDrawers;

        private static BInspectorDefault defaultInspector;
        public static BInspectorDefault DefaultInspector => defaultInspector;

        static BInspectorProvider()
        {
            inspectorValidators = new List<IBInspectorValidator>();
            inspectorDrawers = new List<IBInspectorDrawer>();

            // valid override type
            IEnumerable<Type> validatorTypes = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => TypeUtils.IsSubTypeOf(t, typeof(IBInspectorValidator)))
                .Where(t => t != typeof(IBInspectorValidator))
                .Where(t => t != typeof(BInspectorDefault));

            foreach (Type t in validatorTypes)
            {
                IBInspectorValidator i = (IBInspectorValidator)Activator.CreateInstance(t);
                inspectorValidators.Add(i);
            }

            // order the list of validators
            inspectorValidators.Sort((a, b) => a.Order.CompareTo(b.Order));

            // valid inspector type
            IEnumerable<Type> drawerTypes = TypeUtils.AllTypes
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => TypeUtils.IsSubTypeOf(t, typeof(IBInspectorDrawer)))
                .Where(t => t != typeof(IBInspectorDrawer))
                .Where(t => t != typeof(BInspectorDefault));

            foreach (Type t in drawerTypes)
            {
                IBInspectorDrawer drawer = (IBInspectorDrawer)Activator.CreateInstance(t);
                inspectorDrawers.Add(drawer);
            }

            // default drawer
            defaultInspector = new BInspectorDefault();

            // initialize
            foreach (IBInspectorValidator v in inspectorValidators)
            {
                v.Initialize();
            }

            foreach (IBInspectorDrawer d in inspectorDrawers)
            {
                d.Initialize();
            }

            (defaultInspector as IBInspectorValidator).Initialize();
            (defaultInspector as IBInspectorDrawer).Initialize();
        }

        public static IBInspectorDrawer Get(object instance)
        {
            for (int i = 0; i < inspectorValidators.Count; i++)
            {
                IBInspectorValidator validator = inspectorValidators[i];

                Type instanceType = instance.GetType();

                if (validator.CanInspect(instanceType, instance))
                {
                    return validator.GetDrawer();
                }
            }

            return null;
        }

    }
}
