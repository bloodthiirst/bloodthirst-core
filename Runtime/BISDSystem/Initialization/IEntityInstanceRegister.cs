using System;

namespace Bloodthirst.Core.BISDSystem
{
    public interface IEntityInstanceRegister
    {
        event Action<object> OnRegistered;

        event Action<object> OnUnregistered;

        void Register<T>(T instance) where T : class;

        void Unregister<T>(T instance) where T : class;
    }
}
