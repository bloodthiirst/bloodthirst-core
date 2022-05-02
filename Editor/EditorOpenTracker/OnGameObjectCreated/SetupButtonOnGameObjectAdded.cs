using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Bloodthirst.Utils.EditorOpenTracker
{
    internal class SetupButtonOnGameObjectAdded : IOnGameObjectAdded
    {
        void IOnGameObjectAdded.HandleGameObjectAdded(GameObject obj)
        {
            if(!obj.TryGetComponent(out Button button))
            {
                return;
            }

            GameObject interactable = new GameObject("Interactable");
            interactable.transform.SetParent(button.transform);

            Image uiCollider = interactable.AddComponent<Image>();

            uiCollider.rectTransform.anchorMin = Vector2.zero;
            uiCollider.rectTransform.anchorMax = Vector2.one;
            uiCollider.rectTransform.pivot = new Vector2(0.5f , 0.5f);

            uiCollider.rectTransform.rotation = Quaternion.identity;
            uiCollider.rectTransform.localScale = Vector2.one;

            uiCollider.rectTransform.offsetMax = Vector2.zero;
            uiCollider.rectTransform.offsetMin = Vector2.zero;

            uiCollider.raycastTarget = true;
            uiCollider.maskable = false;
        }
    }
}
