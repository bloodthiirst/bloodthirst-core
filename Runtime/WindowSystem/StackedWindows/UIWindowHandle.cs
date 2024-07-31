using Bloodthirst.Core.BProvider;
using Bloodthirst.Core.SceneManager;
using Bloodthirst.Scripts.Core.GamePassInitiator;
using Bloodthirst.Scripts.Core.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bloodthirst.Core.UI
{
    public class UIWindowHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler , IOnSceneLoaded , IOnSceneUnload
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
        public void OnLoaded(ISceneInstanceManager sceneInstance)
        {
            _mouseUtils = BProviderRuntime.Instance.GetSingleton<MouseUtils>();
        }

        public void OnUnload(ISceneInstanceManager sceneInstance)
        {
            _mouseUtils = null;
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
