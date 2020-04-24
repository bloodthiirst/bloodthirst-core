using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class VectorUtils
    {
        /// <summary>
        /// Changes the vectors velue separatly and returns the vector with the new values
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 Change( this Vector3 vec , float? x = null , float? y = null, float? z = null)
        {
            Vector3 res = vec;

            if (x != null)
                res.x = x.Value;
            
            if (y != null)
                res.y = y.Value;
            
            if (z != null)
                res.z = z.Value;
            
            return res;
        }   
    }
}
