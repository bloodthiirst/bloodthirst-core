using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Bloodthirst.Utils.EditorOpenTracker
{
    internal class DisableRaycasTargetAndMaskOnGameObjectAdded : IOnGameObjectAdded
    {
        void IOnGameObjectAdded.HandleGameObjectAdded(GameObject obj)
        {
            List<Graphic> pooledListGraphic = ListPool<Graphic>.Get();
            List<MaskableGraphic> pooledListMaskables = ListPool<MaskableGraphic>.Get();

            obj.GetComponentsInChildren(pooledListGraphic);
            obj.GetComponentsInChildren(pooledListMaskables);

            foreach(Graphic graphic in pooledListGraphic)
            {
                graphic.raycastTarget = false;
            }
            foreach(MaskableGraphic maskable in pooledListMaskables)
            {
                maskable.maskable = false;
            }

            ListPool<Graphic>.Release(pooledListGraphic);
            ListPool<MaskableGraphic>.Release(pooledListMaskables);
        }
    }
}
