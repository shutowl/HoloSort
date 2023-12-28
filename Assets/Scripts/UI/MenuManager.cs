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
    public RectTransform optionsRect;
    public Slider BGMSlider;
    public Slider SFXSlider;
    float BGMVol;
    float SFXVol;

    public List<int> highscores = new List<int>(5);
    public TextMeshProUGUI highscoreText;

    ScreenTransition screenTransition;

    Sequence DOTmoveOptions;
    Sequence DOTmoveCanvas;

    bool moving;
    bool optionsOpen;
    float movingTimer;

    private void Awake()
    {
        //AudioManager.Instance.PlayMusic("MainBGM");
        CursorManager.Instance.ChangeCursor("Default");
        moving = false;
        optionsOpen = false;
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
        CursorManager.Instance.ChangeCursor("Open");

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
        CursorManager.Instance.ChangeCursor("Default");
    }

    public void ClickOptions()
    {
        if (!optionsOpen)
        {
            OpenOptions();
        }
        else
        {
            CloseOptions();
        }
    }

    public void OpenOptions()
    {
        optionsOpen = true;
        MoveOptions("Left");
    }

    public void CloseOptions()
    {
        optionsOpen = false;
        MoveOptions("Right");
    }

    public void ChangeBGMVol(float vol)
    {
        BGMVol = vol;
        AudioManager.Instance.ChangeBGMVolume(vol);
    }

    public void ChangeSFXVol(float vol)
    {
        SFXVol = vol;
        AudioManager.Instance.ChangeSFXVolume(vol);
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

    public void MoveOptions(string direction)
    {
        moving = true;
        movingTimer = canvasMoveDuration;
        string dir = direction.ToLower();

        if (dir.Equals("left"))
        {
            DOTmoveOptions = DOTween.Sequence();
            DOTmoveOptions.Append(optionsRect.DOAnchorPosX(-323.58f, canvasMoveDuration/4).SetEase(Ease.OutBack));
        }
        else if (dir.Equals("right"))
        {
            DOTmoveOptions = DOTween.Sequence();
            DOTmoveOptions.Append(optionsRect.DOAnchorPosX(323.58f, canvasMoveDuration/4).SetEase(Ease.OutBack));
        }
        else
        {
            Debug.Log("Canvas Move direction invalid!");
        }
    }

    void UpdateHighScores()
    {
        highscores[0] = PlayerPrefs.GetInt("highscore0");
        highscores[1] = PlayerPrefs.GetInt("highscore1");
        highscores[2] = PlayerPrefs.GetInt("highscore2");
        highscores[3] = PlayerPrefs.GetInt("highscore3");
        highscores[4] = PlayerPrefs.GetInt("highscore4");

        int recentScore = PlayerPrefs.GetInt("score");
        //Debug.Log("Recent: " + recentScore);

        for(int i = 0; i < highscores.Count; i++)
        {
            if(recentScore > highscores[i])
            {
                //Debug.Log("Compared: " + recentScore + ">" + highscores[i]);
                highscores.Insert(i, recentScore);
                highscores.RemoveAt(highscores.Count-1);

                break;
            }
            else if(recentScore == highscores[i])
            {
                break;
            }
        }

        highscoreText.text = "";

        for(int i = 0; i < highscores.Count; i++)
        {
            highscoreText.text += (i+1) + " - " + highscores[i].ToString("D4") + "\n";
        }

        string debugScore = "";
        foreach (int score in highscores)
        {
            debugScore += score + " ";
        }
        //Debug.Log(debugScore);

        PlayerPrefs.SetInt("highscore0", highscores[0]);
        PlayerPrefs.SetInt("highscore1", highscores[1]);
        PlayerPrefs.SetInt("highscore2", highscores[2]);
        PlayerPrefs.SetInt("highscore3", highscores[3]);
        PlayerPrefs.SetInt("highscore4", highscores[4]);
    }

    private void OnEnable()
    {
        BGMVol = PlayerPrefs.GetFloat("bgmVol", 3f);
        SFXVol = PlayerPrefs.GetFloat("sfxVol", 30f);
        BGMSlider.value = BGMVol;
        SFXSlider.value = SFXVol;

        AudioManager.Instance.ChangeBGMVolume(BGMVol);
        AudioManager.Instance.ChangeSFXVolume(SFXVol);

        UpdateHighScores();
    }

    private void OnDisable()
    {
        DOTween.Kill(this.gameObject);

        PlayerPrefs.SetFloat("bgmVol", BGMVol);
        PlayerPrefs.SetFloat("sfxVol", SFXVol);
    }
}
