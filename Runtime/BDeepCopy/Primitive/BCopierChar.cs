using System.Collections.Generic;
using System.Reflection;

namespace Bloodthirst.BDeepCopy
{
    internal class BCopierChar : BCopierPureStructBase<char>
    {
        public override IReadOnlyList<MemberInfo> CopiableMembers()
        {
            return BCopierBase.EmptyMembers;
        }
    }
}
