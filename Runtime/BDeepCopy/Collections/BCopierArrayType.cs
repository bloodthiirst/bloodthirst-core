using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    public class BCopierArrayType : BCopierBase
    {
        private const string LENGTH_PARAMETER_NAME = "Length";

        private Type Type { get; set; }

        internal override Type CopierType()
        {
            return Type;
        }

        private Func<int, object> Constructor { get; set; }

        private Type ListType { get; set; }
        private Type ElementType { get; set; }

        private Dictionary<Type, IBCopierInternal> PotentialCopiers { get; set; }

        internal BCopierArrayType(Type t)
        {
            Type = t;

            BDeepCopyProvider.Register(this);

            Initialize();
        }

        private void Initialize()
        {
            PotentialCopiers = new Dictionary<Type, IBCopierInternal>();

            ElementType = Type.GetElementType();

            Constructor = GetConstructor(Type);

            // TODO : make separete copier for non-inheritable classes
            //ElementsAreConcrete = ElementType.IsAbstract || ElementType.IsInterface;
        }

        internal override object CreateEmptyCopy(object original)
        {
            return Constructor(((Array)original).Length);
        }

        internal override object Copy(object t, object instance, BCopierContext copierContext, BCopierSettings copierSettings)
        {
            Array casted = (Array)t;
            Array castedInstance = (Array)instance;

            // cached locals
            object curr = null;
            Type currType = null;
            IBCopierInternal c = null;
            object copy = null;

            for (int i = 0; i < casted.Length; i++)
            {
                curr = casted.GetValue(i);
                currType = curr.GetType();

                c = GetElementCopier(currType);

                copy = c.Copy(curr, copierContext, copierSettings);

                castedInstance.SetValue(copy,i);
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
            Expression<Func<int, object>> result = null;

            ParameterExpression lengthParam = Expression.Parameter(typeof(int), LENGTH_PARAMETER_NAME);

            NewArrayExpression arrayInit = Expression.NewArrayBounds(ElementType , lengthParam);
            result = Expression.Lambda<Func<int, object>>
                    (
                      arrayInit , lengthParam
                    );

            return result.Compile();
        }
    }
}
