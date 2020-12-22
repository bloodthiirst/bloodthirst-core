using Assets.Scripts.Core.Utils;
using Bloodthirst.Core.UnitySingleton;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Bloodthirst.Core.UI
{
    public abstract class WindowLayer<T> : UnitySingleton<T>, IWindowLayer where T : WindowLayer<T>
    {
        [SerializeField]
        protected RectTransform container;

        [ShowInInspector]
        private List<IUIWindow> uiWindows;

        public List<IUIWindow> UiWindows { get => uiWindows; set => uiWindows = value; }

        protected override void Awake()
        {
            UiWindows = new List<IUIWindow>();

            WindowLayerManager.Add(this);
        }

        protected void OnDestroy()
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
                return;

            if (!uiWindows.Contains(uiWindow))
                return;

            uiWindows.Remove(uiWindow);
            uiWindows.Add(uiWindow);

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

        [Button]
        public void OpenAll()
        {
            for (int i = 0; i < uiWindows.Count; i++)
            {
                if (!uiWindows[i].IsOpen)
                    uiWindows[i].RequestOpen = true;
            }

            Refresh();
        }

        [Button]
        public void CloseAll()
        {
            for (int i = 0; i < uiWindows.Count; i++)
            {
                if (uiWindows[i].IsOpen)
                    uiWindows[i].RequestClose = true;
            }

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

            List<IUIWindow> openWindows = new List<IUIWindow>();

            // sort every thing except the last window
            for (int i = 0; i < uiWindows.Count; i++)
            {
                IUIWindow window = uiWindows[i];

                if (window.IsOpen)
                    openWindows.Add(window);

                if (window.RequestOpen)
                    openWindows.Add(window);

                window.ParentTransform.SetSiblingIndex(i);
            }

            for (int i = 0; i < openWindows.Count - 1;i++)
            {
                IUIWindow win = openWindows[i];

                if (!win.IsFocused && !win.RequestOpen)
                    continue;

                win.IsFocused = false;
                win.IsOpen = true;

                if (win.RequestOpen)
                {
                    win.BeforeOpen();
                    StartCoroutine(GameObjectUtils.CombineCoroutine( win.Open() , win.Unfocus()));
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

            IUIWindow last = openWindows.Last();

            last.IsFocused = true;
            last.IsOpen = true;

            if (last.RequestOpen)
            {
                last.BeforeOpen();
                StartCoroutine(last.Open());
            }
            else
            {
                StartCoroutine(last.Focus());
            }


            last.RequestClose = false;
            last.RequestFocus = false;
            last.RequestUnfocus = false;
            last.RequestOpen = false;

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