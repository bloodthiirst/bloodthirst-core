using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Bloodthirst.System.Quadrant
{
    public class QuadrantEntityBehaviour : MonoBehaviour , IQuadrantEntity
    {
        public event Action<IQuadrantEntity> OnQuadrantIdChanged;

        public event Action<IQuadrantEntity> OnPositionChanged;

        [SerializeField]
        private QuadrantManagerBehaviour manager;

        private void OnDestroy()
        {
            Remove();
        }

        [Button]
        private void Add()
        {
            ((IQuadrantEntity)this).QuandrantId = null;
            manager?.QuadrantManager.Add(this);
        }

        [Button]
        private void Remove()
        {
            if (manager == null)
                return;

            manager.QuadrantManager.Remove(((IQuadrantEntity)this).QuandrantId, this);
        }




        private void OnDrawGizmos()
        {
            if(((IQuadrantEntity)this).QuandrantId == null)
            {
                return;
            }

            if(transform.hasChanged)
            {

                Position = transform.position;
            }
        }

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

        private Vector3 position;
        public Vector3 Position
        {
            get => position;
            set
            {
                if(position != value)
                {
                    position = value;
                    OnPositionChanged?.Invoke(this);
                }
            }
        }
        
        Vector3 IQuadrantEntity.Postion => Position;

    }
}
