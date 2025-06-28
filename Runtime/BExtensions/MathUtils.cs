using UnityEngine;
using UnityEngine.Assertions;

namespace Bloodthirst.Core.Utils
{
    public static class MathUtils
    {
        /// <summary>
        /// Is val between min (inclusive) and max (inclusive) ?
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(float val, float min, float max)
        {
            Assert.IsTrue(min < max);

            bool isValid = val <= max && val >= min;

            return isValid;
        }

        /// <summary>
        /// Is val between min (inclusive) and max (inclusive) ?
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(int val, int min, int max)
        {
            Assert.IsTrue(min < max);

            bool isValid = val <= max && val >= min;

            return isValid;
        }

        public static float Remap(float val, float oldMin, float oldMax, float newMin, float newMax)
        {
            float ratio = (val - oldMin) / (oldMax - oldMin);

            return ((newMax - newMin) * ratio) + newMin;

        }
    }
}
