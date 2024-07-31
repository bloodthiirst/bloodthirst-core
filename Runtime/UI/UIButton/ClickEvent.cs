using System;
using UnityEngine.Events;

namespace Bloodthirst.Core.UI
{
    [Serializable]
    public class ClickEvent : UnityEvent<UIButton , ButtonState> { }
}
