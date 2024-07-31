using System.Collections;
using UnityEngine;

namespace Bloodthirst.UI
{
    /// <summary>
    /// Window effect for default opening closing , just toggles the canvas , alpha and the raycasting of the UI
    /// </summary>
    public class DefaultWindowEffect : MonoBehaviour, IWindowUIEffect
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        
        [SerializeField]
        private Canvas _canvas;

        public IEnumerator OpenCrt(IWindowUI window)
        {
            OpenImmidiate(window);
            yield break;
        }

        public IEnumerator CloseCrt(IWindowUI window)
        {
            CloseImmidiate(window);
            yield break;
        }

        public void OpenImmidiate(IWindowUI window)
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

            _canvas.enabled = true;
        }

        public void CloseImmidiate(IWindowUI window)
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;

            _canvas.enabled = false;
        }
    }
}
