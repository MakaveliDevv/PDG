// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// public class HeroStateMachine : MonoBehaviour
// {
//     private BattleStateMachine BSM; 

//     public HeroStats baseHero;
    
//     public enum TurnState 
//     {
//         PROCESSING,
//         ADDTOLIST,
//         WAITING,
//         ACTION,
//         DEAD
//     }

//     public TurnState currentState;
//     public GameObject targetToAttack;
//     private Vector2 startPosition;
//     private float animSpeed = 10f;

//     // Main Panels
//     [Header("Main Panels")]
//     [SerializeField] private Transform heroesPanel;
//     public GameObject heroPanelUI;
//     private HeroPanelStats UI_stats;
    
//     // Normal Panels
//     [Header("Panels")]
//     public GameObject heroStatsPanel; // Hero stats panel
//     public GameObject battlePanel; // Hero battle panel
//     public GameObject selectActionPanel; // Choose action type panel (att, matt, def)
//     public GameObject actionPanels;

//     // Type Action Panel
//     [Header("Sub Panel")]
//     public GameObject wattAction_panel, mattAction_panel, buffAction_Panel; // Panel for the watt buttons
//     public GameObject descriptionPanel, currentActionPanel;

//     [Header("Bool")]
//     public bool heroSelected, alive = true, actionStarted; 
//     public bool battlePanel_open; 
//     public bool actionButtonsCreated, typeActionBtnCreated, wattActionBtnCreated, mattActionBtnCreated, buffActionBtnCreated; 

//     [Header("Buttons")]
//     public GameObject selectHero_btn, toBattlePanel_btn; 
//     public GameObject selectAction_btn, actionType_btn; // Choose action type button
//     public GameObject backButton;
    
    
//     public GameObject heroPanelSelector;

//     void Awake() 
//     {
//         BSM = GameObject.FindGameObjectWithTag("BattleManager").GetComponent<BattleStateMachine>(); // Change to instance
//         FindUIElements();
//     }

//     void Start()
//     {
//         currentActionPanel = null; 
//         currentState = TurnState.ADDTOLIST;
//         startPosition = transform.position;
//     }  

//     private void FindUIElements() 
//     {
//         // CreateHeroPanel();

//         heroesPanel = GameObject.FindGameObjectWithTag("HeroesPanel").transform; // Main Panel
//         heroPanelSelector = heroPanelUI.transform.Find("PanelSelector").gameObject; 

//         heroStatsPanel = heroPanelUI.transform.Find("HeroStuff").gameObject; // Stats panel
//         battlePanel = heroPanelUI.transform.Find("BattlePanel").gameObject; // Battle panel

//         selectActionPanel = battlePanel.transform.Find("SelectActionPanel").gameObject; // Action panel

//         selectHero_btn = heroPanelUI.transform.Find("SelectHeroPanelButton").gameObject;
//         toBattlePanel_btn = heroPanelUI.transform.Find("ToBattlePanelButton").gameObject;
//     }

//     void Update()
//     {
//         IdkWhatToCallIt();

//         switch (currentState)
//         {
//             case (TurnState.ADDTOLIST):
//                 BSM.heroesToManage.Add(gameObject);

//                 currentState = TurnState.WAITING;

//             break;

//             case (TurnState.WAITING):
//                 // Idle state
//                 // When player is choosing an action

//             break;

//             case (TurnState.ACTION):
//                 StartCoroutine(TimeForAction());
                
//             break;

//             case (TurnState.DEAD):
//                 if(!alive) 
//                 {
//                     return; // Do nothing if hero still alive
//                 } else 
//                 {   
//                     // Change tag
//                     gameObject.tag = "DeadHero";

//                     // Not attackable by enemy
//                     BSM.herosInBattle.Remove(gameObject);

//                     // Not managable
//                     BSM.heroesToManage.Remove(gameObject);

//                     // Deactivate the selector
//                     // selector.SetActive(false);

//                     // Reset GUI
//                     // BSM.actionPanel.SetActive(false);
//                     // BSM.enemySelectPanel.SetActive(false);

//                     // Remove inputs from performlist
//                     if(BSM.herosInBattle.Count > 0) 
//                     {
//                         for (int i = 0; i < BSM.performList.Count; i++)
//                         {
//                             if(i != 0) 
//                             {
//                                 if(BSM.performList[i].performer == gameObject) 
//                                 {
//                                     BSM.performList.Remove(BSM.performList[i]);
//                                 }

//                                 // Check if the target of the enemy is this hero
//                                 if(BSM.performList[i].performersTarget == gameObject) 
//                                 {
//                                     // Change target to random target
//                                     BSM.performList[i].performersTarget = BSM.herosInBattle[Random.Range(0, BSM.herosInBattle.Count)];
//                                 }
//                             }
//                         }
//                     }

//                     // Change color / play animation
//                     SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
//                     if (spriteRenderer != null)
//                     {
//                         spriteRenderer.material.color = new Color32(105, 105, 105, 255);
//                     }
//                     else
//                     {
//                         Debug.LogError("SpriteRenderer component not found on the game object.");
//                     }

