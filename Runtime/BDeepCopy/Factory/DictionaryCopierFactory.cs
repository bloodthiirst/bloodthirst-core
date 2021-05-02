using Bloodthirst.Core.Utils;
using System;
using System.Collections;

namespace Bloodthirst.BDeepCopy
{
    internal class DictionaryCopierFactory : IBCopyFactory
    {
        bool IBCopyFactory.CanCopy(Type t)
        {
            return TypeUtils.IsSubTypeOf(t, typeof(IDictionary));
        }

        IBCopierInternal IBCopyFactory.GetCopier(Type t)
        {
            return new BCopierIDictionaryType(t);
        }
    }
}
