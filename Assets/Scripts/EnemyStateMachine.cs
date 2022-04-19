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

    bool alive = true;


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
            if(!alive)
            {
                return;
            }
            else
            {
                this.gameObject.tag = "DeadEnemy";
                BSM.EnemyInBattle.Remove(this.gameObject);
                this.transform.Find("EnemySelector").gameObject.SetActive(false);
                if(BSM.EnemyInBattle.Count > 0)
                {
                    for(int i = 0; i < BSM.PerformList.Count; i++)
                    {
                        if(i != 0)
                        {
                            if(BSM.PerformList[i].AttackerGameObject == this.gameObject)
                            {
                                //BSM.PerformList.RemoveAt(i);
                                BSM.deadActor = BSM.PerformList[i].AttackerName;
                                //Debug.Log(this.gameObject.name +" is removed");
                            }
                            if(BSM.PerformList[i].TargetGameObject == this.gameObject)
                            {
                                BSM.PerformList[i].TargetGameObject = BSM.EnemyInBattle[(Random.Range(0,BSM.EnemyInBattle.Count))];
                            }
                        }
                    }
                }
                this.gameObject.GetComponent<SpriteRenderer>().color = new Color32(105,105,105,255);
                alive = false;
                BSM.EnemyButtons();
                //BSM.battleState = BattleStateMachine.PerformAction.CHECKALIVE;
            }
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
        myAttack.choosenAttack = enemy.AttackList[Random.Range(0, enemy.AttackList.Count)];
        BSM.CollectAction(myAttack);

        //Debug.Log(this.gameObject.name + " choose " + myAttack.choosenAttack.attackName + " and do " + myAttack.choosenAttack.attackDamage + " damage.");
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
        DoDamage();
        //go back to original position
        Vector3 firstPosition = startposition;
        while(MoveTowardStart(firstPosition))
        {
            yield return null;
        }
        //remove from perform list
        if(this.gameObject.tag != "DeadEnemy")
        {
            BSM.PerformList.RemoveAt(0);
        }
        //reset bsm state to waiting
        BSM.battleState = BattleStateMachine.PerformAction.CLEANUP;
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

    void DoDamage()
    {
        float calc_damage = enemy.curAtk * BSM.PerformList[0].choosenAttack.attackDamage;

        AttackTarget.GetComponent<HeroStateMachine>().TakeDamage(calc_damage);
    }

    public void TakeDamage (float damageAmount)
    {
        enemy.curHP -= ((int)damageAmount);
        if (enemy.curHP <= 0)
        {
            enemy.curHP = 0;
            currentState = TurnState.DEAD;
        }
    }
}
