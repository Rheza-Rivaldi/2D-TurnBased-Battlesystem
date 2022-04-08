using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D myRB;
    float mSpeed = 120f;


    void Start()
    {
        myRB = GetComponent<Rigidbody2D>();
        transform.position = GameManager.gameManager.nextHeroPosition;
    }

    void FixedUpdate()
    {
        myRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * mSpeed * Time.fixedDeltaTime;
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
}
