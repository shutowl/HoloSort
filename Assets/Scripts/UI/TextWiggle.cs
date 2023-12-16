using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class TextWiggle : MonoBehaviour
{
    public float duration = 3f;
    public float startAngle = 5f;

    public GameObject textBox;

    void Start()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.rotation = Quaternion.Euler(0f, 0f, startAngle);
        rect.DORotate(new Vector3(0, 0, -startAngle), duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

}
