using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateMachine : MonoBehaviour
{
    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION
    }
    public PerformAction battleState;

    public List<TurnHandler> PerformList = new List<TurnHandler>();
    public List<GameObject> HeroInBattle = new List<GameObject>();
    public List<GameObject> EnemyInBattle = new List<GameObject>();


    void Start()
    {
        battleState = PerformAction.WAIT;
        HeroInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        EnemyInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    void Update()
    {
        switch (battleState)
        {
            case(PerformAction.WAIT):
            if(PerformList.Count > 0)
            {
                battleState = PerformAction.TAKEACTION;
            }
            break;

            case(PerformAction.TAKEACTION):
            GameObject performer = GameObject.Find(PerformList[0].AttackerName);

            if(PerformList[0].Type == "Enemy")
            {
                EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                ESM.AttackTarget = PerformList[0].TargetGameObject;
                ESM.currentState = EnemyStateMachine.TurnState.ACTION;
            }

            if(PerformList[0].Type == "Hero")
            {
                
            }

            battleState = PerformAction.PERFORMACTION;
            break;

            case(PerformAction.PERFORMACTION):
            break;
        }
    }

    public void CollectAction(TurnHandler input)
    {
        PerformList.Add(input);
    }
}
