using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        pickable = true;

        stageManager = FindObjectOfType<StageManager>();
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
            //Follow mouse
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //play grabbed animation
        }
    }

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
        }
    }

    //Talent let go
    private void OnMouseUp()
    {
        if (pickable)
        {
            ChangeState(0);
            GetComponent<BoxCollider2D>().enabled = true;
        }
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
}
