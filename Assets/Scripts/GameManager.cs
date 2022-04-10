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
    public Vector3 lastHeroPosition;

    public string sceneToLoad;

    public bool isWalking = false;
    public bool canGetEncounter = false;
    public bool gotAttacked = false;

    public enum GameStates
    {
        DANGER_STATE,
        SAFE_STATE,
        BATTLE_STATE,
        IDLE
    }
    public GameStates gameStates;

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
        gameStates = GameStates.SAFE_STATE;
    }

    void Update()
    {
        switch(gameStates)
        {
            case(GameStates.SAFE_STATE):
            break;

            case(GameStates.DANGER_STATE):
            if(isWalking)
            {
                RandomEncounter();
            }
            if(gotAttacked)
            {
                gameStates = GameStates.BATTLE_STATE;
            }
            break;

            case(GameStates.BATTLE_STATE):
            break;

            case(GameStates.IDLE):
            break;
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    void SetPlayerPosition(Scene scene, LoadSceneMode mode)
    {
        if(sceneToLoad != "BattleScene")
        {
            if(gotAttacked)
            {
                nextHeroPosition = lastHeroPosition;
            }
            else
            {
                nextPos = GameObject.Find(spawnPointName);
                nextHeroPosition = nextPos.transform.position;
            }
        }
    }

    void RandomEncounter()
    {
        if(isWalking && canGetEncounter)
        {
            if(Random.Range(0,100) <= 5)
            {
                gotAttacked = true;
            }
        }
    }

    void StartBattle()
    {

    }

}
