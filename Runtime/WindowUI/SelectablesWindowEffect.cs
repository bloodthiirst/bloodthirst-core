using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Bloodthirst.UI
{
    /// <summary>
    /// Window effect for selectables opening closing , toggles interactables to avoid gamepad selection slipping into unwanted UIs
    /// </summary>
    public class SelectablesWindowEffect : MonoBehaviour, IWindowUIEffect
    {
        [SerializeField]
        private Selectable[] selectables;

        [ContextMenu("Get all selectables")]
        private void GetAllSelectables()
        {
            using (ListPool<Selectable>.Get(out List<Selectable> tmp))
            {
                GetComponentsInChildren(true, tmp);

                selectables = tmp.ToArray();
            }
        }

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
            foreach (Selectable s in selectables)
            {
                s.interactable = true;
            }
        }

        public void CloseImmidiate(IWindowUI window)
        {
            foreach (Selectable s in selectables)
            {
                s.interactable = false;
            }
        }
    }
}
