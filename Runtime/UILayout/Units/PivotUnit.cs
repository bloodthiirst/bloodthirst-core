using System;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    [Serializable]
    public struct PivotUnit
    {
        [SerializeField]
        private LengthUnit x;

        [SerializeField]
        private LengthUnit y;

        public LengthUnit X { get => x; set => x = value; }
        public LengthUnit Y { get => y; set => y = value; }

        public PivotUnit(LengthUnit x, LengthUnit y)
        {
            this.x = x;
            this.y = y;
        }

    }
}
