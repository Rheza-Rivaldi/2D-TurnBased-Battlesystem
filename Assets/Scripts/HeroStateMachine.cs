using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{

    public BaseClass hero;

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
    float max_cooldown = 3f;
    public Image ProgressBar;


    void Start()
    {
        currentState = TurnState.PROCESSING;
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
        float calc_cooldown = cur_cooldown / max_cooldown;
        ProgressBar.transform.localScale = new Vector2 (Mathf.Clamp(calc_cooldown,0,1), ProgressBar.transform.localScale.y);

        if (cur_cooldown >= max_cooldown){
            currentState = TurnState.ADDTOLIST;
        }
    }
}
