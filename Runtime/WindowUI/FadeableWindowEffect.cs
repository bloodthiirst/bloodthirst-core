using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Bloodthirst.UI
{
    /// <summary>
    /// Window effect for a slight window opening/closing animation , toggles the UI with a slight fade in/out
    /// </summary>
    public class FadeableWindowEffect : MonoBehaviour, IWindowUIEffect
    {
        [SerializeField]
        private float fadeDuration;
        
        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private CanvasGroup canvasGroup;
        
        [SerializeField]
        private GraphicRaycaster graphicRaycaster;

        public IEnumerator OpenCrt(IWindowUI window)
        {
            graphicRaycaster.enabled = true;
            canvasGroup.interactable = true;
            canvas.enabled = true;

            float t = 0;
            
            while( t < 1)
            {
                t += Time.deltaTime / fadeDuration;
                t = Mathf.Clamp01(t);
                canvasGroup.alpha = t;

                yield return null;
            }
        }

        public IEnumerator CloseCrt(IWindowUI window)
        {
            graphicRaycaster.enabled = false;
            canvasGroup.interactable = false;

            float t = 0;

            while (t < 1)
            {
                t += Time.deltaTime / fadeDuration;
                t = Mathf.Clamp01(t);
                canvasGroup.alpha = 1 - t;

                yield return null;
            }

            canvas.enabled = false;
        }

        public void CloseImmidiate(IWindowUI window)
        {
            canvas.enabled = false;
            canvasGroup.interactable = false;
            graphicRaycaster.enabled = false;
            canvasGroup.alpha = 0;
        }

        public void OpenImmidiate(IWindowUI window)
        {
            canvas.enabled = true;
            canvasGroup.interactable = true;
            graphicRaycaster.enabled = true;
            canvasGroup.alpha = 1;
        }
    }
}
