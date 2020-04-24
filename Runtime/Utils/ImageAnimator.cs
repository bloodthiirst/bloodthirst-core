using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image) , typeof(ImageAnimatorController))]
public class ImageAnimator : MonoBehaviour
{
    [SerializeField]
    private Sprite spriteSheet;

    [SerializeField]
    private ImageAnimatorController imageAnimatorController;

    [SerializeField]
    private Image image;

    [SerializeField]
    [Range(0.1f, 10)]
    private float animationDuration;

    [SerializeField]
    private float currentTimer;

    [SerializeField]
    private bool isLoopMode;

    [ReadOnly]
    [SerializeField]
    private bool isPlaying;

    [ReadOnly]
    [SerializeField]
    private bool isDone;

    [ReadOnly]
    [SerializeField]
    private int currentFrame;

    private Sprite[] animationFrames;

    [SerializeField]
    private UnityEvent OnAnimationEnd;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (imageAnimatorController == null)
        {
            imageAnimatorController = GetComponent<ImageAnimatorController>();
        }

        if (image == null)
        {
            image = GetComponent<Image>();
        }

        string spritepath = AssetDatabase.GetAssetPath(spriteSheet);
        animationFrames = AssetDatabase.LoadAllAssetsAtPath(spritepath).OfType<Sprite>().ToArray();
    }

#endif

    // Update is called once per frame
    void Update()
    {
        if (isPlaying == false)
            return;

        if (isLoopMode)
            LoopAnimation();
        else
            OnShotAnimation();

    }

    private void LoopAnimation()
    {
        currentTimer += Time.deltaTime;

        while (currentTimer >= animationDuration)
        {
            currentTimer -= animationDuration;
        }

        float timePerFrame = animationDuration / animationFrames.Length;

        currentFrame = Mathf.Min(Mathf.FloorToInt(currentTimer / timePerFrame), animationFrames.Length - 1);

        image.sprite = animationFrames[currentFrame];
    }

    private void OnShotAnimation()
    {

        if (isDone)
            return;

        currentTimer += Time.deltaTime;

        if (currentTimer >= animationDuration)
        {
            isDone = true;

            OnAnimationEnd?.Invoke();

            currentTimer = animationDuration;
        }

        float timePerFrame = animationDuration / animationFrames.Length;

        currentFrame =  Mathf.Min ( Mathf.FloorToInt(currentTimer / timePerFrame) , animationFrames.Length - 1);

        image.sprite = animationFrames[currentFrame];
    }

    [Button]
    public void Play()
    {
        imageAnimatorController.StopAll();
        isPlaying = true;
        isDone = false;
    }

    [Button]
    public void Replay()
    {
        imageAnimatorController.StopAll();
        Init();
        Play();
    }

    [Button]
    public void Init()
    {
        currentTimer = 0;
        isDone = false;
    }

    [Button]
    public void Pause()
    {
        isPlaying = false;
    }


}
