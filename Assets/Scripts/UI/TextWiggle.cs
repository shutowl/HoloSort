using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class TextWiggle : MonoBehaviour
{
    public float duration = 3f;
    public float angle = 5f;

    public GameObject textBox;
    Sequence DOTwiggle;

    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();

        DOTwiggle = DOTween.Sequence();
        DOTwiggle.Append(rect.DORotate(new Vector3(0, 0, -angle), duration / 4).SetEase(Ease.OutSine));
        DOTwiggle.Append(rect.DORotate(new Vector3(0, 0, angle), duration / 2).SetEase(Ease.InOutSine));
        DOTwiggle.Append(rect.DORotate(new Vector3(0, 0, 0), duration / 4).SetEase(Ease.InSine));
        DOTwiggle.SetLoops(-1).SetUpdate(true);
    }

    private void OnDisable()
    {
        DOTween.Kill(this.gameObject);
    }
}
