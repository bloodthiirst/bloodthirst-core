using System;
using UnityEngine;

namespace Bloodthirst.System.Quadrant
{
    public interface IQuadrantEntity
    {
        event Action<IQuadrantEntity> OnPositionChanged;
        event Action<IQuadrantEntity> OnQuadrantIdChanged;
        (int,int,int)? QuandrantId { get; set; }
        Vector3 Postion { get; }
    }
}
