using Bloodthirst.Scripts.Core.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bloodthirst.Core.UI
{
    public class UIWindowHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private RectTransform uiWindowTransform = null;

        [SerializeField]
        private bool canMove;

        public void OnPointerDown(PointerEventData eventData)
        {
            canMove = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            canMove = false;
        }

        private void Update()
        {
            if (canMove)
            {
                uiWindowTransform.anchoredPosition += (Vector2)MouseUtils.Instance.MouseDelta;
            }
        }
    }
}
