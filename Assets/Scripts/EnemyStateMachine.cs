using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    BattleStateMachine BSM;
    public BaseClass enemy;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }
    public TurnState currentState;

    Vector3 startposition;

    //for progressbar
    float cur_cooldown = 0f;
    float max_cooldown = 3.5f;

    //time for action stuff
    bool actionStarted = false;
    public GameObject AttackTarget;
    float animSpeed = 10f;


    void Start()
    {
        currentState = TurnState.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startposition = transform.position;
        this.transform.Find("EnemySelector").gameObject.SetActive(false);
    }


    void Update()
    {
        switch (currentState)
        {
            case(TurnState.PROCESSING):
            UpdateProgressBar();
            break;
            
            case(TurnState.CHOOSEACTION):
            ChooseAction();
            currentState = TurnState.WAITING;
            break;

            case(TurnState.WAITING):
            //idle
            break;

            case(TurnState.ACTION):
            StartCoroutine(TimeForAction());
            break;

            case(TurnState.DEAD):
            break;
        }
    }

     void UpdateProgressBar()
    {
        cur_cooldown = cur_cooldown + Time.deltaTime;

        if (cur_cooldown >= max_cooldown){
            currentState = TurnState.CHOOSEACTION;
        }
    }

    void ChooseAction()
    {
        TurnHandler myAttack = new TurnHandler();
        myAttack.AttackerName = enemy.name;
        myAttack.Type = "Enemy";
        myAttack.AttackerGameObject = this.gameObject;
        myAttack.TargetGameObject = BSM.HeroInBattle[Random.Range(0, BSM.HeroInBattle.Count)];
        BSM.CollectAction(myAttack);
    }

    IEnumerator TimeForAction()
    {
        if (actionStarted){yield break;}
        actionStarted = true;

        //move to target
        Vector2 targetPosition = new Vector3(AttackTarget.transform.position.x-1f,AttackTarget.transform.position.y,AttackTarget.transform.position.z);
        while(MoveTowardTarget(targetPosition))
        {
            yield return null;
        }
        //wait a bit
        yield return new WaitForSeconds(0.5f);
        //do damage

        //go back to original position
        Vector3 firstPosition = startposition;
        while(MoveTowardStart(firstPosition))
        {
            yield return null;
        }
        //remove from perform list
        BSM.PerformList.RemoveAt(0);
        //reset bsm state to waiting
        BSM.battleState = BattleStateMachine.PerformAction.WAIT;
        //end coroutine
        actionStarted = false;
        //reset this object state
        cur_cooldown = 0f;
        currentState = TurnState.PROCESSING;
    }

    private bool MoveTowardTarget (Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardStart (Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
}