//                     // Reset hero input
//                     BSM.battleStates = BattleStateMachine.BattleStates.CHECKALIVE;
//                     alive = false;
//                 }
//             break;
            
//             // default:
//         }
//     }

//     private IEnumerator TimeForAction() 
//     {
//         if(actionStarted) 
//         {
//             yield break;            
//         }
        
//         actionStarted = true;

//         // Animate the hero near the enemy to attack
//         Vector2 targetPosition = new(targetToAttack.transform.position.x - 1.5f, targetToAttack.transform.position.y);
//         while(MoveTowardsTarget(targetPosition)) { yield return null; } // Change while loop to something else

//         // Wait a bit
//         yield return new WaitForSeconds(1f);

//         // Do damage
//         DoDamage();

//         // Animate back to start position
//         Vector2 initialPosition = startPosition;
//         while(MoveTowardsTarget(initialPosition)) { yield return null; } // Change while loop to something else

//         // Remove this performer from the list in the BSM
//         BSM.performList.RemoveAt(0);

//         // Reset the battle state to waiting after completing an action
//         if(BSM.battleStates != BattleStateMachine.BattleStates.WIN && BSM.battleStates != BattleStateMachine.BattleStates.LOSE) 
//         {
//             BSM.battleStates = BattleStateMachine.BattleStates.WAIT;

//             // Reset the hero state
//             // curCooldown = 0f;
//             currentState = TurnState.PROCESSING;
//         } else 
//         {
//             // Set the hero state back to waiting (idle)
//             currentState = TurnState.WAITING;
//         }

//         // End coroutine
//         actionStarted = false;
//     }

//     private bool MoveTowardsTarget(Vector2 _targetPosition)
//     {
//         Vector2 newPosition = Vector2.MoveTowards(transform.position, _targetPosition, animSpeed * Time.deltaTime);
//         transform.position = newPosition;
//         return newPosition != _targetPosition;
//     }

//     public void DoDamage() 
//     {
//         float calc_damage = baseHero.curAtt + BSM.performList[0].chosenAction.actionPhysicalDmg;
//         targetToAttack.GetComponent<EnemyStateMachine>().TakeDamge(calc_damage); // Get the esm which represents the hero being attacked
//     }

//     public void TakeDamge(float _damageAmount) 
//     {
//         baseHero.currentHP -= _damageAmount;
//         if(baseHero.currentHP <= 0) 
//         {
//             baseHero.currentHP = 0;
//             currentState = TurnState.DEAD;
//         }

//         // UpdateHeroPanel();
//     }

//     // private void CreateHeroPanel() 
//     // {
//     //     heroPanelUI = Instantiate(heroPanelUI);
//     //     UI_stats = heroPanelUI.GetComponent<HeroPanelStats>();

//     //     UI_stats.heroName.text = baseHero.name.ToString();
//     //     UI_stats.HP.text = baseHero.curHP.ToString();
//     //     UI_stats.MP.text = baseHero.curMP.ToString();

//     //     heroPanelUI.transform.SetParent(heroesPanel, false);
//     // }

//     // private void UpdateHeroPanel() 
//     // {
//     //     UI_stats.HP.text = "HP: " + baseHero.curHP;
//     //     UI_stats.MP.text = "MP: " + baseHero.curMP;
//     // }

//     public void SelectHeroInput()
//     {
//         BSM.heroTurn.performerName = baseHero.name;
//         BSM.heroTurn.performer = gameObject;
//         BSM.heroTurn.type = "Hero";

//         heroPanelSelector.SetActive(false);
//         selectHero_btn.SetActive(false);
//         toBattlePanel_btn.SetActive(true);
//         Debug.Log("Panel set inactive: " + baseHero.name);
//     }


//     // Panel navigation
//     void IdkWhatToCallIt() 
//     {
//         if(!heroSelected) 
//         {
//             var heroSelect_btn = selectHero_btn.GetComponent<Button>();
//             heroSelect_btn.onClick.AddListener(() => SelectHeroInput());

//             heroSelected = true;
            
//         } else 
//         {
//             var toBattle_btn = toBattlePanel_btn.GetComponent<Button>();
//             toBattle_btn.onClick.AddListener(() => OpenBattlePanel());
//         }
//     }

//     private void OpenBattlePanel() 
//     {
//         toBattlePanel_btn.SetActive(false);
//         battlePanel.SetActive(true);
//         heroStatsPanel.SetActive(false);

//         if(descriptionPanel == null) 
//         {
//             var descrPanel = battlePanel.transform.Find("DescriptionPanel").gameObject;
//             descriptionPanel = descrPanel;
//         }

//         battlePanel_open = true;
            
        
        
//         CreateActionTypeButtons();
//     }

//     private void CreateActionTypeButtons() 
//     {
//         // Check if buttons are already created
//         if (actionButtonsCreated) 
//             return;

