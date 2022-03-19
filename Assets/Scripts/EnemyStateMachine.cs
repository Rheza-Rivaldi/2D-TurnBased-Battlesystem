using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{

    public BaseClass enemy;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }
    public TurnState currentState;

    //for progressbar
    float cur_cooldown = 0f;
    float max_cooldown = 3.5f;


    void Start()
    {
        currentState = TurnState.PROCESSING;
    }


    void Update()
    {
        switch (currentState)
        {
            case(TurnState.PROCESSING):
            UpdateProgressBar();
            break;
            
            case(TurnState.ADDTOLIST):
            break;

            case(TurnState.WAITING):
            break;

            case(TurnState.SELECTING):
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
            currentState = TurnState.ADDTOLIST;
        }
    }
}
