using System;
using System.Runtime.CompilerServices;

namespace Bloodthirst.Scripts.Utils
{
    public static class BitUtils
    {
        public static int BoolToInt(bool val)
        {
            //basically "return *(Int*)&val;" but i guess "safer" ?
            int asInt = Unsafe.As<bool, int>(ref val);

            return asInt;
        }

        public static int SetBitTo1(this int value, int position)
        {
            // Set a bit at position to 1.
            return value |= (1 << position);
        }

        public static int SetBitTo0(this int value, int position)
        {
            // Set a bit at position to 0.
            return value & ~(1 << position);
        }

        public static bool IsBitSetTo1(this int value, int position)
        {
            // Return whether bit at position is set to 1.
            return (value & (1 << position)) != 0;
        }

        public static bool IsBitSetTo0(this int value, int position)
        {
            // If not 1, bit is 0.
            return !IsBitSetTo1(value, position);
        }
    }
}
