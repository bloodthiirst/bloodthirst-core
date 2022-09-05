using System;
using System.Runtime.CompilerServices;

namespace Bloodthirst.Scripts.Utils
{
    public static class BitUtils
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static TTo Reinterpret<TFrom, TTo>(TFrom val) where TFrom : unmanaged where TTo : unmanaged
        {
            return *(TTo*)(&val);
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
