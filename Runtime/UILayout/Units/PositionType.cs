using System;
using UnityEngine;

namespace Bloodthirst.Core.UILayout
{
    public enum PositionKeyword : int
    {
        DISPLAY_MODE,
        PARENT_SPACE,
        SCREEN_SPACE
    }

    [Serializable]
    public struct PositionType
    {
        [SerializeField]
        private PositionKeyword positionKeyword;

        [SerializeField]
        private Vector2 positionValue;

        public PositionKeyword PositionKeyword => positionKeyword;

        public Vector2 PositionValue => positionValue;
        public PositionType(PositionKeyword positionKeyword)
        {
            this.positionKeyword = positionKeyword;
            this.positionValue = default;
        }

        public PositionType(PositionKeyword positionKeyword, Vector2 positionValue)
        {
            this.positionKeyword = positionKeyword;
            this.positionValue = positionValue;
        }
    }
}
