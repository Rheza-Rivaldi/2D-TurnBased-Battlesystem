using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    public GameObject heroChar;
    GameObject nextPos;

    public string spawnPointName;
    public Vector3 nextHeroPosition;

    public string sceneToLoad;


    void Awake()
    {
        if(gameManager == null)
        {
            gameManager = this;
        }
        else if(gameManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        if(!GameObject.Find("Player"))
        {
            GameObject Player = Instantiate (heroChar, Vector3.zero, Quaternion.identity) as GameObject;
            Player.name = "Player";
        }
    }

    void Start() 
    {
        SceneManager.sceneLoaded += SetPlayerPosition;
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    void SetPlayerPosition(Scene scene, LoadSceneMode mode)
    {
        nextPos = GameObject.Find(spawnPointName);
        nextHeroPosition = nextPos.transform.position;
    }

}
