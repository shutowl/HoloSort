using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenTransition : MonoBehaviour
{
    RectTransform logoRect;
    Sequence DOTzoom;
    Sequence DOTspin;
    public float transitionDuration = 2f;


    // Start is called before the first frame update
    void Start()
    {
        DOTzoom = DOTween.Sequence();
        DOTspin = DOTween.Sequence();
        logoRect = GetComponent<RectTransform>();

        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            //Zoom out and Spin backwards
            logoRect.rotation = Quaternion.Euler(Vector3.zero);
            Zoom(false);
            Spin(false);
        }
    }


    public void Zoom(bool zoomIn)
    {
        if (zoomIn)
        {
            DOTzoom.Append(logoRect.DOScale(7f, transitionDuration).SetEase(Ease.InOutCubic));
        }
        else
        {
            DOTzoom.Append(logoRect.DOScale(0f, transitionDuration).SetEase(Ease.InOutCubic));
        }
        DOTzoom.SetUpdate(true);
        DOTzoom.Play();
    }

    public void Spin(bool clockwise)
    {
        if (clockwise)
        {
            DOTspin.Append(logoRect.DOLocalRotate(new Vector3(0, 0, -1800f), transitionDuration, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic));
        }
        else
        {
            DOTspin.Append(logoRect.DOLocalRotate(new Vector3(0, 0, 1800f), transitionDuration, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic));
        }
        DOTspin.SetUpdate(true);
        DOTspin.Play();
    }

    public float GetDuration()
    {
        return transitionDuration;
    }

    private void OnDisable()
    {
        DOTween.Kill(this.gameObject);
    }
}
