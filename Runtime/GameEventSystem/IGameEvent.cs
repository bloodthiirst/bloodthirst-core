using System;

namespace Bloodthirst.Core.GameEventSystem
{
    public interface IGameEvent { }
    public interface IGameEvent<TEvent, TEnum> : IGameEvent where TEnum : Enum where TEvent : IGameEvent<TEvent, TEnum>
    {
        static TEnum EventID;
        TEnum GetEventID();
    }
}
