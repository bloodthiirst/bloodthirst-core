using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    internal static class BDeepCopyFactory
    {
        private static List<IBCopyFactory> CopyFactories { get; }
        private static bool IsInitialized { get; set; } = false;

        private static IBCopyFactory DefaultCopierFactory { get; set; }

        private static HashSet<Type> FactoryTypes { get; set; }

        static BDeepCopyFactory()
        {
            FactoryTypes = new HashSet<Type>();

            // create list
            CopyFactories = new List<IBCopyFactory>()
            {
                // pure value types
                new PureValueTypeCopierFactory(),

                // array
                new ArrayCopierFactory(),
                
                // dictionary
                new DictionaryCopierFactory(),
                
                // list
                new ListWithSealedElementTypeCopierFactory(),
                new ListCopierFactory(),
                
                // type
                new TypeCopierFactory(),

                // interface
                new InterfaceCopierFactory()
            };

            // save types
            foreach (IBCopyFactory f in CopyFactories)
            {
                FactoryTypes.Add(f.GetType());
            }

            DefaultCopierFactory = new DefaultCopierFactory();
            CheckFactories();
        }

        public static void CheckFactories()
        {
            if (IsInitialized)
                return;

            IsInitialized = true;
            Initialize();
        }

        private static void Initialize()
        {

            IEnumerable<Type> factories = Assembly.GetAssembly(typeof(BDeepCopyFactory))
                .GetTypes()
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Where(t => typeof(IBCopyFactory).IsAssignableFrom(t))
                .Where(t => t != typeof(DefaultCopierFactory));

            foreach (Type t in factories)
            {
                if (!FactoryTypes.Contains(t))
                    throw new Exception($"The copier factory of type {t} is missing from the list");
            }
        }

        internal static IBCopierInternal Create(Type t)
        {
            CheckFactories();

            for (int i = 0; i < CopyFactories.Count; i++)
            {
                IBCopyFactory curr = CopyFactories[i];

                if (curr.CanCopy(t))
                    return curr.GetCopier(t);
            }

            return DefaultCopierFactory.GetCopier(t);
        }



    }
}
