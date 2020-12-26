using System;
using UnityEngine;

namespace Bloodthirst.System.Quadrant
{
    public class QuadrantEntityBehaviour : MonoBehaviour , IQuadrantEntity
    {
        public event Action<IQuadrantEntity> OnQuadrantIdChanged;

        [SerializeField]
        private (int, int, int)? quandrantId;
        (int, int, int)? IQuadrantEntity.QuandrantId 
        { 
            get => quandrantId;
            set
            {
                if(quandrantId != value)
                {
                    quandrantId = value;
                    OnQuadrantIdChanged?.Invoke(this);
                }
            }
        }
        Vector3 IQuadrantEntity.Postion => transform.position;

    }
}
