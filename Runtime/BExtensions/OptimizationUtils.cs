using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bloodthirst.Scripts.Utils
{
    public static class OptimizationUtils
    {
        private static Func<IntPtr,Type,Delegate> GetDelegateForFunctionPointerInternal;
        private static Func<Delegate,IntPtr> GetFunctionPointerForDelegateInternal;

        public static byte[] CopyByteArray(byte[] source, int offset, int size)
        {
            byte[] copy = new byte[size];

            unsafe
            {
                fixed (byte* ptr = &copy[0])
                {
                    Marshal.Copy(source, offset, (IntPtr)ptr, size);
                }
            }
            return copy;
        }

        static OptimizationUtils()
        {
            MethodInfo delegateToFuncPtr = typeof(Marshal).GetMethod("GetDelegateForFunctionPointerInternal", BindingFlags.NonPublic | BindingFlags.Static);
            GetDelegateForFunctionPointerInternal = (Func<IntPtr, Type,Delegate>)delegateToFuncPtr.CreateDelegate(typeof(Func<IntPtr, Type, Delegate>));

            MethodInfo funcPtrToDelegate = typeof(Marshal).GetMethod("GetFunctionPointerForDelegateInternal", BindingFlags.NonPublic | BindingFlags.Static);
            GetFunctionPointerForDelegateInternal = (Func<Delegate,IntPtr>)funcPtrToDelegate.CreateDelegate(typeof(Func<Delegate,IntPtr>));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static TResult LookupTable<TResult>(TResult ifTrue, TResult ifFalse , bool value) where TResult : unmanaged
        {
            TResult* vals = stackalloc TResult[2];

            vals[0] = ifFalse;
            vals[1] = ifTrue;

            return vals[Reinterpret<bool, int>(value)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static Delegate LookupTableDelegate(Delegate ifFalse, Delegate ifTrue, Type delegateType, bool value)
        {
            IntPtr ptrIfFalse = GetFunctionPointerForDelegateInternal(ifFalse);
            IntPtr ptrIfTrue = GetFunctionPointerForDelegateInternal(ifTrue);

            IntPtr* vals = stackalloc IntPtr[2];

            vals[0] = ptrIfFalse;
            vals[1] = ptrIfTrue;

            IntPtr callback = vals[Reinterpret<bool, int>(value)];

            Delegate asDelegate = GetDelegateForFunctionPointerInternal(callback, delegateType);

            return asDelegate;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static TDelegate LookupTableDelegate<TDelegate>(TDelegate ifFalse, TDelegate ifTrue, bool value) where TDelegate : Delegate
        {
            Type delType = typeof(TDelegate);

            return (TDelegate) LookupTableDelegate(ifFalse , ifTrue , delType , value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static TTo Reinterpret<TFrom, TTo>(TFrom val) where TFrom : unmanaged where TTo : unmanaged
        {
            return *(TTo*)(&val);
        }
    }
}
