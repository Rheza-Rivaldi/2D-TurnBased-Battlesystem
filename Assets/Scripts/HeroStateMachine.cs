using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
    BattleStateMachine BSM;
    public BaseClass hero;

    //for animations
    Animator animator;
    string currAnimState;
    //animation name
    public string animationName;
    //animation states
    const string HERO_IDLE = "_idle";
    const string HERO_WALK = "_walk";

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
    Image ProgressBar;

    //IEnumerator
    bool actionStarted = false;
    public GameObject AttackTarget;
    float animSpeed = 10f;

    bool alive = true;

    //heropanel
    HeroPanelStats stats;
    public GameObject HeroPanel;
    Transform HeroPanelSpacer;


    void Start()
    {
        animator = GetComponent<Animator>();
        HeroPanelSpacer = GameObject.Find("HeroPanelSpacer").transform;
        CreateHeroPanel();
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
            if(!alive)
            {
                return;
            }
            else
            {
                //change tag
                this.gameObject.tag = "DeadHero";
                //not attackable
                BSM.HeroInBattle.Remove(this.gameObject);
                //not manageable
                BSM.HeroToManage.Remove(this.gameObject);
                //deactivate selector
                this.transform.Find("HeroSelector").gameObject.SetActive(false);
                //reset GUI
                BSM.ActionPanel.SetActive(false);
                BSM.TargetSelectPanel.SetActive(false);
                //remove from performlist
                if(BSM.HeroInBattle.Count > 0)
                {
                    for(int i = 0; i < BSM.PerformList.Count; i++)
                    {
                        if(i != 0)
                        {
                            if(BSM.PerformList[i].AttackerGameObject == this.gameObject)
                            {
                                BSM.deadActor = BSM.PerformList[i].AttackerName;
                                //BSM.PerformList.Remove(BSM.PerformList[i]);
                            }
                            if(BSM.PerformList[i].TargetGameObject == this.gameObject)
                            {
                                BSM.PerformList[i].TargetGameObject = BSM.HeroInBattle[Random.Range(0, BSM.HeroInBattle.Count)];
                            }
                        }
                    }
                }
                //change color
                this.gameObject.GetComponent<SpriteRenderer>().color = new Color32(105,105,105,255);
                //reset heroinput
                //BSM.battleState = BattleStateMachine.PerformAction.CHECKALIVE;
                alive = false;
            }
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
        ChangeAnimationState(HERO_WALK);
        while(MoveTowardTarget(targetPosition))
        {
            yield return null;
        }
        ChangeAnimationState(HERO_IDLE);
        //wait a bit
        yield return new WaitForSeconds(0.25f);
        //do damage
        DoDamage();
        yield return new WaitForSeconds(0.25f);
        //go back to original position
        Vector3 firstPosition = startposition;
        ChangeAnimationState(HERO_WALK);
        while(MoveTowardStart(firstPosition))
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            yield return null;
        }
        this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        ChangeAnimationState(HERO_IDLE);
        //remove from perform list
        if(this.gameObject.tag != "DeadHero")
        {
            BSM.PerformList.RemoveAt(0);
        }
        //reset bsm state to waiting
        if(BSM.battleState != BattleStateMachine.PerformAction.WIN && BSM.battleState != BattleStateMachine.PerformAction.LOSE)
        {
            BSM.battleState = BattleStateMachine.PerformAction.CLEANUP;
            //end coroutine
            actionStarted = false;
            //reset this object state
            cur_cooldown = 0f;
            currentState = TurnState.PROCESSING;
        }
        else
        {
            currentState = TurnState.WAITING;
        }
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
        hero.curHP -= ((int)damageAmount);
        if (hero.curHP <= 0)
        {
            hero.curHP = 0;
            currentState = TurnState.DEAD;
        }
        UpdateHeroPanel();
    }

    void DoDamage()
    {
        float calc_damage = hero.curAtk * BSM.PerformList[0].choosenAttack.attackDamage;

        AttackTarget.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
    }

    void CreateHeroPanel()
    {
        HeroPanel = Instantiate(HeroPanel) as GameObject;
        stats = HeroPanel.GetComponent<HeroPanelStats>();
        stats.HeroName.text = hero.name;
        stats.HeroHP.text = "HP: " + hero.curHP;
        stats.HeroMP.text = "MP: " + hero.curMP;

        ProgressBar = stats.ProgressBar;
        HeroPanel.transform.SetParent(HeroPanelSpacer, false);
    }

    void UpdateHeroPanel()
    {
        stats.HeroHP.text = "HP: " + hero.curHP;
        stats.HeroMP.text = "MP: " + hero.curMP;
    }

    void ChangeAnimationState (string newAnimState)
    {
        //stop the same animation from interrupting itself
        if(currAnimState == newAnimState){return;}
        string newAnimName = this.gameObject.name+newAnimState;
        //Debug.Log(newAnimName);
        //play the animation
        animator.Play(newAnimName);
        //reassign the current state
        currAnimState = newAnimState;
    }
}
