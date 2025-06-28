using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bloodthirst.Core.UI
{
    public interface IUIEventFilter
    {
        bool ShouldProcess(BaseEventData eventData);
    }

    public class UIEventSystemFilter : MonoBehaviour, IUIEventFilter
    {
        public List<EventSystem> blockedEventSystems = new List<EventSystem>();

        bool IUIEventFilter.ShouldProcess(BaseEventData eventData)
        {
            BaseInputModule inputModule = eventData.currentInputModule;

            if(inputModule == null)
            {
                return true;
            }

            if (!inputModule.TryGetComponent(out EventSystem evt))
            {
                eventData.Use();
                return false;
            }

            if (blockedEventSystems.Contains(evt))
            {
                eventData.Use();
                return false;
            }

            return true;
        }
    }
}
