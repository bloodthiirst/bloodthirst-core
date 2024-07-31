using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bloodthirst.Core.UI
{
    public class UIButton : Selectable , IPointerClickHandler
    {
        [SerializeField]
        private ButtonState state;
        public ButtonState State
        {
            get => state;
            set
            {
                if (state == value)
                    return;

                state = value;
                OnStateChanged?.Invoke(this);
            }
        }

        [SerializeField]
        private ClickEvent clickEvent;
        public ClickEvent ClickEvent => clickEvent;

        public event Action<UIButton> OnStateChanged;

        public void OnPointerClick(PointerEventData eventData)
        {
            if(state != ButtonState.Enabled)
            {
                return;
            }

            clickEvent.Invoke(this , state);
        }
    }
}
