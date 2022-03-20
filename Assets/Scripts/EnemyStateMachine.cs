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

    Vector2 startposition;

    //for progressbar
    float cur_cooldown = 0f;
    float max_cooldown = 3.5f;


    void Start()
    {
        currentState = TurnState.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startposition = transform.position;
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
        myAttack.AttackerGameObject = this.gameObject;
        myAttack.TargetGameObject = BSM.HeroInBattle[Random.Range(0, BSM.HeroInBattle.Count)];
        BSM.CollectAction(myAttack);
    }
}
