using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Core.Utils
{
    public static class GameObjectUtils
    {
        public static void ClearTransform(this Transform t)
        {
            for(int i = t.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(t.GetChild(i).gameObject);
            }
        }

        public static void ClearTransformEditor(this Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(t.GetChild(i).gameObject);
            }
        } 

        public static void CreateOrClear<C> (this C collection) where C : IList , new()
        {
            if (collection == null)
                collection = new C();
            else
                collection.Clear();

        }

        public static void CreateOrClearDict<D>(this D collection) where D : IDictionary, new()
        {
            if (collection == null)
                collection = new D();
            else
                collection.Clear();

        }
    }
}
