using System;

namespace Bloodthirst.BDeepCopy
{
    public class BDeepCopy
    {
        public static void Preregister<T>()
        {
            GetCopier<T>();
        }

        public static void Preregister(Type t)
        {
            GetCopier(t);
        }

        public static BCopier<T> GetCopier<T>()
        {
            return BCopier<T>.Instance;
        }

        public static IBCopier GetCopier(Type t)
        {
            return BDeepCopyProvider.GetOrCreate(t.GetType());
        }

        public static T Copy<T>(T t)
        {
            return BCopier<T>.Instance.Copy(t);
        }

        public static object Copy(object t)
        {
            return CopyInternal(t);
        }

        private static object CopyInternal(object t)
        {
            return GetCopier(t.GetType()).Copy(t);
        }


    }
}
