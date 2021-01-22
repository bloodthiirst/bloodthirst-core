using System;
using System.Linq.Expressions;

namespace JsonDB
{
    public static class Utils
    {
        public static Func<TInterface> CreateDefaultConstructor<TInterface>(Type typeConcrete)
        {
            var body = Expression.New(typeConcrete);
            var lambda = Expression.Lambda<Func<TInterface>>(body);

            return lambda.Compile();
        }
    }
}
