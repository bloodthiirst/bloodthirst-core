using System;
using System.Collections;

namespace Bloodthirst.BDeepCopy
{
    internal class DictionaryCopierFactory : IBCopyFactory
    {
        bool IBCopyFactory.CanCopy(Type t)
        {
            return typeof(IDictionary).IsAssignableFrom(t);
        }

        IBCopierInternal IBCopyFactory.GetCopier(Type t)
        {
            return new BCopierIDictionaryType(t);
        }
    }
}
