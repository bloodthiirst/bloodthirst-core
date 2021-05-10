using Bloodthirst.Core.Utils;
using System;
using System.Collections;

namespace Bloodthirst.BDeepCopy
{
    internal class ListWithSealedElementTypeCopierFactory : IBCopyFactory
    {
        bool IBCopyFactory.CanCopy(Type t)
        {
            if (!TypeUtils.IsSubTypeOf(t , typeof(IList)))
                return false;

            Type elementType = t.GenericTypeArguments[0];

            return !TypeUtils.HasSubClass(elementType);
        }

        IBCopierInternal IBCopyFactory.GetCopier(Type t)
        {
            return new BCopierIListTypeSealedElementType(t);
        }
    }
}
