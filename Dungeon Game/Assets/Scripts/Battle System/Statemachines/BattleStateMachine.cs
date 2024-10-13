using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BattleStateMachine : MonoBehaviour
{
    // ENUMS
    public enum BattleStates 
    {
        IDLE,
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
    }

    public enum HeroGUI 
    {
        ACTIVATE,
        WAITING,
        DONE
    }

    public BattleStates battleStates;

    // LIST
    public List<HandleTurn> performList = new();
    public List<GameObject> herosInBattle = new();
    public List<GameObject> enemiesInBattle = new();

    // HERO INPUT
    public HeroGUI heroInput;
    public HandleTurn heroTurn;
    public List<GameObject> heroesToManage = new();
    // [SerializeField] private List<HeroStateMachine> heroSelected = new();

    // PANELS UI
    // public GameObject attackParentPanel;
    // public GameObject actionPanel;
    // public GameObject magicPanel;
    // public GameObject enemySelectPanel;

    // New Panels

    // SPACERS UI
    // public Transform targetSelectSpacer;
    // public Transform actionSpacer;
    // public Transform magicSpacer;

    // BUTTONS UI
    // public GameObject targetButton;
    // public GameObject magicAttButton;
    // private readonly List<GameObject> atkBtns = new();
    // private List<GameObject> physicalAtt_btns = new();
    // private List<GameObject> magicalAtt_btns = new();
    // private List<GameObject> defense_btns = new();

    // private List<GameObject> enemyTarget_btns = new();
    // private List<GameObject> heroTarget_btns = new();

    // private readonly List<GameObject> enemyBtns = new();


    // public bool isListenerAdded = false;


    // SPAWN
    // public List<Transform> spawnPoints = new();

    // void Awake() 
    // {
    //     herosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Hero").OrderBy(hero => hero.name)); 
    //     for (int i = 0; i < GameManager.instance.enemyAmount; i++)
    //     {
    //         GameObject newEnemy = Instantiate(GameManager.instance.enemiesToBattle[i], spawnPoints[i].position, Quaternion.identity) as GameObject;
    //         newEnemy.name = newEnemy.GetComponent<EnemyStateMachine>().enemy.TheName + "_" + (i + 1);
    //         newEnemy.GetComponent<EnemyStateMachine>().enemy.TheName = newEnemy.name;
    //         enemiesInBattle.Add(newEnemy);
    //     }
    // }

    void Start()
    {
        battleStates = BattleStates.IDLE;
        // battleStates = BattleStates.WAIT;
        // heroInput = HeroGUI.ACTIVATE;
        // foreach (var player in heroesToManage)
        // {
            // HeroStateMachine HSM = heroesToManage[0].GetComponent<HeroStateMachine>();
            // HSM.select_btn.GetComponent<Button>().onClick.AddListener( () => ChooseHeroInput() );

            // Debug.Log("Succeed in adding onclick to the button");
        // }
        // PANELS
        // attackParentPanel.SetActive(false);
        // actionPanel.SetActive(false);
        // enemySelectPanel.SetActive(false);
        // magicPanel.SetActive(false);

        // EnemyButtons();
    }

    void Update()
    {
        // foreach (var hero in heroesToManage)
        // {
        //     if(hero.TryGetComponent<HeroStateMachine>(out var hsm)) 
        //     {
        //         var heroPanel = hsm.heroPanelUI;

        //         // var heroPanelStatsClass = heroPanel.GetComponent<HeroPanelStats>();

        //         if(!hsm.heroSelected) 
        //         {
        //             var heroSelect_btn = hsm.selectHero_btn.GetComponent<Button>();
        //             heroSelect_btn.onClick.AddListener(() => SelectHeroInput());

        //             hsm.heroSelected = true;
        //         }


        //         if(hsm.heroSelected) 
        //         {
        //             var toBattle_btn = hsm.toBattlePanel_btn.GetComponent<Button>();
        //             toBattle_btn.onClick.AddListener(() => OpenBattlePanel());
        //         }
        //     }
        // }

        switch (battleStates)
        {
            case(BattleStates.WAIT):
                if(performList.Count > 0) 
                {
                    battleStates = BattleStates.TAKEACTION;
                }

            break;

            case(BattleStates.TAKEACTION):
                GameObject performer = GameObject.Find(performList[0].performerName);

                if(performList[0].type == "Enemy") 
                {
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    for (int i = 0; i < herosInBattle.Count; i++)
                    {
                        if(performList[0].performersTarget == herosInBattle[i])
                        {
                            ESM.targetToAttack = performList[0].performersTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.ACTION; 
            
                        } else 
                        {
                            performList[0].performersTarget = herosInBattle[Random.Range(0, herosInBattle.Count)];

                            ESM.targetToAttack = performList[0].performersTarget;
                            ESM.currentState = EnemyStateMachine.TurnState.ACTION; 
                        }
                    }
                }

                if(performList[0].type == "Hero") 
                {
                    HeroStateMachine HSM = performer.GetComponent<HeroStateMachine>();
                    HSM.targetToAttack = performList[0].performersTarget;
                    HSM.currentState = HeroStateMachine.TurnState.ACTION;
                }

                battleStates = BattleStates.PERFORMACTION;

            break;

            case(BattleStates.PERFORMACTION):
                // Idle state
            break;

            case(BattleStates.CHECKALIVE):
                if(herosInBattle.Count < 1) 
                {
                    battleStates = BattleStates.LOSE;

                } else if(enemiesInBattle.Count < 1)
                {
                    battleStates = BattleStates.WIN;
                    
                } else 
                {
                    // ClearAttackPanel();
                    heroInput = HeroGUI.ACTIVATE;
                }

            break;

            case(BattleStates.WIN):
                // for (int i = 0; i < herosInBattle.Count; i++)
                // {
                //     herosInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.WAITING;

                //     GameManager.instance.LoadSceneAfterBattle();
                //     GameManager.instance.gameState = GameManager.GameStates.WORLDSTATE;
                //     GameManager.instance.enemiesToBattle.Clear();
                // }

            break;

            case(BattleStates.LOSE):
                // Do something
            break;
        }
        
        switch (heroInput) 
        {
            case(HeroGUI.ACTIVATE):
                // attackButton.GetComponent<Button>().onClick.AddListener( () => Input1() );
                if(heroesToManage.Count > 0)
                {
                  
                    // Instead of activating the selector on the first hero in the list
                    // I want to be able to select a specific hero and then a selector will appear on the clicked hero

                    // herosToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    heroTurn = new HandleTurn(); // Create new handle turn instance

                    // heroPanel.SetActive(false);
                    // battlePanel.SetActive(true);

                    // // attackParentPanel.SetActive(true);
                    // actionPanel.SetActive(true);
                    
                    // CreateAttackBTNS(); // Populate the attack panel with attack buttons
                    
                    // heroInput = HeroGUI.WAITING;
                }

            break;

            case(HeroGUI.WAITING):
                // Idle state

            break;

            case(HeroGUI.DONE):
                HeroInputDone();

            break;
        }
    
    }

    public void CollectActions(HandleTurn _input) 
    {
        performList.Add(_input);
    }

    // public void EnemyButtons() 
    // {
    //     // Clean up everything that is already created
    //     foreach(GameObject _enemyBtn in enemyBtns) 
    //     {
    //         Destroy(_enemyBtn);
    //     }
    //     enemyBtns.Clear();

    //     // Proceed further with creating buttons
    //     foreach (GameObject _enemy in enemiesInBattle)
    //     {
    //         GameObject newButton = Instantiate(targetButton) as GameObject;
    //         newButton.name = "Target: " + _enemy.name;

    //         SelectButton button = newButton.GetComponent<SelectButton>();
    //         TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
    //         EnemyStateMachine curEnemy = _enemy.GetComponent<EnemyStateMachine>();
            
    //         buttonText.text = curEnemy.enemy.TheName;
    //         button.enemyObj = _enemy; 

    //         newButton.transform.SetParent(targetSelectSpacer, false);
    //         enemyBtns.Add(newButton);
    //     }
    // }


    // public void Input1() // Attack button
    // {
    //     heroTurn.performerName = herosToManage[0].name;
    //     heroTurn.performer = herosToManage[0];
    //     heroTurn.type = "Hero";
    //     heroTurn.chosenAction = herosToManage[0].GetComponent<HeroStateMachine>().baseHero.attacks[0];

    //     actionPanel.SetActive(false);
    //     enemySelectPanel.SetActive(true);
    // }

    public void ChooseAttSpell() 
    {

    }

    public void SelectHeroInput()
    {
        Debug.Log("Click detected");
        Debug.Log("Number of heroes to manage: " + heroesToManage.Count);

        for (int i = 0; i < heroesToManage.Count; i++)
        {
            if(heroesToManage[i].TryGetComponent<HeroStateMachine>(out var hsm))
            {
                // Pass in the hero for the HandleTurn class
                heroTurn.performerName = heroesToManage[i].name;
                heroTurn.performer = heroesToManage[i];
                heroTurn.type = "Hero";

                hsm.heroPanelSelector.SetActive(false);
                hsm.selectHero_btn.SetActive(false);
                hsm.toBattlePanel_btn.SetActive(true);
                Debug.Log("Panel set inactive: " + heroesToManage[i].name);

            }

            // Debug.Log("Processing hero at index: " + i);

            // if (heroesToManage[i] == null)
            // {
            //     Debug.LogError("Hero at index " + i + " is null!");
            //     continue;
            // }

            // if (!heroesToManage[i].TryGetComponent<HeroStateMachine>(out var heroStateMachine))
            // {
            //     Debug.LogError("HeroStateMachine component is missing on hero at index " + i);
            //     continue;
            // }

            // Debug.Log("Click detected after click");

            // if (heroSelected.Contains(heroStateMachine))
            // {
            //     Debug.Log("Hero already selected, clearing selection");
            //     ClearSelection();

            //     heroStateMachine.heroPanelSelector.SetActive(false);

            //     heroTurn.performerName = heroesToManage[i].name;
            //     heroTurn.performer = heroesToManage[i];
            //     heroTurn.type = "Hero";

            //     heroSelected.Add(heroStateMachine);
            // }
            // else 
            // {
                
            //     heroStateMachine.heroPanelSelector.SetActive(false);

            //     heroTurn.performerName = heroesToManage[i].name;
            //     heroTurn.performer = heroesToManage[i];
            //     heroTurn.type = "Hero";

            //     heroSelected.Add(heroStateMachine);
            // }
        }
    }

    // private void OpenBattlePanel() 
    // {
    //     for (int i = 0; i < heroesToManage.Count; i++)
    //     {
    //         if(heroesToManage[i].TryGetComponent<HeroStateMachine>(out var hsm))
    //         {
    //             hsm.toBattlePanel_btn.SetActive(false);
    //             hsm.battlePanel.SetActive(true);
    //             hsm.heroStatsPanel.SetActive(false);

    //             if(hsm.descriptionPanel == null) 
    //             {
    //                 var descrPanel = hsm.battlePanel.transform.Find("DescriptionPanel").gameObject;
    //                 hsm.descriptionPanel = descrPanel;
    //             }

    //             hsm.battlePanel_open = true;
    //         }
    //     }
        
    //     CreateActionTypeButtons();
    // }
    
    // public void Input1() // Attack button
    // {
    //     heroTurn.performerName = heroesToManage[0].name;
    //     heroTurn.performer = heroesToManage[0];
    //     heroTurn.type = "Hero";
    //     heroTurn.chosenAction = heroesToManage[0].GetComponent<HeroStateMachine>().baseHero.physicalAttacks[0];

    //     actionPanel.SetActive(false);
    //     enemySelectPanel.SetActive(true);
    // }



    // public void ClearSelection() 
    // {
    //     if(heroSelected.Count > 0) 
    //     {
    //         heroSelected.Clear();

    //     }

    //     // Change the battle button to select button again
    //     for (int i = 0; i < heroSelected.Count; i++)
    //     {
    //         heroSelected[i].battle_btn.SetActive(false);
    //         heroSelected[i].select_btn.SetActive(true);
    //         heroSelected[i].selectHero_btn.SetActive(true);
    //     }
    // }
    
    public void ChooseTarget(GameObject _chosenEnemy) // Gets called in the Select button script 
    {
       heroTurn.performersTarget = _chosenEnemy;
       heroInput = HeroGUI.DONE;
    }

    private void HeroInputDone()
    {
        performList.Add(heroTurn);
        // ClearAttackPanel();

        heroesToManage[0].transform.Find("Selector").gameObject.SetActive(false);
        heroesToManage.RemoveAt(0);
        heroInput = HeroGUI.ACTIVATE;
    }

    // private void ClearAttackPanel()
    // {
    //     enemySelectPanel.SetActive(false);
    //     actionPanel.SetActive(false);
    //     magicPanel.SetActive(false);
    //     attackParentPanel.SetActive(false);

    //     foreach (GameObject _atkBtn in atkBtns)
    //     {
    //         Destroy(_atkBtn);
    //     }
    //     atkBtns.Clear();
    // }

    // private void CreateActionButtonsForHero(GameObject panel, List<BaseAction> actions, GameObject actionTypeBtnPrefab)
    // {
    //     foreach (BaseAction action in actions)
    //     {
    //         GameObject actionButton = Instantiate(actionTypeBtnPrefab, panel.transform);
    //         actionButton.transform.SetParent(panel.transform);

    //         TextMeshProUGUI buttonText = actionButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
    //         buttonText.text = action.actionName;

    //         // Add a listener to the button here (if needed)
    //         actionButton.GetComponent<Button>().onClick.AddListener(() =>
    //         {
    //             // Handle button click logic, e.g., choosing this action
    //             Debug.Log("Clicked on action: " + action.actionName);
    //         });
    //     }
    // }

    

    // private void ShowActionPanel(GameObject actionPanel)
    // {
    //     Debug.Log("Button clicked!"); // Check if this message appears in the console

    //     foreach (var hero in heroesToManage)
    //     {
    //         Debug.Log("Hero found");
    //         if (hero.TryGetComponent<HeroStateMachine>(out var hsm))
    //         {
    //             // Disable select action panel and enable appropriate action panel
    //             hsm.selectActionPanel.SetActive(false);
    //             hsm.actionPanels.SetActive(true);

    //             Debug.Log("Fetched the HSM");

    //             // Hide previous panel if any
    //             if (hsm.currentActionPanel != null && hsm.currentActionPanel != actionPanel)
    //             {
    //                 hsm.currentActionPanel.SetActive(false);
    //                 Debug.Log("Previous panel hidden");
    //             }

    //             // Show the new panel
    //             actionPanel.SetActive(true);
    //             Debug.Log("New panel active");

    //             hsm.currentActionPanel = actionPanel;
    //             Debug.Log("current action panel = actionPanel");

    //             // Check if buttons are already created for this panel
    //             bool buttonsCreated = false;
    //             if (actionPanel == hsm.wattAction_panel && hsm.wattActionBtnCreated)
    //             {
    //                 buttonsCreated = true;
    //             }
    //             else if (actionPanel == hsm.mattAction_panel && hsm.mattActionBtnCreated)
    //             {
    //                 buttonsCreated = true;
    //             }
    //             else if (actionPanel == hsm.buffAction_Panel && hsm.buffActionBtnCreated)
    //             {
    //                 buttonsCreated = true;
    //             }

    //             // If buttons are not already created, create them
    //             if (!buttonsCreated)
    //             {
    //                 // Clear existing buttons in the panel
    //                 foreach (Transform child in actionPanel.transform)
    //                 {
    //                     Destroy(child.gameObject);
    //                 }

    //                 // Create buttons based on the action panel
    //                 if (actionPanel == hsm.wattAction_panel)
    //                 {
    //                     CreateActionButtonsForHero(actionPanel, hsm.baseHero.physicalAttacks, hsm.actionType_btn);
    //                     hsm.wattActionBtnCreated = true;
    //                     Debug.Log("watt buttons created");
    //                 }
    //                 else if (actionPanel == hsm.mattAction_panel)
    //                 {
    //                     CreateActionButtonsForHero(actionPanel, hsm.baseHero.magicAttacks, hsm.actionType_btn);
    //                     hsm.mattActionBtnCreated = true;
    //                     Debug.Log("matt buttons created");
    //                 }
    //                 else if (actionPanel == hsm.buffAction_Panel)
    //                 {
    //                     CreateActionButtonsForHero(actionPanel, hsm.baseHero.buffs, hsm.actionType_btn);
    //                     hsm.buffActionBtnCreated = true;
    //                     Debug.Log("buff buttons created");
    //                 }
    //             }

    //             // Ensure visibility of each action panel
    //             hsm.wattAction_panel.SetActive(actionPanel == hsm.wattAction_panel);
    //             hsm.mattAction_panel.SetActive(actionPanel == hsm.mattAction_panel);
    //             hsm.buffAction_Panel.SetActive(actionPanel == hsm.buffAction_Panel);
    //         }
    //     }
    // }


    // private void ShowActionPanel(GameObject actionPanel)
    // {
    //     Debug.Log("Button clicked!"); // Check if this message appears in the console

    //     foreach (var hero in heroesToManage)
    //     {
    //         Debug.Log("Hero found");
    //         if (hero.TryGetComponent<HeroStateMachine>(out var hsm))
    //         {
    //             hsm.actionPanels.SetActive(true);
    //             hsm.selectActionPanel.SetActive(false);
    //             Debug.Log("Fetched the HSM");
    //             // Hide previous panel if any
    //             if (hsm.currentActionPanel != null)
    //             {
    //                 hsm.currentActionPanel.SetActive(false);
    //                 Debug.Log("Previous panel hidden");
    //             }

    //             // Show the new panel
    //             actionPanel.SetActive(true);
    //             Debug.Log("New panel active");

    //             hsm.currentActionPanel = actionPanel;
    //             Debug.Log("current action panel = actionPanel");

    //             // Create buttons specific to the panel
    //             if (actionPanel == hsm.wattAction_panel)
    //             {
    //                 CreateActionButton(actionPanel, hsm.baseHero.physicalAttacks);
    //                 Debug.Log("watt button created");
    //             }
    //             else if (actionPanel == hsm.mattAction_panel)
    //             {
    //                 CreateActionButton(actionPanel, hsm.baseHero.magicAttacks);
    //                 Debug.Log("matt button created");
    //             }
    //             else if (actionPanel == hsm.buffAction_Panel)
    //             {
    //                 CreateActionButton(actionPanel, hsm.baseHero.buffs);
    //                 Debug.Log("buff button created");
    //             }

    //             // Ensure visibility of each action panel
    //             hsm.wattAction_panel.SetActive(actionPanel == hsm.wattAction_panel);
    //             hsm.mattAction_panel.SetActive(actionPanel == hsm.mattAction_panel);
    //             hsm.buffAction_Panel.SetActive(actionPanel == hsm.buffAction_Panel);
    //         }
    //     }
    // }


    // Back button functionality to go back to selectActionPanel
    public void GoBackToSelectActionPanel()
    {
        Debug.Log("Click Detected");
        foreach (var hero in heroesToManage)
        {
            var hsm = hero.GetComponent<HeroStateMachine>();
            if (hsm.currentActionPanel != null)
            {
                hsm.currentActionPanel.SetActive(false);
                hsm.currentActionPanel = null; // Reset current action panel

                // Show selectActionPanel
                hsm.selectActionPanel.SetActive(true);
            }

            hsm.actionPanels.SetActive(false);
        }
    }

    // private void CreateActionTypeButtons() 
    // {
    //     for (int i = 0; i < heroesToManage.Count; i++)
    //     {
    //         if (heroesToManage[i].TryGetComponent<HeroStateMachine>(out var hsm))
    //         {
    //             // Check if buttons are already created
    //             if (hsm.actionButtonsCreated) 
    //                 return;

    //             var actionPanels = hsm.battlePanel.transform.Find("ActionPanels").gameObject;
    //             hsm.actionPanels = actionPanels;

    //             hsm.wattAction_panel = actionPanels.transform.Find("wattAction_panel").gameObject;
    //             hsm.mattAction_panel = actionPanels.transform.Find("mattAction_panel").gameObject;
    //             hsm.buffAction_Panel = actionPanels.transform.Find("defAction_panel").gameObject;   
                
    //             // Physical attack action button
    //             GameObject attAction_btn = Instantiate(hsm.selectAction_btn);
    //             TextMeshProUGUI attAction_btnText = attAction_btn.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
    //             attAction_btn.name = "AttButton";
    //             attAction_btnText.text = "Physical Attack";

    //             attAction_btn.GetComponent<Button>().onClick.AddListener(() => ShowActionPanel(hsm.wattAction_panel));
    //             attAction_btn.transform.SetParent(hsm.selectActionPanel.transform);

    //             // Magic attack action button
    //             GameObject matt_btn = Instantiate(hsm.selectAction_btn);
    //             TextMeshProUGUI matt_btnText = matt_btn.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
    //             matt_btn.name = "MattButton";
    //             matt_btnText.text = "Magic Attack";

    //             matt_btn.GetComponent<Button>().onClick.AddListener(() => ShowActionPanel(hsm.mattAction_panel));
    //             matt_btn.transform.SetParent(hsm.selectActionPanel.transform);

    //             // Defense action button
    //             GameObject def_btn = Instantiate(hsm.selectAction_btn);
    //             TextMeshProUGUI def_btnText = def_btn.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
    //             def_btn.name = "BuffButton";
    //             def_btnText.text = "Buff";

    //             def_btn.GetComponent<Button>().onClick.AddListener(() => ShowActionPanel(hsm.buffAction_Panel));
    //             def_btn.transform.SetParent(hsm.selectActionPanel.transform);

    //             // Mark that buttons have been created
    //             hsm.actionButtonsCreated = true;

    //             var backBtn = hsm.descriptionPanel.transform.Find("ReturnButton").gameObject;

    //             hsm.backButton = backBtn;
    //             hsm.backButton.GetComponent<Button>().onClick.AddListener(GoBackToSelectActionPanel);                
    //         }
    //     }
    // }
    

    // public void Input3() // Switching to magic attacks 
    // {
    //     actionPanel.SetActive(false);
    //     magicPanel.SetActive(true);
    // }

    // public void Input4(BaseAction _chosenMagicSpell) // Magic att Panel
    // {
    //     heroTurn.performerName = herosToManage[0].name;
    //     heroTurn.performer = herosToManage[0];
    //     heroTurn.type = "Hero";

    //     heroTurn.chosenAction = _chosenMagicSpell;
    //     magicPanel.SetActive(false);
    //     enemySelectPanel.SetActive(true);
    // }
}


