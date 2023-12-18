using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class StageManager : MonoBehaviour
{
    public enum StageType { generation, boing, kemomimi}
    StageType stageType;        //Determines what kind of collabs (zones) will be on stage
    public float spawnRate;     //Rate at which more talents spawn (may lower as level increases)
    float spawnTimer;
    public float delayTime;     //The delay before more talents spawn
    float delayTimer;
    public int stageRate;       //The number of stages needed before stages change
    int stagesUntilChange;
    [SerializeField] int lostTalents;            //Number of talents still on the play field
    int genLeft;
    int genRight;

    public GameObject[] talents;    //The talents in order of generation (will update in a doc if it gets bigger)
    public TextMeshProUGUI[] zoneText;
    public Transform[] spawnPoints;   //Points at where talents will spawn

    [SerializeField] int level = 0;

    [SerializeField] int score = 0;
    public TextMeshProUGUI scoreText;
    public GameObject[] zones;

    void Start()
    {
        //Initialize variables
        spawnTimer = 1f;
        stagesUntilChange = stageRate;
        scoreText.text = "Score: 0";

        //Start Text animations

        DetermineStage();
    }

    void Update()
    {
        //Spawn Logic


        //Timer
        if(spawnTimer < 0 || lostTalents <= 0)
        {

            if (delayTimer <= 0)
            {
                spawnTimer = spawnRate;
                delayTimer = delayTime;

                if (stagesUntilChange == 0)
                {
                    DetermineStage();
                    stagesUntilChange = stageRate;
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
        else
        {
            spawnTimer -= Time.deltaTime;
        }
    }

    //Spawn talents depending on the stage
    //Certain talents cannot spawn during certain stages because of this
    //More talents will also spawn depending on level
    void SpawnTalents(StageType stage)
    {
        List<GameObject> validTalents = new();
        switch (stage)
        {
            case StageType.generation:
                if(genLeft == 0 || genRight == 0)   //Myth
                {
                    validTalents.Add(talents[0]);   //Gura
                }
                if(genLeft == 1 || genRight == 1)   //Promise
                {
                    validTalents.Add(talents[1]);   //Bae
                }
                if(genLeft == 2 || genRight == 2)
                {
                    validTalents.Add(talents[2]);   //Mococo
                }

                for(int i = 0; i < 3; i++)
                {
                    GameObject talent = Instantiate(validTalents[Random.Range(0, validTalents.Count)], spawnPoints[0].position, Quaternion.identity);
                    talent.GetComponent<Talent>().SetZ(Random.Range(0, 50f));
                    lostTalents++;
                }
                zones[0].GetComponent<Zone>().SetStage(0);
                zones[1].GetComponent<Zone>().SetStage(0);
                break;
            case StageType.boing:
                validTalents.Add(talents[0]);   //Gura
                validTalents.Add(talents[1]);   //Bae
                validTalents.Add(talents[2]);   //Mococo

                for (int i = 0; i < 3; i++)
                {
                    GameObject talent = Instantiate(validTalents[Random.Range(0, validTalents.Count)], spawnPoints[0].position, Quaternion.identity);
                    talent.GetComponent<Talent>().SetZ(Random.Range(0, 50f));
                    lostTalents++;
                }
                zones[0].GetComponent<Zone>().SetStage(1);
                zones[1].GetComponent<Zone>().SetStage(1);
                break;
            case StageType.kemomimi:
                validTalents.Add(talents[0]);   //Gura
                validTalents.Add(talents[1]);   //Bae
                validTalents.Add(talents[2]);   //Mococo

                for (int i = 0; i < 3; i++)
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
                int boingLeft = Random.Range(0, 1);         //Done to randomize which side it'll appear on (0 = boing)
                int boingRight = (boingLeft == 0) ? 1 : 0;  //Make sure both are different

                zoneText[0].text = (boingLeft == 0) ? "Boing Boing" : "Pettan...";
                zones[0].GetComponent<Zone>().SetBoing((boingLeft == 0) ? true : false);
                zoneText[1].text = (boingRight == 0) ? "Boing Boing" : "Pettan...";
                zones[1].GetComponent<Zone>().SetBoing((boingRight == 0) ? true : false);
                break;

            case StageType.kemomimi:
                int mimiLeft = Random.Range(0, 1);         //Same as with boing
                int mimiRight = (mimiLeft == 0) ? 1 : 0;

                zoneText[0].text = (mimiLeft == 0) ? "Animal Ears!" : "Human";
                zones[0].GetComponent<Zone>().SetKemomimi((mimiLeft == 0) ? true : false);
                zoneText[1].text = (mimiRight == 0) ? "Animal Ears!" : "Human";
                zones[1].GetComponent<Zone>().SetKemomimi((mimiRight == 0) ? true : false);
                break;
        }//end of stage initialization

         //Spin text
        zoneText[0].GetComponentInParent<RectTransform>().DORotate(new Vector3(0, 0, 1080), 1f, RotateMode.FastBeyond360).SetEase(Ease.InOutBack);
        zoneText[1].GetComponentInParent<RectTransform>().DORotate(new Vector3(0, 0, 1080), 1f, RotateMode.FastBeyond360).SetEase(Ease.InOutBack);

    }

    public void UpdateScore(int points)
    {
        //Maybe play an animation too
        score += points;
        scoreText.text = "Score: " + this.score;
    }

    public void UpdateLostTalents(int num)
    {
        lostTalents += num;
    }

    public StageType GetStageType()
    {
        return stageType;
    }
}
