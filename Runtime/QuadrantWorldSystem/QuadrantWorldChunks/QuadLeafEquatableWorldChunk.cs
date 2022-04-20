using UnityEngine;

namespace Bloodthirst.System.Quadrant
{
    public class QuadLeafEquatableWorldChunk : QuadLeafEquatableBase<int, QuadrantEntityBehaviour, QuadLeafEquatableWorldChunk>
    {
        [SerializeField]
        private bool isCulled;
        public bool IsCulled
        {
            get => isCulled;
            set
            {
                if (isCulled != value)
                {
                    isCulled = value;
                    OnCullingChanged();
                }
            }
        }

        private void OnCullingChanged()
        {
            foreach (QuadrantEntityBehaviour e in Elements)
            {
                e.OnCulledStatusChanged(isCulled);
            }
        }

        protected override void OnElementAdded(QuadrantEntityBehaviour element)
        {
            element.OnCulledStatusChanged(isCulled);
        }
    }
}
