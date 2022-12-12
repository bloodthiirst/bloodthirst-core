using DG.Tweening;
#if ODIN_INSPECTOR
	using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;

namespace Bloodthirst.Core.UI
{
    [Flags]
    public enum DIRECTION : int
    {
        HORIZONTAL = 0x01,
        VERTICAL = 0x02
    }

    public class FoldableUI : MonoBehaviour
    {
        [SerializeField]
        private RectTransform rectTransform;

        [SerializeField]
        private CanvasGroup group;

        [SerializeField]
        private float speed;

        [SerializeField]
        private DIRECTION foldDirection;

        public event Action<FoldableUI> OnAnimationStarted;

        public event Action<FoldableUI> OnRemoved;

        private bool isAnimating;

        
#if ODIN_INSPECTOR
[ReadOnly]
#endif

        
#if ODIN_INSPECTOR
[ShowInInspector]
#endif

        private bool isOpen = true;

        public bool IsAnimating
        {
            get => isAnimating;
            private set
            {
                if (!isAnimating && value)
                {
                    isAnimating = value;
                    OnAnimationStarted?.Invoke(this);
                    return;
                }

                isAnimating = value;
            }
        }

        private Vector2 deltaSize;

        private void OnValidate()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
        }

        private void OnDestroy()
        {
            OnRemoved?.Invoke(this);
        }


        
#if ODIN_INSPECTOR
[Button]
#endif

        public void Open()
        {
            if (isOpen)
                return;
            isOpen = true;

            // Grab a free Sequence to use
            Sequence mySequence = DOTween.Sequence();


            mySequence
                .AppendCallback(() => IsAnimating = true)
                //.Append(rectTransform.DOSizeDelta(deltaSize, speed))
                //.Join(group.DOFade(1, speed))
                .AppendCallback(() => IsAnimating = false)
                .Play();
        }

        
#if ODIN_INSPECTOR
[Button]
#endif

        public void Close()
        {
            if (!isOpen)
                return;
            isOpen = false;

            // save the size
            deltaSize = rectTransform.sizeDelta;

            Vector2 closedSize = deltaSize;
            closedSize.y = (foldDirection & DIRECTION.VERTICAL) == DIRECTION.VERTICAL ? 0 : closedSize.y;
            closedSize.x = (foldDirection & DIRECTION.HORIZONTAL) == DIRECTION.HORIZONTAL ? 0 : closedSize.x;


            Sequence seq = DOTween.Sequence();
            seq
            .AppendCallback(() => IsAnimating = true)
            //.Append(rectTransform.DOSizeDelta(closedSize, speed))
            //.Join(group.DOFade(0, speed))
            .AppendCallback(() => IsAnimating = false)
            .Play();
        }
    }
}
