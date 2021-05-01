using System.Collections.Generic;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    internal class BCopierFloat : BCopierPureStructBase<float>
    {
        public override IReadOnlyList<MemberInfo> CopiableMembers()
        {
            return BCopierBase.EmptyMembers;
        }
    }
}
