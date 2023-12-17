using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

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


    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenHowToPlay()
    {

    }

    public void CloseHowToPlay()
    {

    }
}
