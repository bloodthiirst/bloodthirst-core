using System;
using System.Collections.Generic;
using System.Reflection;

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

        object IBCopier.Copy(object t , BCopierSettings bCopierSettings)
        {
            return Copier.Copy(t , bCopierSettings);
        }

        T IBCopier<T>.Copy(T t, BCopierSettings bCopierSettings = null)
        {
            return Copy(t , bCopierSettings);
        }
        
        public T Copy(T t , BCopierSettings bCopierSettings = null)
        {
            return (T)Copier.Copy(t , bCopierSettings);
        }

        public IReadOnlyList<MemberInfo> CopiableMembers()
        {
            return Copier.CopiableMembers();
        }
    }
}
