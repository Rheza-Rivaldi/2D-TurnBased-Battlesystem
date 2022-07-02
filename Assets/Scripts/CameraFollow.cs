using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    GameObject player;

    [SerializeField]
    float camSpeed = 3f;

    public float topLimit, bottomLImit, leftLimit, rightLimit;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        transform.position = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = player.transform.position;
        endPos.z = -10;

        transform.position = Vector3.Lerp(startPos, endPos, camSpeed * Time.deltaTime);


        transform.position = new Vector3(Mathf.Clamp(transform.position.x, leftLimit, rightLimit), Mathf.Clamp(transform.position.y, bottomLImit, topLimit), transform.position.z);
    }
}
