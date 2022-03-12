using Bloodthirst.Core.ServiceProvider;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Scripts.Core.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bloodthirst.Core.UI
{
    public class UIWindowHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IAwakePass
    {
        [SerializeField]
        private RectTransform uiWindowTransform = null;

        [SerializeField]
        private bool canMove;

        private MouseUtils _mouseUtils;

        public void OnPointerDown(PointerEventData eventData)
        {
            canMove = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            canMove = false;
        }

        void IAwakePass.Execute()
        {
            Execute();
        }

        private void Execute()
        {
            _mouseUtils = BProviderRuntime.Instance.GetSingleton<MouseUtils>();
        }

        private void Update()
        {
            if (canMove)
            {
                uiWindowTransform.anchoredPosition += (Vector2)_mouseUtils.MouseDelta;
            }
        }
    }
}
