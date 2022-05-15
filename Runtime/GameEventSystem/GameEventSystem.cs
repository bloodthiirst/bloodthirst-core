using System;

namespace Bloodthirst.Core.GameEventSystem
{
    public class GameEventSystem<T> where T : Enum
    {
        private static Type enumType = typeof(T);

        public void Initialize()
        {

        }
    }
}
