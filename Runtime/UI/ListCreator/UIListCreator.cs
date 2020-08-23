using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bloodthirst.Core.UI
{
    public class UIListCreator<UI, INSTANCE> where UI : MonoBehaviour , IUIElement<INSTANCE>
    {
        private IList<UI> cachedUIs;

        private UI prefab;

        private RectTransform container;

        public UIListCreator(IList<INSTANCE> instances, IList<UI> cachedUIs, UI prefab, RectTransform container)
        {
            this.cachedUIs = cachedUIs;
            this.prefab = prefab;
            this.container = container;
        }

        public void RefreshUI(IList<INSTANCE> instances)
        {
            int connectionCount = instances.Count;

            if (connectionCount == 0)
            {
                container.gameObject.SetActive(false);
                return;
            }


            container.gameObject.SetActive(true);

            // difference between total instances and UIs available

            int uiDiff = connectionCount - cachedUIs.Count;

            // instantiate extra uis if needed

            if (uiDiff > 0)
            {
                // spawn the ui
                for (int i = 0; i < uiDiff; i++)
                {
                    UI ui = Object.Instantiate(prefab, container);
                    cachedUIs.Add(ui);
                }
            }

            // setup ui

            for (int i = 0; i < connectionCount; i++)
            {
                INSTANCE connectedBuilding = instances[i];
                cachedUIs[i].gameObject.SetActive(true);
                cachedUIs[i].SetupUI(connectedBuilding);
            }

            // disable ununsed uis

            for (int i = connectionCount; i < cachedUIs.Count; i++)
            {
                cachedUIs[i].CleanupUI();
                cachedUIs[i].gameObject.SetActive(false);
            }

            // refresh ui
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);
        }
    }
}
