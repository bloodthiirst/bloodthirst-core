using System;

namespace Bloodthirst.Core.Singleton
{
    public interface IBSingleton
    {
        Type Concrete { get;}
        Type Interface { get;}
        void OnSetupSingleton();
    }


}
