using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Core.Utils
{
    public static class MathUtils
    {
        /// <summary>
        /// Is val between min and max ? (inclusive)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(float val, float min, float max)
        {
            if (val < min)
                return false;
            if (val > max)
                return false;

            return true;
        }

        public static float Remap(float val, float oldMin, float oldMax, float newMin, float newMax)
        {
            float ratio = (val - oldMin) / (oldMax - oldMin);

            return ((newMax - newMin) * ratio) + newMin;

        }
    }
}
