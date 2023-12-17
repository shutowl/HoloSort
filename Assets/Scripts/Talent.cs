using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  //Documentation: https://dotween.demigiant.com/documentation.php
using UnityEngine.SceneManagement;

public class Talent : MonoBehaviour
{
    enum State
    {
        moving,
        grabbed
    }
    enum Generation { myth, promise, advent }

    [SerializeField] State state;
    [Header("Main Variables")]
    public float startTime = 10f;     //initialize "fuse" time
    float timer;
    float speed;
    public float maxSpeed = 3f;
    public float minSpeed = 5f;
    bool pickable = true;               //determines whether they can be picked up with mouse inputs
    public float wiggleAngle = 5f;      //When held, determines how far talent wiggles
    public float wiggleDuration = 1f;   //When held, determines how long each wiggle cycle is
    Sequence DOTwiggle;

    float stuckTime = 0.1f;             //If talent gets stuck in the wall, reset them
    float stuckTimer;
    bool visible;

    Sprite walkSprite;
    Sprite heldSprite;

    [Header("Filters")]
    [SerializeField] Generation gen;
    public bool kemomimi = false;
    public bool boing = false;

    Vector3 startDirection;
    Vector3 curDirection;

    StageManager stageManager;

    void Start()
    {
        startDirection = new Vector3(Random.Range(-4f, 4f), -5f).normalized;
        curDirection = startDirection;
        speed = Random.Range(minSpeed, maxSpeed);
        state = State.moving;
        timer = startTime;
        stuckTimer = stuckTime;
        pickable = true;

        stageManager = FindObjectOfType<StageManager>();

        DOTwiggle = DOTween.Sequence();
        DOTwiggle.Append(transform.DORotate(new Vector3(0, 0, -wiggleAngle), wiggleDuration / 4).SetEase(Ease.OutSine));
        DOTwiggle.Append(transform.DORotate(new Vector3(0, 0, wiggleAngle), wiggleDuration / 2).SetEase(Ease.InOutSine));
        DOTwiggle.Append(transform.DORotate(new Vector3(0, 0, 0), wiggleDuration / 4).SetEase(Ease.InSine));
        DOTwiggle.SetLoops(-1);
        DOTwiggle.Pause();
    }

    void Update()
    {
        //Timer
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            //Lose animations
            DOTwiggle.Kill();
            Destroy(this.gameObject); //temporary destroy
        }
    }

    void FixedUpdate()
    {
        if(state == State.moving)
        {
            transform.position += speed * Time.deltaTime * curDirection;
        }
        if(state == State.grabbed)
        {
            //Smoothly follow mouse
            Vector3 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 smoothedPos = Vector3.Lerp(transform.position, mousePos, 15f * Time.deltaTime);
            transform.position = smoothedPos;
        }

        //Check if talent offscreen
        if (!visible && SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (stuckTimer >= 0)
            {
                stuckTimer -= Time.deltaTime;
            }
            else
            {
                stuckTimer = stuckTime;
                transform.position = Vector2.zero;
            }
        }
    }

    private void OnBecameVisible() { visible = true; }
    private void OnBecameInvisible() { visible = false; }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Wall"))
        {
            // Reflect the movement direction when hitting an obstacle
            Reflect(col.contacts[0].normal);
        }
    }

    private void Reflect(Vector2 normal)
    {
        // Reflect the movement direction based on the obstacle's normal
        curDirection = Vector2.Reflect(curDirection, normal).normalized;
    }


    public void ChangeState(int state)
    {
        this.state = (State)state;

        if (state == 1) //put back down
        {
            //Check if in correct zone

            //Move in random direction
            curDirection = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f)).normalized;
        }
    }

    //Talent grabbed
    private void OnMouseDown()
    {
        if (pickable)
        {
            ChangeState(1);
            GetComponent<BoxCollider2D>().enabled = false;

            //Talent wiggles a bit while held
            DOTwiggle.Play();
        }
    }

    //Talent let go
    private void OnMouseUp()
    {
        if (pickable)
        {
            ChangeState(0);
            GetComponent<BoxCollider2D>().enabled = true;

            //Reset rotation from wiggles
            transform.rotation = Quaternion.Euler(Vector3.zero);
            DOTwiggle.Restart();
            DOTwiggle.Pause();
        }
    }

    //Change Layer Order number
    public void SetZ(float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Zone"))
        {
            Zone zone = col.GetComponent<Zone>();

            switch (zone.GetStageType())
            {
                case 0: //Gen
                    if (zone.GetGenType() == (int)gen)
                    {
                        stageManager.UpdateScore(1);
                    }
                    else
                    {
                        stageManager.UpdateScore(-1);
                    }
                    stageManager.UpdateLostTalents(-1);
                    break;
                case 1: //Boing
                    if (zone.GetBoing() == boing)
                    {
                        stageManager.UpdateScore(1);
                    }
                    else
                    {
                        stageManager.UpdateScore(-1);
                    }
                    stageManager.UpdateLostTalents(-1);
                    break;
                case 2: //kemomimi
                    if (zone.GetKemomimi() == kemomimi)
                    {
                        stageManager.UpdateScore(1);
                    }
                    else
                    {
                        stageManager.UpdateScore(-1);
                    }
                    stageManager.UpdateLostTalents(-1);
                    break;
            }
            pickable = false;
        }

        //Destroy(this.gameObject);   //temporary destroy
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (col.CompareTag("Wall"))
            {
                if (stuckTimer >= 0)
                {
                    stuckTimer -= Time.deltaTime;
                }
                else
                {
                    stuckTimer = stuckTime;

                    if (pickable)                       //Stuck outside of zones
                    {
                        transform.position = Vector2.zero;
                    }
                    else if (transform.position.x > 0)   //Stuck in right zone
                    {
                        transform.position = new Vector2(11.5f, 0);
                    }
                    else                                //Stuck in left zone
                    {
                        transform.position = new Vector2(-11.5f, 0);
                    }

                }
            }
        }
    }


}
