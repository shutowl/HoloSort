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
    public float zoneTime = 20f;      //Amount of time talents wander around the zone
    float timer;
    float speed;
    public float maxSpeed = 3f;
    public float minSpeed = 5f;
    bool scorable = true;               //attempts to fix a bug where 1 talent can be scored twice
    bool pickable = true;               //determines whether they can be picked up with mouse inputs
    public float wiggleAngle = 5f;      //When held, determines how far talent wiggles
    public float wiggleDuration = 1f;   //When held, determines how long each wiggle cycle is
    public float walkAngle = 3f;
    public float walkDuration = 0.5f;
    public float walkDistance = 0.3f;
    Sequence DOTwiggle;

    //Highlight
    private Color highlightColor;

    bool warning = false;
    public float warningTime = 2f;          //The warning will pop after when there's only this much time left
    public float warningFlashRate = 0.2f;   //The warning indicator will flash every X seconds.
    public GameObject warningIndicator;
    Sequence DOTwarning;

    public GameObject sprite;
    public Sprite walkSprite;
    public Sprite heldSprite;
    Sequence DOTwalk;


    float stuckTime = 0.1f;             //If talent gets stuck in the wall, reset them
    float stuckTimer;
    bool visible;

    Vector3 startDirection;
    Vector3 curDirection;
    public bool startRandomDirection = false;

    [Header("Filters")]
    [SerializeField] Generation gen;
    public bool kemomimi = false;
    public bool boing = false;

    StageManager stageManager;


    void Start()
    {
        SetZ(Random.Range(-0f, 50f));
        highlightColor = new Color(255f/255f, 235f/255f, 122f/255f, 1f);
        startDirection = new Vector3(Random.Range(-4f, 4f), -5f).normalized;
        curDirection = startDirection;
        if(startRandomDirection) { curDirection = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f)).normalized; }
        CheckDirection();

        speed = Random.Range(minSpeed, maxSpeed);
        state = State.moving;
        timer = startTime;
        stuckTimer = stuckTime;
        pickable = true;
        warningIndicator.SetActive(false);
        warning = false;

        stageManager = FindObjectOfType<StageManager>();

        DOTwiggle = DOTween.Sequence();
        DOTwiggle.Append(transform.DORotate(new Vector3(0, 0, -wiggleAngle), wiggleDuration / 4).SetEase(Ease.OutSine));
        DOTwiggle.Append(transform.DORotate(new Vector3(0, 0, wiggleAngle), wiggleDuration / 2).SetEase(Ease.InOutSine));
        DOTwiggle.Append(transform.DORotate(new Vector3(0, 0, 0), wiggleDuration / 4).SetEase(Ease.InSine));
        DOTwiggle.SetLoops(-1);
        DOTwiggle.Pause();

        DOTwalk = DOTween.Sequence();
        DOTwalk.Append(sprite.transform.DORotate(new Vector3(0, 0, -walkAngle), walkDuration/4).SetEase(Ease.OutCubic));
            DOTwalk.Join(sprite.transform.DOLocalMoveY(-walkDistance, walkDuration / 4).SetEase(Ease.OutSine));
        DOTwalk.Append(sprite.transform.DORotate(new Vector3(0, 0, 0), walkDuration / 4).SetEase(Ease.Linear));
            DOTwalk.Join(sprite.transform.DOLocalMoveY(0f, walkDuration / 4).SetEase(Ease.Linear));
        DOTwalk.Append(sprite.transform.DORotate(new Vector3(0, 0, walkAngle), walkDuration / 4).SetEase(Ease.OutCubic));
            DOTwalk.Join(sprite.transform.DOLocalMoveY(-walkDistance, walkDuration / 4).SetEase(Ease.OutSine));
        DOTwalk.Append(sprite.transform.DORotate(new Vector3(0, 0, 0), walkDuration / 4).SetEase(Ease.Linear));
            DOTwalk.Join(sprite.transform.DOLocalMoveY(0f, walkDuration / 4).SetEase(Ease.Linear));
        DOTwalk.SetLoops(-1);

        DOTwarning = DOTween.Sequence();
        DOTwarning.Append(sprite.GetComponent<SpriteRenderer>().DOColor(Color.red, warningTime)).SetEase(Ease.InFlash, 12);
        DOTwarning.SetUpdate(true);
        DOTwarning.Pause();
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
            if (scorable) //Talent runs out of time in playfield
            { 
                stageManager.UpdateScore(-1);
                TalentPlaced(false);
            } 
            else //Talent runs out of time in zone
            {
                sprite.GetComponent<SpriteRenderer>().color = Color.white;
                Destroy(this.gameObject, 1f);
            }
        }

        if(timer <= warningTime && !warning)
        {
            DisplayWarning(true);
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
        CheckDirection();
    }

    void DisplayWarning(bool on)
    {
        warning = true;
        //warningIndicator.SetActive(on);
        /*
        float warningAngle = 20f;
        float warningDuration = 0.5f;

        DOTwarning.Append(warningIndicator.transform.DORotate(new Vector3(0, 0, -warningAngle), warningDuration / 4).SetEase(Ease.OutSine));
        DOTwarning.Append(warningIndicator.transform.DORotate(new Vector3(0, 0, warningAngle), warningDuration / 2).SetEase(Ease.InOutSine));
        DOTwarning.Append(warningIndicator.transform.DORotate(new Vector3(0, 0, 0), warningDuration / 4).SetEase(Ease.InSine));
        DOTwarning.SetLoops(-1);
        */

        //Make warning flash?
        //DOTwarning.Append(sprite.GetComponent<SpriteRenderer>().DOColor(Color.white, warningFlashRate)).SetEase(Ease.Flash, 1, 0);
        //DOTwarning.SetLoops(-1);

        if(on) DOTwarning.Play();
        else
        {
            DOTwarning.Restart();
            DOTwarning.Kill();
        }
    }


    public void ChangeState(int state)
    {
        this.state = (State)state;

        if (state == 1) //pick up
        {
            //Move in random direction
            curDirection = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f)).normalized;
        }
        else
        {
            CheckDirection();
        }
    }

    //Flips the sprite if going a certain direction
    void CheckDirection()
    {
        if (curDirection.x <= 0) { sprite.GetComponent<SpriteRenderer>().flipX = false; }
        else { sprite.GetComponent<SpriteRenderer>().flipX = true; }
    }

    //Talent grabbed
    private void OnMouseDown()
    {
        if (pickable)
        {
            ChangeState(1);
            GetComponent<BoxCollider2D>().enabled = false;
            sprite.GetComponent<SpriteRenderer>().sprite = heldSprite;

            //Talent wiggles a bit while held
            DOTwiggle.Play();

            DOTwalk.Restart();
            DOTwalk.Pause();

            AudioManager.Instance.Play("Grab");
            CursorManager.Instance.ChangeCursor("Hold");
        }
    }

    //Talent let go
    private void OnMouseUp()
    {
        if (pickable)
        {
            ChangeState(0);
            GetComponent<BoxCollider2D>().enabled = true;
            sprite.GetComponent<SpriteRenderer>().sprite = walkSprite;

            //Reset rotation from wiggles
            transform.rotation = Quaternion.Euler(Vector3.zero);

            //Walk animation
            DOTwalk.Play();

            DOTwiggle.Restart();
            DOTwiggle.Pause();

            CursorManager.Instance.ChangeCursor("Open");
        }
    }

    private void OnMouseEnter()
    {
        if (pickable)
        {
            //Highlight or Outline talent
            sprite.GetComponent<SpriteRenderer>().color = highlightColor;
        }
    }

    private void OnMouseExit()
    {
        if (pickable)
        {
            //Remove highlight or outline
            sprite.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    //Change Layer Order number
    public void SetZ(float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }

    public float GetTimeLeft()
    {
        return timer;
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Zone") && scorable)
        {
            Zone zone = col.GetComponent<Zone>();

            switch (zone.GetStageType())
            {
                case 0: //Gen
                    TalentPlaced(zone.GetGenType() == (int)gen);
                    break;
                case 1: //Boing
                    TalentPlaced(zone.GetBoing() == boing);
                    break;
                case 2: //kemomimi
                    TalentPlaced(zone.GetKemomimi() == kemomimi);
                    break;
            }
            pickable = false;
            scorable = false;
            DisplayWarning(false);
        }
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
                        transform.position = new Vector2(13.5f, 0);
                    }
                    else                                //Stuck in left zone
                    {
                        transform.position = new Vector2(-13.5f, 0);
                    }
                    sprite.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }
    }

    void TalentPlaced(bool correct)
    {
        scorable = false;
        Sequence DOTwiggleFast = DOTween.Sequence();
        //sprite.GetComponent<SpriteRenderer>().color = Color.white;

        if (correct)
        {
            stageManager.UpdateScore(1);
            timer = zoneTime;
        }
        else
        {
            stageManager.UpdateScore(-1);
            wiggleDuration = 0.5f;
            DOTwiggleFast.Append(transform.DORotate(new Vector3(0, 0, -wiggleAngle), wiggleDuration / 4).SetEase(Ease.OutSine));
            DOTwiggleFast.Append(transform.DORotate(new Vector3(0, 0, wiggleAngle), wiggleDuration / 2).SetEase(Ease.InOutSine));
            DOTwiggleFast.Append(transform.DORotate(new Vector3(0, 0, 0), wiggleDuration / 4).SetEase(Ease.InSine));
            DOTwiggleFast.SetLoops(-1).SetUpdate(true);

            SetZ(-5f);  //Show above every other talent
        }

        transform.gameObject.tag = "TalentScored";
        stageManager.UpdateLostTalents(-1);
        sprite.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void OnDisable()
    {
        DOTween.Kill(this.gameObject);
    }
}
