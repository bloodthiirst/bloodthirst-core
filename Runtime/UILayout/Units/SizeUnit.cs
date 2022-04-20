using System;
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
        Content,
        BasedOnWidth,
        BasedOnHeight
    }

    [Serializable]
    public struct SizeUnit
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
        public SizeUnit(UnitKeyword keyword)
        {
            unitType = UnitType.KEYWORD;
            keywordValue = keyword;
            unitValue = default;
        }
        public SizeUnit(float value, UnitType unit)
        {
            unitType = unit;
            unitValue = value;
            keywordValue = UnitKeyword.None;
        }
    }
}
