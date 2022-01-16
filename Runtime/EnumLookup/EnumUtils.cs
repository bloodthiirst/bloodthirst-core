using System;
using System.Collections.Generic;

namespace Bloodthirst.Core.EnumLookup
{
    public static class EnumUtils<TEnum> where TEnum : Enum
    {
        private static readonly Type type = typeof(TEnum);

        public static readonly int EnumCount = enumValues.Length;
        
        private static readonly TEnum[] enumValues = (TEnum[]) Enum.GetValues(type);
        
        public static IReadOnlyList<TEnum> EnumValues => enumValues;

        public static int GetMask(TEnum val)
        {
            return 1 << Array.IndexOf(enumValues, val);
        }

        public static int GetIndex(TEnum val)
        {
            return Array.IndexOf(enumValues, val);
        }

        public static TEnum GetValue(int index)
        {
            return enumValues[index];
        }
    }
}
