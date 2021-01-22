using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bloodthirst.System.Quadrant
{
    public interface IQuadrantEntity<T> where T : IQuadrantEntity<T>
    {
        event Action<T> OnPositionChanged;
        event Action<T> OnQuadrantIdChanged;
        List<int> QuandrantId { get; set; }
        Vector3 Postion { get; }
    }
}
