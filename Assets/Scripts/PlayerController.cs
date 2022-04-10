using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D myRB;
    float mSpeed = 120f;
    Vector3 curPos, lastPos;


    void Start()
    {
        myRB = GetComponent<Rigidbody2D>();
        transform.position = GameManager.gameManager.nextHeroPosition;
    }

    void FixedUpdate()
    {
        myRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * mSpeed * Time.fixedDeltaTime;

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
}
