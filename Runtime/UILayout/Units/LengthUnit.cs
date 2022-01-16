using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    public enum UnitType
    {
        PERCENTAGE,
        PIXEL,
        KEYWORD
    }

    public enum UnitKeyword
    {
        None,
        Auto,
        Content
    }

    [Serializable]
    public struct LengthUnit
    {
        [SerializeField]
        private UnitType unitType;

        [SerializeField]
        private UnitKeyword keywordValue;

        [SerializeField]
        private float unitValue;

        public UnitType UnitType => unitType;
        public float UnitValue => unitValue;
        public UnitKeyword KeywordValue => keywordValue;
        public LengthUnit(UnitKeyword keyword)
        {
            unitType = UnitType.KEYWORD;
            keywordValue = keyword;
            unitValue = default;
        }
        public LengthUnit(float value, UnitType unit)
        {
            unitType = unit;
            unitValue = value;
            keywordValue = UnitKeyword.None;
        }
    }
}
