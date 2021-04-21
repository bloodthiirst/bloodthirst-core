using Bloodthirst.Core.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.Quadrant
{
    [ExecuteAlways]
    public class QuadrantEntityBehaviour : MonoBehaviour, IQuadrantEntity<QuadrantEntityBehaviour>
    {
        public event Action<QuadrantEntityBehaviour> OnQuadrantIdChanged;

        public event Action<QuadrantEntityBehaviour> OnPositionChanged;

        [SerializeField]
        private QuadrantManagerBehaviour manager;

        [SerializeField]
        private GameObject graphics;

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


        private void Update()
        {
            if (((IQuadrantEntity<QuadrantEntityBehaviour>)this).QuandrantId == null)
            {
                return;
            }

            CheckPositionChanged();
        }

        private void OnDestroy()
        {
            Remove();
        }

        public void OnCulledStatusChanged(bool isCulled)
        {
            graphics.SetActive(!isCulled);
        }

        private void CheckPositionChanged()
        {
            if (transform.hasChanged)
            {
                Position = transform.position;
            }
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
    }
}
