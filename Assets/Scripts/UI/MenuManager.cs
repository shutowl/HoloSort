using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    public enum menuState
    {
        main,
        stage,
        difficulty,
        options
    }
    public menuState state;

    public RectTransform canvasRect;
    public float canvasMoveDuration = 2f;
    Sequence DOTmoveCanvas;
    ScreenTransition screenTransition;

    bool moving;
    float movingTimer;

    private void Awake()
    {
        //AudioManager.Instance.PlayMusic("MainBGM");
        moving = false;
        movingTimer = canvasMoveDuration;
        screenTransition = FindObjectOfType<ScreenTransition>();
    }

    private void Update()
    {
        if(movingTimer >= 0)
        {
            movingTimer -= Time.unscaledDeltaTime;
        }
        else
        {
            moving = false;
        }
    }


    public void StartGame()
    {
        AudioManager.Instance.Play("Start");

        screenTransition.Zoom(true);
        screenTransition.Spin(true);
        StartCoroutine(DelayedStart(screenTransition.GetDuration()));
    }

    IEnumerator DelayedStart(float sec)
    {
        yield return new WaitForSeconds(sec);
        SceneManager.LoadScene(1);
    }

    public void OpenHowToPlay()
    {
        MoveCanvas("Up");
    }

    public void CloseHowToPlay()
    {
        MoveCanvas("Down");
    }

    public void OpenHighScores()
    {
        MoveCanvas("Down");
    }

    public void CloseHighScores()
    {
        MoveCanvas("Up");
    }

    public void MouseOverButton()
    {
        AudioManager.Instance.Play("MenuClick");
    }

    public void MoveCanvas(string direction)
    {
        if (!moving)
        {
            moving = true;
            movingTimer = canvasMoveDuration;
            string dir = direction.ToLower();

            if (dir.Equals("up"))
            {
                DOTmoveCanvas = DOTween.Sequence();
                DOTmoveCanvas.Append(canvasRect.DOMoveY(canvasRect.transform.position.y + Screen.height, canvasMoveDuration).SetEase(Ease.InOutBack));
            }
            else if (dir.Equals("down"))
            {
                DOTmoveCanvas = DOTween.Sequence();
                DOTmoveCanvas.Append(canvasRect.DOMoveY(canvasRect.transform.position.y - Screen.height, canvasMoveDuration).SetEase(Ease.InOutBack));
            }
            else
            {
                Debug.Log("Canvas Move direction invalid!");
            }
        }
    }

    private void OnDisable()
    {
        DOTween.Kill(this.gameObject);
    }
}
