using Bloodthirst.Core.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.Quadrant
{
    public class QuadrantEntityBehaviour : MonoBehaviour, IQuadrantEntity<QuadrantEntityBehaviour>
    {
        public event Action<QuadrantEntityBehaviour> OnQuadrantIdChanged;

        public event Action<QuadrantEntityBehaviour> OnPositionChanged;

        [SerializeField]
        private QuadrantManagerBehaviour manager;

        private void OnDestroy()
        {
            Remove();
        }

        [Button]
        private void Add()
        {
            ((IQuadrantEntity<QuadrantEntityBehaviour>)this).QuandrantId = null;
            manager?.QuadrantManager.Add(this);
        }

        [Button]
        private void Remove()
        {
            if (manager == null)
                return;

            manager.QuadrantManager.Remove(((IQuadrantEntity<QuadrantEntityBehaviour>)this).QuandrantId, this);
        }

        private void OnDrawGizmos()
        {
            if (((IQuadrantEntity<QuadrantEntityBehaviour>)this).QuandrantId == null)
            {
                return;
            }

            CheckPositionChanged();
        }

        private void CheckPositionChanged()
        {
            if (transform.hasChanged)
            {

                Position = transform.position;
            }
        }

        private void Update()
        {
            CheckPositionChanged();
        }

        [SerializeField]
        private List<int> quandrantId;
        List<int> IQuadrantEntity<QuadrantEntityBehaviour>.QuandrantId
        {
            get => quandrantId;
            set
            {
                if (!quandrantId.IsSame(value))
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
                if (position != value)
                {
                    position = value;
                    OnPositionChanged?.Invoke(this);
                }
            }
        }

        Vector3 IQuadrantEntity<QuadrantEntityBehaviour>.Postion => Position;
    }
}
