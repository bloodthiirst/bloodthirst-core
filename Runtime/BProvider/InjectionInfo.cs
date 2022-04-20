using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bloodthirst.Core.BProvider
{
    internal class InjectionInfo
    {
        internal Type MainType { get; set; }

        public Dictionary<MemberInfo, IInjectable> Injectables { get; set; }
    }
}