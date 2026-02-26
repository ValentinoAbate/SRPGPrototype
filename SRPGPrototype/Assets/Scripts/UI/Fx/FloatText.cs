using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class FloatText : MonoBehaviour
{
    private const float showHideTime = 0.075f;
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private CanvasGroup group;
    public void Play(Vector2 pos, string text, Color color, float time, TweenCallback onComplete = null)
    {
        transform.position = pos;
        transform.localScale = Vector3.zero;
        textUI.text = text;
        textUI.color = color;
        group.alpha = 0;
        group.DOFade(1, showHideTime);
        transform.DOScale(1, showHideTime);
        transform.DOMoveY(transform.position.y + 65, time).SetEase(Ease.OutQuint).SetDelay(showHideTime);
        group.DOFade(0, time * 0.45f).SetEase(Ease.InCubic).SetDelay((time * 0.55f) + showHideTime).OnComplete(onComplete);
    }
}
