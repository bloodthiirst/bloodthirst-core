using System;

namespace Bloodthirst.BDeepCopy
{
    public class BCopier<T> : IBCopier<T>, IBCopier
    {
        private static readonly Type t = typeof(T);

        private static BCopier<T> instance;
        
        internal static BCopier<T> Instance
        {   
            get
            {
                if (instance == null)
                {
                    instance = new BCopier<T>(BDeepCopyProvider.GetOrCreate(t));
                }

                return instance;
            }
        }

        Type IBCopier.Type => t;

        internal IBCopier Copier { get; }

        private BCopier(IBCopier copier)
        {
            Copier = copier;
        }

        object IBCopier.Copy(object t)
        {
            return Copier.Copy(t);
        }

        T IBCopier<T>.Copy(T t)
        {
            return Copy(t);
        }

        public T Copy(T t)
        {
            return (T)Copier.Copy(t);
        }
    }
}
