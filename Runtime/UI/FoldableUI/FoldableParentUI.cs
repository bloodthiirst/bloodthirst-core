using Bloodthirst.Core.Utils;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bloodthirst.Core.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class FoldableParentUI : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;

        [SerializeField]
        private List<FoldableUI> foldables;

        private void OnValidate()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            // clear the list
            foldables = foldables.CreateOrClear();

            // fetch the child elements
            GetComponentsInChildren(foldables);
        }

        /// <summary>
        /// Do we need to update thee layout ?
        /// </summary>
        #if ODIN_INSPECTOR[ReadOnly]#endif
        #if ODIN_INSPECTOR[ShowInInspector]#endif
        private bool isDirty;

        public void AddFoladable(FoldableUI foldable)
        {
            foldables.Add(foldable);

            foldable.OnAnimationStarted -= Foldable_OnAnimationStarted;
            foldable.OnAnimationStarted += Foldable_OnAnimationStarted;

            foldable.OnRemoved -= Foldable_OnRemoved;
            foldable.OnRemoved += Foldable_OnRemoved;
        }

        public void RemoveFoladable(FoldableUI foldable)
        {
            foldables.Remove(foldable);

            foldable.OnAnimationStarted -= Foldable_OnAnimationStarted;
            foldable.OnRemoved -= Foldable_OnRemoved;
        }

        private void Foldable_OnRemoved(FoldableUI foldable)
        {
            RemoveFoladable(foldable);
        }

        private void Foldable_OnAnimationStarted(FoldableUI obj)
        {
            isDirty = true;
        }

        private void Awake()
        {
            List<FoldableUI> copy = new List<FoldableUI>(foldables);

            // clear the list
            foldables = foldables.CreateOrClear();

            foreach (FoldableUI f in copy)
            {
                AddFoladable(f);
            }

        }

        private void Update()
        {
            // if we don't need updating we exit
            if (!isDirty)
                return;

            // as long as an element is updating we keep refreshing
            for (int i = 0; i < foldables.Count; i++)
            {
                if (foldables[i].IsAnimating)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
                    return;
                }
            }

            // once we reach this that means all elements are static
            // so we set dirty to false
            isDirty = false;

        }
    }
}
