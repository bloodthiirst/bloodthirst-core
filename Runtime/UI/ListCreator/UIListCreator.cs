﻿using Bloodthirst.Core.AdvancedPool;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bloodthirst.Core.UI
{
    /// <summary>
    /// A helper class used to help make UI collections and link them easily
    /// </summary>
    /// <typeparam name="UI"></typeparam>
    /// <typeparam name="INSTANCE"></typeparam>
    public class UIListCreator<UI, INSTANCE> where UI : MonoBehaviour, IUIElement<INSTANCE>
    {
        private IList<UI> cachedUIs;
        private UnityPool<UI> pool;
        private RectTransform container;
        private int fixedUICount;

        public IList<UI> UIs => cachedUIs;

        public int FixedUICount { get => fixedUICount; set => fixedUICount = value; }

        public UIListCreator(UnityPool<UI> pool, RectTransform container, IList<UI> cachedUIs = null, int fixedUICount = -1)
        {
            // if not list is supplied , create one on the fly
            this.cachedUIs = cachedUIs == null ? new List<UI>() : cachedUIs;

            this.pool = pool;
            this.container = container;
            this.FixedUICount = fixedUICount;
        }

        /// <summary>
        /// Refresh the list of the UIs by passing it the instances that we want to show
        /// </summary>
        /// <param name="instances"></param>
        public void RefreshUI(IList<INSTANCE> instances)
        {
            int instCount = fixedUICount == -1 ? instances.Count : fixedUICount;

            if (instCount == 0)
            {
                container.gameObject.SetActive(false);
                return;
            }


            container.gameObject.SetActive(true);

            // difference between total instances and UIs available

            int uiDiff = instCount - cachedUIs.Count;

            // instantiate extra uis if needed

            if (uiDiff > 0)
            {
                // spawn the ui
                for (int i = 0; i < uiDiff; i++)
                {
                    UI ui = pool.GetTyped();
                    cachedUIs.Add(ui);
                }
            }

            // setup ui

            for (int i = 0; i < instCount; i++)
            {
                INSTANCE inst = i < instances.Count ? instances[i] : default;
                cachedUIs[i].gameObject.SetActive(true);

                // clean up first in case its needed to clear dependencies that were linked previously
                cachedUIs[i].CleanupUI();

                cachedUIs[i].transform.SetParent(container);

                cachedUIs[i].SetupUI(inst);

            }

            // disable ununsed uis

            for (int i = instCount; i < cachedUIs.Count; i++)
            {
                cachedUIs[i].CleanupUI();
                cachedUIs[i].gameObject.SetActive(false);
            }

            // refresh ui
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);
        }
    }
}
