using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


    public enum HeroGUI
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }

    public HeroGUI HeroInput;

    public List<GameObject> HeroToManage = new List<GameObject>();
    private TurnHandler HeroChoice;

    public GameObject enemyButton;
    public GameObject actionButton;
    public Transform TargetSpacer;
    public Transform ActionSpacer;
    public Transform MagicSpacer;

    public GameObject ActionPanel;
    public GameObject TargetSelectPanel;
    public GameObject MagicPanel;

    List<GameObject> atkBtns = new List<GameObject>();




    void Start()
    {
        battleState = PerformAction.WAIT;
        HeroInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero"));
        EnemyInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        HeroInput = HeroGUI.ACTIVATE;

        ActionPanel.SetActive(false);
        TargetSelectPanel.SetActive(false);
        MagicPanel.SetActive(false);

        EnemyButtons();
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
                for(int i = 0; i<HeroInBattle.Count;i++){
                    if(PerformList[0].TargetGameObject == HeroInBattle[i])
                    {
                        ESM.AttackTarget = PerformList[0].TargetGameObject;
                        ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                        break;
                    }
                    else
                    {
                        PerformList[0].TargetGameObject = HeroInBattle[Random.Range(0, HeroInBattle.Count)];
                        ESM.AttackTarget = PerformList[0].TargetGameObject;
                        ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                    }
                }
                
            }

            if(PerformList[0].Type == "Hero")
            {
                HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                HSM.AttackTarget = PerformList[0].TargetGameObject;
                HSM.currentState = HeroStateMachine.TurnState.ACTION;
            }

            battleState = PerformAction.PERFORMACTION;
            break;

            case(PerformAction.PERFORMACTION):
            break;
        }

        switch(HeroInput)
        {
            case(HeroGUI.ACTIVATE):
            if(HeroToManage.Count > 0)
            {
                HeroToManage[0].transform.Find("HeroSelector").gameObject.SetActive(true);
                HeroChoice = new TurnHandler();
                ActionPanel.SetActive(true);
                CreateAttackButton();

                HeroInput = HeroGUI.WAITING;
            }
            break;

            case(HeroGUI.WAITING):
            //idle
            break;

            case(HeroGUI.DONE):
            HeroInputDone();
            break;
        }

    }

    public void CollectAction(TurnHandler input)
    {
        PerformList.Add(input);
    }

    void EnemyButtons()
    {
        foreach (GameObject enemy in EnemyInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();

            Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
            buttonText.text = cur_enemy.enemy.name;

            button.EnemyPrefab = enemy;

            newButton.transform.SetParent(TargetSpacer,false);
        }
    }

    public void AttackButtonPress()
    {
        ActionPanel.SetActive(false);
        TargetSelectPanel.SetActive(true);
    }

    public void EnemySelectButtonPress(GameObject choosenEnemy)
    {
        HeroChoice.AttackerName = HeroToManage[0].name;
        HeroChoice.AttackerGameObject = HeroToManage[0];
        HeroChoice.Type = "Hero";
        HeroChoice.TargetGameObject = choosenEnemy;
        HeroInput = HeroGUI.DONE;
    }

    public void HeroInputDone()
    {
        PerformList.Add (HeroChoice);
        TargetSelectPanel.SetActive(false);

        foreach(GameObject atkBtn in atkBtns)
        {
            Destroy(atkBtn);
        }
        atkBtns.Clear();

        HeroToManage[0].transform.Find("HeroSelector").gameObject.SetActive(false);
        HeroToManage.RemoveAt(0);
        HeroInput = HeroGUI.ACTIVATE;
    }

    void CreateAttackButton()
    {
        GameObject AttackButton = Instantiate(actionButton) as GameObject;
        Text AttackButtontext = AttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        AttackButtontext.text = "Attack";
        AttackButton.GetComponent<Button>().onClick.AddListener(()=>AttackButtonPress());
        AttackButton.transform.SetParent(ActionSpacer, false);
        atkBtns.Add(AttackButton);

        GameObject MagicAttackButton = Instantiate(actionButton) as GameObject;
        Text MagicAttackButtontext = MagicAttackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        MagicAttackButtontext.text = "Magic";
        //MagicAttackButton.GetComponent<Button>().onClick.AddListener(()=>AttackButtonPress());
        MagicAttackButton.transform.SetParent(ActionSpacer, false);
        atkBtns.Add(MagicAttackButton);
    }
}
