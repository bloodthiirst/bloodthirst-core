using UnityEngine;
using UnityEngine.EventSystems;

namespace Bloodthirst.UI
{
    /// <summary>
    /// A utility component used to auto-close windows after a certain period of time without interaction
    /// </summary>
    public class WindowAutocloser : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        [Range(0 , 30)]
        private float _closingTimerInSeconds;

        [SerializeField]
        private bool _isInteracting;

        [SerializeField]
        private float _currentInteractionTimer;

        private IWindowUI _window;

        private void Awake()
        {
            _window = GetComponent<IWindowUI>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isInteracting = true;

            _currentInteractionTimer = 0;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isInteracting = false;

            _currentInteractionTimer = 0;
        }

        private void Update()
        {
            if (_window.State != WindowState.Open)
                return;

            if (_isInteracting)
            {
                return;
            }

            _currentInteractionTimer += Time.deltaTime;

            if(_currentInteractionTimer >= _closingTimerInSeconds)
            {
                _currentInteractionTimer = 0;
                _window.Close();
            }
        }
    }
}
