using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    [Serializable]
    public struct LengthUnit
    {
        [SerializeField]
        private UnitType unitType;

        [SerializeField]
        private float unitValue;

        public UnitType UnitType => unitType;
        public float UnitValue => unitValue;
        public LengthUnit(float value, UnitType unit)
        {
            unitType = unit;
            unitValue = value;
        }
    }
}
