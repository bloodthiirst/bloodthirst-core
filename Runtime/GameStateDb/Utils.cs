using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

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
