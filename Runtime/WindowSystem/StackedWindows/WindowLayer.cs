using Bloodthirst.Core.SceneManager;
using Bloodthirst.Core.Utils;
using Bloodthirst.Scripts.Core.GamePassInitiator;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Core.UI
{
    public abstract class WindowLayer<T> : MonoBehaviour, IWindowLayer, IOnSceneLoaded where T : WindowLayer<T>
    {
        [SerializeField]
        protected RectTransform container;

        [SerializeField]
        protected Canvas canvas;

        public Canvas Canvas => canvas;

        public event Action<IWindowLayer> OnLayerFocused;

        public event Action<IWindowLayer> OnLayerUnfocused;

        #if ODIN_INSPECTOR[ShowInInspector]#endif
        private List<IUIWindow> uiWindows = new List<IUIWindow>();

        private List<IUIWindow> openWindows = new List<IUIWindow>();

        public List<IUIWindow> UiWindows { get => uiWindows; set => uiWindows = value; }


        public void OnLoaded(ISceneInstanceManager manager)
        {
            CloseAll();
        }

        void IWindowLayer.OnInitialize()
        {
            WindowLayerManager.Add(this);
        }

        void IWindowLayer.OnDestroy()
        {
            WindowLayerManager.Remove(this);
        }


        void IWindowLayer.Add<TWindow>(TWindow t)
        {
            // sub events
            t.OnOpen -= OnWindowOpen;
            t.OnOpen += OnWindowOpen;

            t.OnFocus -= OnWindowFocus;
            t.OnFocus += OnWindowFocus;

            t.OnUnfocus -= OnWindowUnfocus;
            t.OnUnfocus += OnWindowUnfocus;

            t.OnClose -= OnWindowClose;
            t.OnClose += OnWindowClose;

            // remove and add to the front
            uiWindows.Remove(t);
            uiWindows.Add(t);
        }

        private void OnWindowOpen(IUIWindow uiWindow)
        {
            uiWindows.Remove(uiWindow);
            uiWindows.Add(uiWindow);

            Refresh();
        }
        private void OnWindowFocus(IUIWindow uiWindow)
        {
            if (uiWindows.Last() == uiWindow)
            {
                Refresh();
                OnLayerFocused?.Invoke(this);
                return;
            }
            if (!uiWindows.Contains(uiWindow))
            {
                Refresh();
                OnLayerFocused?.Invoke(this);
                return;
            }

            uiWindows.Remove(uiWindow);
            uiWindows.Add(uiWindow);

            OnLayerFocused?.Invoke(this);

            Refresh();
        }
        private void OnWindowUnfocus(IUIWindow uiWindow)
        {
            Refresh();
        }
        private void OnWindowClose(IUIWindow uiWindow)
        {
            Refresh();
        }

        void IWindowLayer.Remove<TWindow>(TWindow t)
        {
            // unsub events
            t.OnOpen -= OnWindowOpen;
            t.OnFocus -= OnWindowFocus;
            t.OnUnfocus -= OnWindowUnfocus;
            t.OnClose -= OnWindowClose;

            // remove
            uiWindows.Remove(t);
        }
        public IEnumerable<TWindow> Get<TWindow>() where TWindow : IUIWindow
        {
            for (int i = 0; i < uiWindows.Count; i++)
            {
                var cast = (TWindow)uiWindows[i];

                if (cast != null)
                    yield return cast;
            }
        }

        #if ODIN_INSPECTOR[Button]#endif
        public void OpenAll()
        {
            for (int i = 0; i < uiWindows.Count; i++)
            {
                if (!uiWindows[i].IsOpen)
                    uiWindows[i].RequestOpen = true;
            }


            OnLayerFocused?.Invoke(this);
            Refresh();
        }

        #if ODIN_INSPECTOR[Button]#endif
        public void CloseAll()
        {
            for (int i = 0; i < uiWindows.Count; i++)
            {
                if (uiWindows[i].IsOpen)
                    uiWindows[i].RequestClose = true;
            }

            OnLayerUnfocused?.Invoke(this);
            Refresh();
        }

        public void Refresh()
        {
            if (uiWindows.Count == 0)
                return;

            // handle closing windows
            for (int i = 0; i < uiWindows.Count; i++)
            {
                IUIWindow window = uiWindows[i];
                if (window.RequestClose)
                {
                    window.BeforeClose();
                    StartCoroutine(window.Close());

                    window.IsOpen = false;
                    window.IsFocused = false;

                    window.RequestClose = false;
                    window.RequestFocus = false;
                    window.RequestOpen = false;
                    window.RequestUnfocus = false;
                }
            }

            // clear opens windows saved
            openWindows.Clear();

            // - select the windows that need to shown
            // which  are the windows that are already opened
            // or the window that requested tp be opened
            for (int i = 0; i < uiWindows.Count; i++)
            {
                IUIWindow window = uiWindows[i];

                if (window.IsOpen || window.RequestOpen)
                    openWindows.Add(window);
            }

            // operate on all open windows EXCEPT the last one
            // since they are behind the first window, they should be unfocused
            for (int i = 0; i < openWindows.Count - 1; i++)
            {
                IUIWindow win = openWindows[i];

                // if the window is already unfocused and no RequestFocus is called
                // then we can skip it since it's already setup correctly
                if (!win.IsFocused && !win.RequestOpen)
                {
                    win.RequestClose = false;
                    win.RequestFocus = false;
                    win.RequestUnfocus = false;
                    win.RequestOpen = false;
                    continue;
                }

                // if a focus request is done then we switch spots
                if (win.RequestFocus)
                {
                    openWindows[i] = openWindows[openWindows.Count - 1];
                    openWindows[openWindows.Count - 1] = win;
                    win = openWindows[i];
                }

                // unfocus
                win.IsFocused = false;

                // this happens when we open everything at the same time
                // but this is the case where a window asked to be open but it's in the middle of the stack
                if (win.RequestOpen)
                {
                    win.IsOpen = true;
                    win.BeforeOpen();

                    // combine the open and unfocus
                    StartCoroutine(GameObjectUtils.CombineCoroutine(win.Open(), win.Unfocus()));
                }
                else
                {
                    StartCoroutine(win.Unfocus());
                }
                win.RequestClose = false;
                win.RequestFocus = false;
                win.RequestUnfocus = false;
                win.RequestOpen = false;
            }

            if (openWindows.Count == 0)
                return;

            IUIWindow last = openWindows[openWindows.Count - 1];

            last.IsOpen = true;

            // we do still the Unfocus check is in case where we want to unfocus the entire layer (for example , player clicked on a window in a different layer)
            // in that case even the first open window is unfocused
            if (last.RequestOpen)
            {
                last.BeforeOpen();

                if (last.RequestUnfocus)
                {
                    last.IsFocused = false;
                    StartCoroutine(GameObjectUtils.CombineCoroutine(last.Open(), last.Unfocus()));
                }
                else
                {
                    last.IsFocused = true;
                    StartCoroutine(last.Open());
                }
            }
            else
            {
                if (last.RequestUnfocus)
                {
                    last.IsFocused = false;
                    StartCoroutine(last.Unfocus());
                }
                else
                {
                    last.IsFocused = true;
                    StartCoroutine(last.Focus());
                }
            }


            last.RequestClose = false;
            last.RequestFocus = false;
            last.RequestUnfocus = false;
            last.RequestOpen = false;

            // - then order them under the parents transform
            for (int i = 0; i < uiWindows.Count; i++)
            {
                IUIWindow window = uiWindows[i];

                window.ParentTransform.SetSiblingIndex(i);
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (UiWindows.Count == 0)
                return;

            if (UiWindows.Last().RequestClose)
            {
                Refresh();
            }
        }

    }

}