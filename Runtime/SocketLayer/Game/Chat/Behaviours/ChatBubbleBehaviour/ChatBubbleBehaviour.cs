using Bloodthirst.Systems.CameraSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ChatBubbleBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform worldPosBubbleAnchor;

    [SerializeField]
    private RectTransform parentRect;

    [SerializeField]
    private RectTransform uiPosBubbleAnchor;

    [SerializeField]
    private Vector2 screenSpaceOffset;

    [SerializeField]
    private TMP_Text chatBubbleText;

    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    [Range(0, 1)]
    private float durationMultiplier;

    [SerializeField]
    [Range(0, 1)]
    private float fadeDuration;

    [SerializeField]
    private Vector2 MinMaxDuration;

    private Sequence seq;

    private void Awake()
    {
        canvasGroup.alpha = 0;
    }

    private void Update()
    {
        Vector3 viewPortPos = CameraManager.Instance.SceneCamera.WorldToScreenPoint(worldPosBubbleAnchor.position);

        Vector2 localPoint = default;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, viewPortPos, null, out localPoint);

        uiPosBubbleAnchor.localPosition = localPoint + screenSpaceOffset;
    }

    public void SetColor(Color color)
    {
        chatBubbleText.color = color;
    }

    public void Say(string message)
    {
        chatBubbleText.text = message;

        float showDuration = message.Length * durationMultiplier;

        showDuration = Mathf.Clamp(showDuration, MinMaxDuration.x, MinMaxDuration.y);

        if (seq != null && seq.IsActive())
        {
            seq.Kill();
        }

        seq = DOTween.Sequence()
            .AppendCallback(() => canvasGroup.alpha = 0)
            //.Append(canvasGroup.DOFade(1, fadeDuration))
            .AppendInterval(showDuration);
        //.Append(canvasGroup.DOFade(0, fadeDuration));

        seq.Play();



    }
}
