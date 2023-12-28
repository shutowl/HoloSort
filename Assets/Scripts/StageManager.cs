using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public enum StageType { generation, boing, kemomimi}
    StageType stageType;        //Determines what kind of collabs (zones) will be on stage
    public float spawnRate;     //More talents will spawn every [spawnRate] seconds
    float spawnTimer;
    public float delayTime;     //The delay before more talents spawn
    float delayTimer;
    public int stageRate;       //The number of stages needed before stages change
    public int startSpawnAmount;    //The number of talents that are spawned in the first level (increases with time)
    int stagesUntilChange;
    [SerializeField] int lostTalents;            //Number of talents still on the play field
    int genLeft;
    int genRight;

    public GameObject[] talents;    //The talents in order of generation (will update in a doc if it gets bigger)
    public TextMeshProUGUI[] zoneText;
    public Transform[] spawnPoints;   //Points at where talents will spawn

    [SerializeField] int level = 0;
    [SerializeField] int stage = 0;

    [SerializeField] int score = 0;
    public TextMeshProUGUI scoreText;
    public GameObject[] zones;

    [Header("Game Over Variables")]
    public GameObject gameOverMenu;
    public TextMeshProUGUI gameOverScoreText;

    [Header("Other UI Elements")]
    public Slider timeLeftSlider;
    public GameObject themeChangeText;
    public float themeChangeDuration = 2f;
    public GameObject themeText;
    Sequence DOTThemeChange;
    Sequence DOTTheme;

    void Start()
    {
        //Initialize variables
        spawnTimer = 1f;
        stagesUntilChange = stageRate;
        scoreText.text = "Score: 0";

        gameOverMenu.SetActive(false);

        DetermineStage();
        switch (stageType)
        {
            case StageType.generation:
                themeText.GetComponent<TextMeshProUGUI>().text = "Generation?";
                break;
            case StageType.boing:
                themeText.GetComponent<TextMeshProUGUI>().text = "Cup-Size?";
                break;
            case StageType.kemomimi:
                themeText.GetComponent<TextMeshProUGUI>().text = "Animal?";
                break;
        }

        timeLeftSlider.minValue = 0;
        timeLeftSlider.maxValue = talents[0].GetComponent<Talent>().startTime;
    }

    void Update()
    {
        //Timers
        if(spawnTimer < 0 || lostTalents <= 0)
        {

            if (delayTimer <= 0)
            {
                spawnTimer = spawnRate;
                delayTimer = delayTime;

                if (stagesUntilChange == 0)
                {
                    //DetermineStage();     //done in UpdateLostTalents() now
                    stagesUntilChange = stageRate-1;
                }
                else
                {
                    stagesUntilChange--;
                }
                //Spawn more talents
                SpawnTalents(stageType);
                level++;
            }
            else
            {
                delayTimer -= Time.deltaTime;
            }
        }
        else if(stagesUntilChange != 0)
        {
            spawnTimer -= Time.deltaTime;
        }

        //Time Left UI
        if(timeLeftSlider.value > 0 && lostTalents > 0)
        {
            timeLeftSlider.value -= Time.deltaTime;
        }
    }

    //Spawn talents depending on the stage
    //Certain talents cannot spawn during certain stages because of this
    //If a talent's trait is ambiguous to the theme, they should be excluded from it too. (Like Ina or Nerissa with Animal-themed)
    //More talents will also spawn depending on level

    //Order will be based on the official site's ordering: [HoloEN] https://hololive.hololivepro.com/en/talents?gp=english
    void SpawnTalents(StageType stage)
    {
        List<GameObject> validTalents = new();
        switch (stage)
        {
            case StageType.generation:
                if(genLeft == 0 || genRight == 0)   //Myth
                {
                    validTalents.Add(talents[0]);   //Calli
                    validTalents.Add(talents[1]);   //Kiara
                    validTalents.Add(talents[2]);   //Ina
                    validTalents.Add(talents[3]);   //Gura
                    validTalents.Add(talents[4]);   //Ame
                }
                if(genLeft == 1 || genRight == 1)   //Promise
                {
                    validTalents.Add(talents[5]);   //Irys
                    validTalents.Add(talents[6]);   //Fauna
                    validTalents.Add(talents[7]);   //Kronii
                    validTalents.Add(talents[8]);   //Mumei
                    validTalents.Add(talents[9]);   //Bae
                }
                if(genLeft == 2 || genRight == 2)
                {
                    validTalents.Add(talents[10]);   //Shiori
                    validTalents.Add(talents[11]);   //Biboo
                    validTalents.Add(talents[12]);   //Nerissa
                    validTalents.Add(talents[13]);   //Fuwawa
                    validTalents.Add(talents[14]);   //Mococo
                }

                for(int i = 0; i < startSpawnAmount; i++)
                {
                    GameObject talent = Instantiate(validTalents[Random.Range(0, validTalents.Count)], spawnPoints[0].position, Quaternion.identity);
                    talent.GetComponent<Talent>().SetZ(Random.Range(0, 50f));
                    lostTalents++;
                }
                zones[0].GetComponent<Zone>().SetStage(0);
                zones[1].GetComponent<Zone>().SetStage(0);
                break;
            case StageType.boing:
                for(int i = 0; i < talents.Length; i++)
                {
                    validTalents.Add(talents[i]);   //Add all talents
                }

                for (int i = 0; i < startSpawnAmount; i++)
                {
                    GameObject talent = Instantiate(validTalents[Random.Range(0, validTalents.Count)], spawnPoints[0].position, Quaternion.identity);
                    talent.GetComponent<Talent>().SetZ(Random.Range(0, 50f));
                    lostTalents++;
                }
                zones[0].GetComponent<Zone>().SetStage(1);
                zones[1].GetComponent<Zone>().SetStage(1);
                break;
            case StageType.kemomimi:
                validTalents.Add(talents[0]);   //Calli
                validTalents.Add(talents[1]);   //Kiara
                //validTalents.Add(talents[2]);   //Ina     >Ambiguous between tako or human
                validTalents.Add(talents[3]);   //Gura
                validTalents.Add(talents[4]);   //Ame

                validTalents.Add(talents[5]);   //Irys
                validTalents.Add(talents[6]);   //Fauna
                validTalents.Add(talents[7]);   //Kronii
                validTalents.Add(talents[8]);   //Mumei
                validTalents.Add(talents[9]);   //Bae

                validTalents.Add(talents[10]);   //Shiori
                validTalents.Add(talents[11]);   //Biboo
                //validTalents.Add(talents[12]);   //Nerissa    >Ambiguous between raven or demon
                validTalents.Add(talents[13]);   //Fuwawa
                validTalents.Add(talents[14]);   //Mococo

                for (int i = 0; i < startSpawnAmount; i++)
                {
                    GameObject talent = Instantiate(validTalents[Random.Range(0, validTalents.Count)], spawnPoints[0].position, Quaternion.identity);
                    talent.GetComponent<Talent>().SetZ(Random.Range(0, 50f));
                    lostTalents++;
                }
                zones[0].GetComponent<Zone>().SetStage(2);
                zones[1].GetComponent<Zone>().SetStage(2);
                break;
        }
    }

    void DetermineStage()
    {
        stage++;
        //Determine stage
        stageType = (StageType)Random.Range(0, 3);  //Will change depending on number of stage types
        switch (stageType)
        {
            case StageType.generation:
                genLeft = Random.Range(0, 3);   //Will change depending on number of gens
                do
                {
                    genRight = Random.Range(0, 3);  //Same as above
                } while (genRight == genLeft);      //Repeat until both are different

                switch (genLeft)
                {
                    case 0:
                        zoneText[0].text = "Myth";
                        zones[0].GetComponent<Zone>().SetGen(0);
                        break;
                    case 1:
                        zoneText[0].text = "Promise";
                        zones[0].GetComponent<Zone>().SetGen(1);
                        break;
                    case 2:
                        zoneText[0].text = "Advent";
                        zones[0].GetComponent<Zone>().SetGen(2);
                        break;
                }
                switch (genRight)
                {
                    case 0:
                        zoneText[1].text = "Myth";
                        zones[1].GetComponent<Zone>().SetGen(0);
                        break;
                    case 1:
                        zoneText[1].text = "Promise";
                        zones[1].GetComponent<Zone>().SetGen(1);
                        break;
                    case 2:
                        zoneText[1].text = "Advent";
                        zones[1].GetComponent<Zone>().SetGen(2);
                        break;
                }
                break;

            case StageType.boing:
                int boingLeft = Random.Range(0, 2);         //Done to randomize which side it'll appear on (0 = boing)
                int boingRight = (boingLeft == 0) ? 1 : 0;  //Make sure both are different

                zoneText[0].text = (boingLeft == 0) ? "Boing Boing" : "Pettan...";
                zones[0].GetComponent<Zone>().SetBoing((boingLeft == 0));
                zoneText[1].text = (boingRight == 0) ? "Boing Boing" : "Pettan...";
                zones[1].GetComponent<Zone>().SetBoing((boingRight == 0));
                break;

            case StageType.kemomimi:
                int mimiLeft = Random.Range(0, 2);         //Same as with boing
                int mimiRight = (mimiLeft == 0) ? 1 : 0;

                zoneText[0].text = (mimiLeft == 0) ? "Animal-themed!" : "Human";
                zones[0].GetComponent<Zone>().SetKemomimi((mimiLeft == 0));
                zoneText[1].text = (mimiRight == 0) ? "Animal-themed!" : "Human";
                zones[1].GetComponent<Zone>().SetKemomimi((mimiRight == 0));
                break;
        }//end of stage initialization

        //Difficulty scaling (manual for now lol)
        switch (stage)
        {
            //Start:
            /* spawnRate = 5
             * delayTime = 1
             * stageRate = 4
             * startSpawnAmmount = 2
             */
            case 2:
                spawnRate -= 1f;    // 4
                stageRate = 3;      // 3
                break;
            case 3:
                spawnRate += 1f;        //5
                stageRate = 4;
                startSpawnAmount += 1;  //3
                break;
            default:    //nvm i don't wanna make one for every stage lmao
                spawnRate = Mathf.Clamp(spawnRate - 0.2f, 1f, 10);
                stageRate = Random.Range(3, 6);
                if(stage % 2 == 0) startSpawnAmount = Mathf.Clamp(startSpawnAmount + 1, 2, 7);  //increase by 1 every even number stage
                break;
        }
    }

    public void UpdateScore(int points)
    {
        //Maybe play an animation too
        if(points > 0)
        {
            score += points;
            scoreText.text = "Score: " + score;
        }
        else
        {
            gameOverMenu.SetActive(true);
            gameOverScoreText.text = "Score: " + score;
            Time.timeScale = 0;
        }
    }

    public void UpdateLostTalents(int num)
    {
        lostTalents += num;

        if(lostTalents == 0 && stagesUntilChange == 0)
        {
            //Show theme change warning
            RectTransform themeChangeRect = themeChangeText.GetComponent<RectTransform>();
            DOTThemeChange = DOTween.Sequence();
            DOTThemeChange.Append(themeChangeRect.DOAnchorPosY(Mathf.Abs(themeChangeRect.anchoredPosition.y), themeChangeDuration / 2).SetEase(Ease.OutCubic));
            DOTThemeChange.Append(themeChangeRect.DOAnchorPosY(-Mathf.Abs(themeChangeRect.anchoredPosition.y), themeChangeDuration / 2).SetEase(Ease.InOutCubic));
            delayTimer = themeChangeDuration + (delayTime/2);

            //Theme text
            DOTTheme = DOTween.Sequence();
            RectTransform themeRect = themeText.GetComponent<RectTransform>();
            DOTTheme.Append(themeRect.DOAnchorPosY(3f, themeChangeDuration / 2).SetEase(Ease.OutCubic));
            DOTTheme.Append(themeRect.DOAnchorPosY(0f, themeChangeDuration / 2).SetEase(Ease.InOutCubic));

            StartCoroutine(DelayedTextChange(themeChangeDuration / 2));

            DetermineStage();
            //Spin text
            zoneText[0].GetComponentInParent<RectTransform>().DORotate(new Vector3(0, 0, 1080), 1.5f, RotateMode.FastBeyond360).SetEase(Ease.OutQuint);
            zoneText[1].GetComponentInParent<RectTransform>().DORotate(new Vector3(0, 0, -1080), 1.5f, RotateMode.FastBeyond360).SetEase(Ease.OutQuint);
        }

        //Time remaining UI
        var foundTalents = GameObject.FindGameObjectsWithTag("Talent");

        float min = talents[0].GetComponent<Talent>().startTime;
        for (int i = 0; i < foundTalents.Length; i++) //get min value from scorable talents
        {
            if (foundTalents[i].GetComponent<Talent>().GetTimeLeft() < min)
            {
                min = foundTalents[i].GetComponent<Talent>().GetTimeLeft();
            }
            Debug.Log("Checked: " + foundTalents[i].GetComponent<Talent>().GetTimeLeft());
        }
        timeLeftSlider.value = min;
        Debug.Log("Count: " + foundTalents.Length);
        Debug.Log("Min: " + min);
    }

    IEnumerator DelayedTextChange(float time)
    {
        yield return new WaitForSeconds(time);

        switch (stageType)
        {
            case StageType.generation:
                themeText.GetComponent<TextMeshProUGUI>().text = "Generation?";
                break;
            case StageType.boing:
                themeText.GetComponent<TextMeshProUGUI>().text = "Cup-Size?";
                break;
            case StageType.kemomimi:
                themeText.GetComponent<TextMeshProUGUI>().text = "Animal?";
                break;
        }
    }

    public StageType GetStageType()
    {
        return stageType;
    }

    public void ReturnToMainMenu()
    {
        PlayerPrefs.SetInt("score", score);
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    private void OnDisable()
    {
        DOTween.Kill(this.gameObject);
    }
}
