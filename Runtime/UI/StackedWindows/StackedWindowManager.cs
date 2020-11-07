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
            if (!uiWindows.Contains(t))
            {
                uiWindows.Add(t);
            }
            t.ParentTransform.SetParent(container);
            t.ParentTransform.SetAsLastSibling();
            Refresh();
        }

        void IStackedWindowManager.Remove<T1>(T1 t)
        {
            uiWindows.Remove(t);
            Refresh();
        }

        private void Refresh()
        {
            // TODO : fix the sort order
        }


        // Update is called once per frame
        void Update()
        {
            if (UiWindows.Count == 0)
                return;

            if (UiWindows.Last().RequestClose())
            {
                IUIWindow window = UiWindows.Last();
                UiWindows.RemoveAt(UiWindows.Count - 1);
                window.TriggerClose();
            }
        }
    }

}