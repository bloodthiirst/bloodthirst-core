using System.Collections.Generic;

namespace Bloodthirst.BDeepCopy
{
    public class BCopierSettings
    {
        internal List<IBCopierOverride> CopierOverrides { get; set; } = new List<IBCopierOverride>();

        public void Add(IBCopierOverride copierOverride)
        {
            CopierOverrides.Add(copierOverride);
        }
        public void Remove(IBCopierOverride copierOverride)
        {
            CopierOverrides.Remove(copierOverride);
        }
    }
}
