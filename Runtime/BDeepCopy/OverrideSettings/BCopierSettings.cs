using System;
using System.Collections.Generic;

namespace Bloodthirst.BDeepCopy
{
    public class BCopierSettings
    {
        private static List<IBCopierOverrideInternal> CopierOverrideInternals { get; set; } = new List<IBCopierOverrideInternal>()
        {
            new KeepReferenceOverride(),
            new IgnoreMemberOverride()
        };

        private static Dictionary<Type, IBCopierOverrideInternal> typeToCopier = new Dictionary<Type, IBCopierOverrideInternal>();
        internal static IReadOnlyDictionary<Type, IBCopierOverrideInternal> TypeToCopier => typeToCopier;

        static BCopierSettings()
        {
            foreach (IBCopierOverrideInternal c in CopierOverrideInternals)
            {
                typeToCopier.Add(c.AttributeType, c);
            }
        }

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
