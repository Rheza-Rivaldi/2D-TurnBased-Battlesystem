using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
    BattleStateMachine BSM;
    public BaseClass hero;

    Vector3 startposition;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        ACTION,
        DEAD
    }
    public TurnState currentState;

    //for progressbar
    float cur_cooldown = 0f;
    float max_cooldown = 3f;
    public Image ProgressBar;

    //IEnumerator
    bool actionStarted = false;
    public GameObject AttackTarget;
    float animSpeed = 10f;


    void Start()
    {
        cur_cooldown = Random.Range(0,1f);
        currentState = TurnState.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        this.transform.Find("HeroSelector").gameObject.SetActive(false);
        startposition = transform.position;
    }


    void Update()
    {
        //Debug.Log (currentState);
        switch (currentState)
        {
            case(TurnState.PROCESSING):
            UpdateProgressBar();
            break;
            
            case(TurnState.ADDTOLIST):
            BSM.HeroToManage.Add(this.gameObject);
            currentState = TurnState.WAITING;
            break;

            case(TurnState.WAITING):
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
        float calc_cooldown = cur_cooldown / max_cooldown;
        ProgressBar.transform.localScale = new Vector2 (Mathf.Clamp(calc_cooldown,0,1), ProgressBar.transform.localScale.y);

        if (cur_cooldown >= max_cooldown){
            currentState = TurnState.ADDTOLIST;
        }
    }

    IEnumerator TimeForAction()
    {
        if (actionStarted){yield break;}
        actionStarted = true;

        //move to target
        Vector2 targetPosition = new Vector3(AttackTarget.transform.position.x+1f,AttackTarget.transform.position.y,AttackTarget.transform.position.z);
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

    public void TakeDamage (float damageAmount)
    {
        hero.curHP -= damageAmount;
        if (hero.curHP <= 0)
        {
            currentState = TurnState.DEAD;
        }
    }
}
