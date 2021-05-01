using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    public class BCopierIDictionaryType : BCopierBase
    {
        private const string CAPACITY_PARAMETER_NAME = "capacity";
        private Type Type { get; set; }

        internal override Type CopierType()
        {
            return Type;
        }

        private Func<int , object> Constructor { get; set; }

        private Type ListType { get; set; }
        private Type KeyType { get; set; }
        private Type ValueType { get; set; }

        private Dictionary<Type, IBCopierInternal> PotentialKeyCopiers { get; set; }
        private Dictionary<Type, IBCopierInternal> PotentialValueCopiers { get; set; }

        internal BCopierIDictionaryType(Type t)
        {
            Type = t;

            BDeepCopyProvider.Register(this);

            Initialize();
        }

        private void Initialize()
        {
            PotentialKeyCopiers = new Dictionary<Type, IBCopierInternal>();
            
            PotentialValueCopiers = new Dictionary<Type, IBCopierInternal>();

            ListType = Type.GetGenericTypeDefinition();

            KeyType = Type.GenericTypeArguments[0];
            ValueType = Type.GenericTypeArguments[1];

            Constructor = GetConstructor(Type);

            // TODO : make separete copier for non-inheritable classes
            //ElementsAreConcrete = ElementType.IsAbstract || ElementType.IsInterface;
        }

        internal override object CreateEmptyCopy(object original)
        {
            return Constructor(((IDictionary)original).Count);
        }

        internal override object Copy(object t , object instance , BCopierContext copierContext , BCopierSettings copierSettings)
        {
            // TODO : optimize by looking for contructor with capacity first
            // DONE
            IDictionary casted = (IDictionary)t;
            IDictionary castedInstance = (IDictionary)instance;

            object[] keys = new object[casted.Count];
            object[] vals = new object[casted.Count];

            casted.Keys.CopyTo(keys, 0);
            casted.Values.CopyTo(vals, 0);

            // cached locals
            object curr = null;
            IBCopierInternal c = null;
            Type currType = null;
            object copyKey = null;
            object copyVal = null;

            for (int i = 0; i < casted.Count; i++)
            {
                // copy key
                curr = keys[i];
                currType = curr.GetType();

                c = GetKeyCopier(currType);

                copyKey = c.Copy(curr , copierContext , copierSettings);

                // copy key
                curr = vals[i];
                currType = curr.GetType();

                c = GetValueCopier(currType);

                copyVal = c.Copy(curr , copierContext , copierSettings);

                castedInstance.Add(copyKey , copyVal);
            }

            return castedInstance;
        }

        private IBCopierInternal GetValueCopier(Type currType)
        {
            // check the copiers local cache
            if (!PotentialValueCopiers.TryGetValue(currType, out IBCopierInternal copier))
            {
                copier = BDeepCopyProvider.GetOrCreate(currType);
                PotentialValueCopiers.Add(currType, copier);
            }

            return copier;
        } 
        
        private IBCopierInternal GetKeyCopier(Type currType)
        {
            // check the copiers local cache
            if (!PotentialKeyCopiers.TryGetValue(currType, out IBCopierInternal copier))
            {
                copier = BDeepCopyProvider.GetOrCreate(currType);
                PotentialKeyCopiers.Add(currType, copier);
            }

            return copier;
        }

        internal Func<int ,object> GetConstructor(Type type)
        {
            // look for constructor with a "capacity" parameter
            ConstructorInfo ctorWithCapacity = type
                .GetConstructors()
                .FirstOrDefault
                (
                    c =>
                    {
                        ParameterInfo[] ctorParams = c.GetParameters();
                        return  ctorParams.Length == 1 &&
                                ctorParams[0].ParameterType == typeof(int) && ctorParams[0].Name.Equals(CAPACITY_PARAMETER_NAME , StringComparison.OrdinalIgnoreCase);
                    }
                );

            Expression<Func<int,object>> result = null;

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
                result = Expression.Lambda<Func<int,object>>
                        (
                          Expression.New(type) , capacityParam
                        );
            }

            return result.Compile();
        }

    }
}
