using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D myRB;
    float mSpeed = 120f;
    Vector3 curPos, lastPos;
    float xAxis = 0f;
    float yAxis = 0f;

    //for animations
    Animator animator;
    string currAnimState;
    //animation states
    const string PLAYER_IDLE = "Player_idle";
    const string PLAYER_WALK = "Player_walk";


    void Start()
    {
        myRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if(!GameManager.gameManager.fromBattle && !GameManager.gameManager.loseBattle)
        {
            transform.position = GameManager.gameManager.nextHeroPosition;
            GameManager.gameManager.spawnPointName = "";
            GameManager.gameManager.nextHeroPosition = Vector3.zero;
        }
        else if (GameManager.gameManager.loseBattle && GameManager.gameManager.fromBattle)
        {
            transform.position = Vector3.zero;
            GameManager.gameManager.spawnPointName = "";
            GameManager.gameManager.nextHeroPosition = Vector3.zero;
            GameManager.gameManager.loseBattle = false;
            GameManager.gameManager.fromBattle = false;
        }
        else
        {
            transform.position = GameManager.gameManager.lastHeroPosition;
            GameManager.gameManager.fromBattle = false;
        }
    }

    void Update()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        myRB.velocity = new Vector2(xAxis, yAxis) * mSpeed * Time.deltaTime;

        curPos = transform.position;
        if(curPos==lastPos)
        {
            GameManager.gameManager.isWalking = false;
        }
        else
        {
            GameManager.gameManager.isWalking = true;
        }
        lastPos = curPos;

        if(xAxis != 0 || yAxis != 0)
        {
            if(xAxis < 0)
            {
                this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            if(xAxis > 0)
            {
                this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            ChangeAnimationState(PLAYER_WALK);
        }
        else
        {
            ChangeAnimationState(PLAYER_IDLE);
        }

    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Warper")
        {
            GameManager.gameManager.spawnPointName = other.gameObject.GetComponent<SceneChanger>().spawnPoint;
            GameManager.gameManager.sceneToLoad = other.gameObject.GetComponent<SceneChanger>().sceneToLoad;
            GameManager.gameManager.LoadNextScene();
        }
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(other.tag == "Encounter Zone")
        {
            GameManager.gameManager.canGetEncounter = true;
            GameManager.gameManager.gameStates = GameManager.GameStates.DANGER_STATE;
            GameManager.gameManager.curRegion = other.gameObject.GetComponent<RegionData>();
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.tag == "Encounter Zone")
        {
            GameManager.gameManager.canGetEncounter = false;
            GameManager.gameManager.gameStates = GameManager.GameStates.SAFE_STATE;
        }
    }

    void ChangeAnimationState (string newAnimState)
    {
        //stop the same animation from interrupting itself
        if(currAnimState == newAnimState){return;}

        //play the animation
        animator.Play(newAnimState);
        //reassign the current state
        currAnimState = newAnimState;
    }
}
