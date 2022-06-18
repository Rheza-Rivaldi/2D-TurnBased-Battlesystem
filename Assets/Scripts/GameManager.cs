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
    public string lastScene;

    public bool isWalking = false;
    public bool canGetEncounter = false;
    public bool gotAttacked = false;
    public bool fromBattle = false;
    public bool encounterRoll = false;

    public RegionData curRegion;
    public List<GameObject> enemiesToBattle = new List<GameObject>();

    public int enemyAmount;

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
        SceneManager.sceneLoaded -= SetPlayerPosition;
        SceneManager.sceneLoaded += SetPlayerPosition;
        gameStates = GameStates.SAFE_STATE;
    }

    void OnDestroy() 
    {
        SceneManager.sceneLoaded -= SetPlayerPosition;
    }

    void Update()
    {
        switch(gameStates)
        {
            case(GameStates.SAFE_STATE):
            break;

            case(GameStates.DANGER_STATE):
            if(isWalking && !gotAttacked)
            {
                //Invoke("RandomEncounter", 0.1f);
                StartCoroutine(RandomEncounter());
            }
            if(gotAttacked)
            {
                gameStates = GameStates.BATTLE_STATE;
            }
            break;

            case(GameStates.BATTLE_STATE):
            StartBattle();
            SceneManager.LoadScene("BattleScene");
            sceneToLoad = lastScene;
            gameStates = GameStates.IDLE;
            break;

            case(GameStates.IDLE):
            break;
        }
        //if(gotAttacked){gameStates = GameStates.BATTLE_STATE; gotAttacked = false;}
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    void SetPlayerPosition(Scene scene, LoadSceneMode mode)
    {
        canGetEncounter = false;
        isWalking = false;
        gotAttacked = false;

        if(scene.name != "BattleScene" && !fromBattle)
        {
            if(spawnPointName != null)
            {
                nextPos = GameObject.Find(spawnPointName);
                nextHeroPosition = nextPos.transform.position;
            }
        }
    }

    IEnumerator RandomEncounter()
    {
        if(encounterRoll){yield break;}
        encounterRoll = true;
        if(isWalking && canGetEncounter)
        {
            if(Random.Range(0,100) <= 2)
            {
                encounterRoll = false;
                gotAttacked = true;
            }
            else
            {
                yield return new WaitForSecondsRealtime(1f);
                encounterRoll = false;
            }
        }
    }

    void StartBattle()
    {
        enemyAmount = Random.Range(1, curRegion.MaxAmountEnemies+1);
        for(int i = 0; i < enemyAmount; i++)
        {
            enemiesToBattle.Add(curRegion.possibleEnemies[Random.Range(0, curRegion.possibleEnemies.Count)]);
        }
        lastHeroPosition = GameObject.Find("Player").transform.position;
        lastScene = SceneManager.GetActiveScene().name;

        isWalking = false;
        canGetEncounter = false;

        //SceneManager.LoadScene("BattleScene");
    }

}
