using UnityEngine;

namespace Assets.Scripts.Core.Utils
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

            if (!IsBetween(val, oldMin, oldMax))
            {
                Debug.LogError("val needs to be between the oldMibn and oldMax");
                return 0;
            }

            if (oldMax < oldMin)
            {
                Debug.LogError("oldMax needs to be greater than oldMin");
                return 0;
            }

            if (newMax < newMin)
            {
                Debug.LogError("newMax needs to be greater than newMin");
                return 0;
            }

            float ratio = (val - oldMin) / (oldMax - oldMin);

            return ((newMax - newMin) * ratio) + newMin;

        }
    }
}