//         var _actionPanels = battlePanel.transform.Find("ActionPanels").gameObject; // Find ActionPanels holder
//         actionPanels = _actionPanels; // Assign to variable

//         // Physical attack action button
//         InstantiateActionButton(selectAction_btn, actionPanels, (panel) => () => ActivateActionPanel(panel), wattAction_panel, "Weapon Attack", "wattAction_panel");

//         // Magic attack action button
//         InstantiateActionButton(selectAction_btn, actionPanels, (panel) => () => ActivateActionPanel(panel), mattAction_panel, "Magic Attack", "mattAction_panel");

//         // Buff action button
//         InstantiateActionButton(selectAction_btn, actionPanels, (panel) => () => ActivateActionPanel(panel), buffAction_Panel, "Buff", "defAction_panel");

//         // Mark that buttons have been created
//         actionButtonsCreated = true;
//         var backBtn = descriptionPanel.transform.Find("ReturnButton").gameObject;

//         backButton = backBtn;
//         backButton.GetComponent<Button>().onClick.AddListener(GoBackToSelectActionPanel);                    
//     }

//     private void InstantiateActionButton(GameObject btnPrefab, GameObject panel, System.Func<GameObject, System.Action> actionMethod, GameObject parameter, string btnName, string panelName)
//     {
//         parameter = actionPanels.transform.Find(panelName).gameObject; // Find the action panel

//         GameObject attAction_btn = Instantiate(btnPrefab);
//         TextMeshProUGUI attAction_btnText = attAction_btn.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>();
        
//         // Name the btn
//         attAction_btn.name = btnName;
//         attAction_btnText.text = btnName;

//         // Add listener
//         attAction_btn.GetComponent<Button>().onClick.AddListener(() => actionMethod(parameter)());
//         attAction_btn.transform.SetParent(panel.transform); 
//     }


//     private void ActivateActionPanel(GameObject actionPanel)
//     {
//         selectActionPanel.SetActive(false);
//         actionPanels.SetActive(true);

//         // Hide previous panel if any
//         if (currentActionPanel != null && currentActionPanel != actionPanel)
//             currentActionPanel.SetActive(false);
        

//         actionPanel.SetActive(true);
//         currentActionPanel = actionPanel;

//         // Check if buttons are already created for this panel
//         bool buttonsCreated = false;
//         if (actionPanel == wattAction_panel && wattActionBtnCreated)
//             buttonsCreated = true;
        
//         else if (actionPanel == mattAction_panel && mattActionBtnCreated)
//             buttonsCreated = true;
        
//         else if (actionPanel == buffAction_Panel && buffActionBtnCreated)
//             buttonsCreated = true;
        

//         // If buttons are not already created, create them
//         if (!buttonsCreated)
//         {
//             // Clear existing buttons in the panel
//             foreach (Transform child in actionPanel.transform)
//                 Destroy(child.gameObject);
            

//             // Create buttons based on the action panel
//             if (actionPanel == wattAction_panel)
//             {
//                 CreateActionButtonsForHero(actionPanel, baseHero.physicalAttacks, actionType_btn);
//                 wattActionBtnCreated = true;
//             }
//             else if (actionPanel == mattAction_panel)
//             {
//                 CreateActionButtonsForHero(actionPanel, baseHero.magicAttacks, actionType_btn);
//                 mattActionBtnCreated = true;
//             }
//             else if (actionPanel == buffAction_Panel)
//             {
//                 CreateActionButtonsForHero(actionPanel, baseHero.buffs, actionType_btn);
//                 buffActionBtnCreated = true;
//                 Debug.Log("buff buttons created");
//             }
//         }

//         // Ensure visibility of each action panel
//         wattAction_panel.SetActive(actionPanel == wattAction_panel);
//         mattAction_panel.SetActive(actionPanel == mattAction_panel);
//         buffAction_Panel.SetActive(actionPanel == buffAction_Panel);
//     }

//     private void CreateActionButtonsForHero(GameObject panel, List<BaseAction> actions, GameObject actionTypeBtnPrefab)
//     {
//         foreach (BaseAction action in actions)
//         {
//             GameObject actionButton = Instantiate(actionTypeBtnPrefab, panel.transform);
//             actionButton.transform.SetParent(panel.transform);

//             TextMeshProUGUI buttonText = actionButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
//             buttonText.text = action.actionName;

//             // Add a listener to the button here (if needed)
//             actionButton.GetComponent<Button>().onClick.AddListener(() =>
//             {
//                 // Handle button click logic, e.g., choosing this action
//                 Debug.Log("Clicked on action: " + action.actionName);
//             });
//         }
//     }

//     public void GoBackToSelectActionPanel()
//     {
//         if (currentActionPanel != null)
//         {
//             currentActionPanel.SetActive(false);
//             currentActionPanel = null; // Reset current action panel

//             selectActionPanel.SetActive(true);
//         }
//         actionPanels.SetActive(false);
//     }

// }


