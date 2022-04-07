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
    }

    void FixedUpdate()
    {
        myRB.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * mSpeed * Time.fixedDeltaTime;
    }
}
