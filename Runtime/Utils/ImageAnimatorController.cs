using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageAnimatorController : MonoBehaviour
{
    private List<ImageAnimator> imageAnimators;

    private void OnValidate()
    {
        if (imageAnimators == null)
        {
            imageAnimators = new List<ImageAnimator>();
        }
        else
        {
            imageAnimators.Clear();
        }

        GetComponents<ImageAnimator>(imageAnimators);
    }

    public void StopAll()
    {
        foreach (ImageAnimator animator in imageAnimators)
        {
            animator.Pause();
            animator.Init();
        }
    }

}
