using Bloodthirst.Scripts.Core.GamePassInitiator;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bloodthirst.Core.UI
{
    public abstract class UIWindow : MonoBehaviour, IUIWindow, IPointerDownHandler, IAwakePass
    {
        [SerializeField]
        protected RectTransform parentTransform;
        public IWindowLayer Manager => GetManager();
        public RectTransform ParentTransform => parentTransform;

        [ShowInInspector]
        public abstract bool IsFocused { get; set; }
        [ShowInInspector]
        public bool IsOpen { get; set; }

        public virtual bool RequestOpen { get; set; }

        [ShowInInspector]
        public virtual bool RequestFocus { get; set; }

        private bool requestUnfocus;

        [ShowInInspector]
        public virtual bool RequestUnfocus
        {
            get => requestUnfocus;
            set
            {
                requestUnfocus = value;
            }
        }
        public virtual bool RequestClose { get; set; }

        public event Action<IUIWindow> OnClose;

        public event Action<IUIWindow> OnFocus;

        public event Action<IUIWindow> OnUnfocus;

        public event Action<IUIWindow> OnOpen;

        public abstract IWindowLayer GetManager();
        public abstract IEnumerator Open();
        public abstract IEnumerator Close();
        public abstract IEnumerator Focus();
        public abstract IEnumerator Unfocus();
        private void Execute()
        {
            IsOpen = false;
            IsFocused = false;
            Manager.Add(this);
            StartCoroutine(Close());
        }

        void IGamePass.Execute()
        {
            Execute();
        }

        void IAwakePass.Execute()
        {
            Execute();
        }

        public void OnDestroy()
        {
            IsOpen = false;
            IsFocused = false;
            Manager.Remove(this);
        }

        public void TriggerUnfocus()
        {
            RequestUnfocus = true;
            OnUnfocus?.Invoke(this);
        }

        public void TriggerFocus()
        {
            RequestFocus = true;
            OnFocus?.Invoke(this);
        }


        public void TriggerOpen()
        {
            if (!IsOpen)
            {
                RequestOpen = true;
                OnOpen?.Invoke(this);
            }
            else
            {
                RequestFocus = true;
                OnFocus?.Invoke(this);
            }
        }
        public void TriggerClose()
        {
            RequestClose = true;
            OnClose?.Invoke(this);
        }

#if UNITY_EDITOR
        [Button]
        private void TriggerOpenEditor()
        {
            TriggerOpen();
        }

        [Button]
        private void TriggerCloseEditor()
        {
            TriggerClose();
        }
#endif

        public void OnPointerDown(PointerEventData eventData)
        {
            RequestFocus = true;
            OnFocus?.Invoke(this);
        }

        public abstract void BeforeOpen();
        public abstract void BeforeClose();
    }
}
