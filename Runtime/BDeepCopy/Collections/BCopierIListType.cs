using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    public class BCopierIListType : BCopierBase
    {
        private const string CAPACITY_PARAMETER_NAME = "capacity";

        private Type Type { get; set; }

        internal override Type CopierType()
        {
            return Type;
        }

        private Func<int, object> Constructor { get; set; }

        private Type ListType { get; set; }
        private Type ElementType { get; set; }

        private Dictionary<Type, IBCopierInternal> PotentialCopiers { get; set; }

        internal BCopierIListType(Type t)
        {
            Type = t;

            BDeepCopyProvider.Register(this);

            Initialize();
        }

        protected override void Initialize()
        {
            base.Initialize();

            PotentialCopiers = new Dictionary<Type, IBCopierInternal>();

            ListType = Type.GetGenericTypeDefinition();

            ElementType = Type.GenericTypeArguments[0];

            Constructor = GetConstructor(Type);

            // TODO : make separete copier for non-inheritable classes
            //ElementsAreConcrete = ElementType.IsAbstract || ElementType.IsInterface;
        }

        internal override object CreateEmptyCopy(object original)
        {
            return Constructor(((IList)original).Count);
        }

        internal override object Copy(object t,object instance, BCopierContext copierContext , BCopierSettings copierSettings)
        {
            // TODO : optimize by looking for contructor with capacity first
            // DONE
            IList casted = (IList)t;
            IList castedInstance = (IList)instance;

            // cached locals
            object curr = null;
            Type currType = null;
            IBCopierInternal c = null;
            object copy = null;

            for (int i = 0; i < casted.Count; i++)
            {
                curr = casted[i];
                currType = curr.GetType();

                c = GetElementCopier(currType);

                copy = c.Copy(curr, copierContext , copierSettings);

                castedInstance.Add(copy);
            }

            return castedInstance;
        }

        private IBCopierInternal GetElementCopier(Type currType)
        {
            // check the copiers local cache
            if (!PotentialCopiers.TryGetValue(currType, out IBCopierInternal copier))
            {
                copier = BDeepCopyProvider.GetOrCreate(currType);
                PotentialCopiers.Add(currType, copier);
            }

            return copier;
        }

        internal Func<int, object> GetConstructor(Type type)
        {
            // look for constructor with a "capacity" parameter
            ConstructorInfo ctorWithCapacity = type
                .GetConstructors()
                .FirstOrDefault
                (
                    c =>
                    {
                        ParameterInfo[] ctorParams = c.GetParameters();
                        return ctorParams.Length == 1 &&
                                ctorParams[0].ParameterType == typeof(int) && ctorParams[0].Name.Equals(CAPACITY_PARAMETER_NAME, StringComparison.OrdinalIgnoreCase);
                    }
                );

            Expression<Func<int, object>> result = null;

            ParameterExpression capacityParam = Expression.Parameter(typeof(int), CAPACITY_PARAMETER_NAME);

            if (ctorWithCapacity != null)
            {
                result = Expression.Lambda<Func<int, object>>
                        (
                          Expression.New(ctorWithCapacity, capacityParam), capacityParam
                        );
            }
            else
            {
                result = Expression.Lambda<Func<int, object>>
                        (
                          Expression.New(type), capacityParam
                        );
            }

            return result.Compile();
        }
    }
}
