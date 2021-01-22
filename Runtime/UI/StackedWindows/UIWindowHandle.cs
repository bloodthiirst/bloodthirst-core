using UnityEngine;
using UnityEngine.EventSystems;

namespace Bloodthirst.Core.UI
{
    public class UIWindowHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private RectTransform uiWindowTransform = null;

        private bool canMove;

        [SerializeField]
        private Vector3 lastMousePos;

        public void OnPointerDown(PointerEventData eventData)
        {
            canMove = true;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            canMove = false;
        }

        private void Start()
        {
            lastMousePos = Input.mousePosition;
        }

        private void Update()
        {
            Vector2 delta = Input.mousePosition - lastMousePos;

            if (canMove)
            {
                uiWindowTransform.anchoredPosition += delta;
            }

            lastMousePos = Input.mousePosition;

        }


    }
}
