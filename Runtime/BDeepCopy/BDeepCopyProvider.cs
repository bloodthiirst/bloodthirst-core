using Bloodthirst.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Bloodthirst.BDeepCopy
{
    internal static class BDeepCopyProvider
    {
        private static Dictionary<Type, IBCopierInternal> Copiers { get; }

        static BDeepCopyProvider()
        {
            Copiers = new Dictionary<Type, IBCopierInternal>();

            // pre-add the primitive types copiers
            for(int i = 0; i < TypeUtils.PrimitiveTypes.Length;i++)
            {
                Type curr = TypeUtils.PrimitiveTypes[i];
                UncheckedRegister(new BCopierPureValueType(curr));
            }

            UncheckedRegister(new BCopierPureValueType(typeof(string)));
        }



        internal static IBCopierInternal GetOrCreate(Type t)
        {
            if (!Copiers.TryGetValue(t, out IBCopierInternal c))
            {
                c = CreateCopier(t);
                return c;
            }

            return c;
        }

        internal static IBCopierInternal CreateCopier(Type t)
        {
            return BDeepCopyFactory.Create(t);

        }

        internal static void Register(IBCopierInternal copier)
        {
            if (!Copiers.ContainsKey(copier.Type))
            {
                UncheckedRegister(copier);
            }
        }
        private static void UncheckedRegister(IBCopierInternal copier)
        {
            Copiers.Add(copier.Type, copier);
        }

    }
}
