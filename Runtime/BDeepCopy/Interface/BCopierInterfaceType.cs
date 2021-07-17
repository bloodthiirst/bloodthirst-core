using Bloodthirst.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    public class BCopierInterfaceType : IBCopierInternal
    {
        private Type Type { get; set; }

        private Dictionary<Type, IBCopierInternal> PotentialTypeCopiers { get; set; }

        Type IBCopier.Type => Type;

        internal BCopierInterfaceType(Type t)
        {
            Type = t;

            BDeepCopyProvider.Register(this);

            Initialize();
        }

        private void Initialize()
        {
            PotentialTypeCopiers = new Dictionary<Type, IBCopierInternal>();

            // TODO : cache the copiers for non generic types that inherit the interface

            IEnumerable<Type> types = TypeUtils.AllTypes
                .Where(t => !t.IsAbstract)
                .Where(t => !t.IsInterface)
                .Where(t => !t.IsGenericType)
                .Where(t => TypeUtils.IsSubTypeOf(t, Type));

            foreach(Type t in types)
            {
                PotentialTypeCopiers.Add(t, BDeepCopyProvider.CreateCopier(t));
            }
        }

        object IBCopierInternal.Copy(object t, BCopierContext copierContext, BCopierSettings bCopierSettings)
        {
            if (t == null)
                return null;

            Type concrete = t.GetType();

            // check the copiers local cache
            if (!PotentialTypeCopiers.TryGetValue(concrete, out IBCopierInternal copier))
            {
                copier = BDeepCopyProvider.GetOrCreate(concrete);
                PotentialTypeCopiers.Add(concrete, copier);
            }

            return copier.Copy(t, copierContext, bCopierSettings);
        }

        object IBCopier.Copy(object t, BCopierSettings bCopierSettings)
        {
            return ((IBCopierInternal)this).Copy(t, new BCopierContext() , bCopierSettings);
        }

        object IBCopierInternal.GetDefaultValue()
        {
            return null;
        }
    }
}
