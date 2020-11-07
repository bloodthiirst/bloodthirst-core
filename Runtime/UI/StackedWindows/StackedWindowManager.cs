using Bloodthirst.Core.UnitySingleton;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Bloodthirst.Core.UI
{
    public abstract class StackedWindowManager<T> : UnitySingleton<T>, IStackedWindowManager where T : StackedWindowManager<T>
    {
        [SerializeField]
        protected RectTransform container;

        [ShowInInspector]
        private List<IUIWindow> uiWindows;

        public List<IUIWindow> UiWindows { get => uiWindows; set => uiWindows = value; }

        protected override void Awake()
        {
            UiWindows = new List<IUIWindow>();
        }

        void IStackedWindowManager.Add<T1>(T1 t)
        {
            // if doesn't exist add to the front
            if (!uiWindows.Contains(t))
            {
                uiWindows.Add(t);
            }

            // else remove and add to the front
            else
            {
                uiWindows.Remove(t);
                uiWindows.Add(t);
            }

            t.ParentTransform.SetParent(container);
            Refresh();
        }

        void IStackedWindowManager.Remove<T1>(T1 t)
        {
            uiWindows.Remove(t);
            Refresh();
        }

        private void Refresh()
        {
            // sort every thing except the last window
            for(int i = 0; i < uiWindows.Count - 1; i++)
            {
                IUIWindow window = uiWindows[i];

                window.ParentTransform.SetParent(container);
                window.ParentTransform.SetSiblingIndex(i);

                window.Hide();
            }

            // setup the last window
            if (uiWindows.Count > 0)
            {
                IUIWindow last = uiWindows[uiWindows.Count - 1];
                last.ParentTransform.SetParent(container);
                last.ParentTransform.SetSiblingIndex(uiWindows.Count - 1);

                if (last.IsHidden)
                {
                    last.Show();
                }
            }
        }


        // Update is called once per frame
        void Update()
        {
            if (UiWindows.Count == 0)
                return;

            if (UiWindows.Last().RequestClose())
            {
                IUIWindow window = UiWindows.Last();
                window.TriggerClose();

                Refresh();
            }
        }
    }

}