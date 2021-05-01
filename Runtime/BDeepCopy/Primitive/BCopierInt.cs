using System.Collections.Generic;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    internal class BCopierInt : BCopierPureStructBase<int>
    {
        public override IReadOnlyList<MemberInfo> CopiableMembers()
        {
            return BCopierBase.EmptyMembers;
        }
    }
}
