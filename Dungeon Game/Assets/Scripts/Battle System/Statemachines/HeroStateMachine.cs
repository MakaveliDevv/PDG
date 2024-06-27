using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeroStateMachine : MonoBehaviour
{
    private BattleStateMachine BSM; 

    public BaseHero baseHero;
    
    public enum TurnState 
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currentState;
    // private float maxCooldown = 1f;
    // private float curCooldown;

    // public Image progressBar;
    // public GameObject selector;

    // IENumerator
    public GameObject targetToAttack;
    private Vector2 startPosition;
    private bool actionStarted = false;
    private float animSpeed = 10f;

    // Dead
    private bool alive = true;

    // Hero panel
    private HeroPanelStats UI_stats;
    // public GameObject heroPanelUI;
    // private Transform heroPanelSpacer;
    [SerializeField] private Transform heroesPanel;


    [Header("Panels")]
    public GameObject heroPanelUI; // Main hero stats panel
    public GameObject battlePanel; // Main hero battle panel
    public GameObject actionPanel; // Choose action panel (attack, defense, buff)
    public GameObject inputActionPanel; // Choose type of action (if attack: att, matt, if buff: increase stats, if defense: heal, armor, shield, etc)
    public GameObject targetPanel; // Choose target panel
    public GameObject descriptionPanel; // Description about the action (optional)
    public GameObject heroPanelSelector; // Makes the hero panel dark if not selected
    
    [Header("ButtonHeroStatsPanel")]
    public GameObject select_btn; 
    public GameObject battle_btn;

    [Header("Action Panel buttons")]
    public GameObject selectAction_btn;
    public GameObject inputAction_btn; // This one can be either attack, matt or a defense button
    // public GameObject physicalAtt_btn;
    // public GameObject magicalAtt_btn;
    // public GameObject defense_btn;
    public GameObject target_btn;

    void Start()
    {
        currentState = TurnState.ADDTOLIST;
        
        // selector.SetActive(false);
        // heroPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("BattlePanel").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");
        
        BSM = GameObject.FindGameObjectWithTag("BattleManager").GetComponent<BattleStateMachine>(); // Change to instance
        heroesPanel = GameObject.FindGameObjectWithTag("HeroesPanel").transform;
    
        // Create panel
        CreateHeroPanel();

        // currentState = TurnState.PROCESSING;
        startPosition = transform.position;

        battlePanel = heroPanelUI.transform.Find("NewBattlePanel").gameObject;

        actionPanel = battlePanel.transform.Find("NewActionPanel").gameObject;
        inputActionPanel = battlePanel.transform.Find("InputActionPanel").gameObject;
        targetPanel = battlePanel.transform.Find("NewSelectTargetPanel").gameObject;
        heroPanelSelector = heroPanelUI.transform.Find("SelectHeroPanelButton").gameObject;
    }

    void Update()
    {
        switch (currentState)
        {
            case (TurnState.ADDTOLIST):
                BSM.heroesToManage.Add(gameObject);

                currentState = TurnState.WAITING;

            break;

            case (TurnState.WAITING):
                // Idle state
                // When player is choosing an action

            break;

            case (TurnState.ACTION):
                StartCoroutine(TimeForAction());
                
            break;

            case (TurnState.DEAD):
                if(!alive) 
                {
                    return; // Do nothing if hero still alive
                } else 
                {   
                    // Change tag
                    gameObject.tag = "DeadHero";

                    // Not attackable by enemy
                    BSM.herosInBattle.Remove(gameObject);

                    // Not managable
                    BSM.heroesToManage.Remove(gameObject);

                    // Deactivate the selector
                    // selector.SetActive(false);

                    // Reset GUI
                    // BSM.actionPanel.SetActive(false);
                    // BSM.enemySelectPanel.SetActive(false);

                    // Remove inputs from performlist
                    if(BSM.herosInBattle.Count > 0) 
                    {
                        for (int i = 0; i < BSM.performList.Count; i++)
                        {
                            if(i != 0) 
                            {
                                if(BSM.performList[i].performer == gameObject) 
                                {
                                    BSM.performList.Remove(BSM.performList[i]);
                                }

                                // Check if the target of the enemy is this hero
                                if(BSM.performList[i].performersTarget == gameObject) 
                                {
                                    // Change target to random target
                                    BSM.performList[i].performersTarget = BSM.herosInBattle[Random.Range(0, BSM.herosInBattle.Count)];
                                }
                            }
                        }
                    }

                    // Change color / play animation
                    SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.material.color = new Color32(105, 105, 105, 255);
                    }
                    else
                    {
                        Debug.LogError("SpriteRenderer component not found on the game object.");
                    }

                    // Reset hero input
                    BSM.battleStates = BattleStateMachine.BattleStates.CHECKALIVE;
                    alive = false;
                }
            break;
            
            // default:
        }
    }

    // private void ProgressBar() 
    // {
    //     curCooldown += Time.deltaTime;
    //     float calc_cooldown = curCooldown / maxCooldown; // Calculation of the cool down
    //     progressBar.transform.localScale = new Vector3(Mathf.Clamp(calc_cooldown, 0, 1), progressBar.transform.localScale.y, progressBar.transform.localScale.z);

    //     if(curCooldown >= maxCooldown)
    //     {
    //         currentState = TurnState.ADDTOLIST;
    //     }
    // }

    private IEnumerator TimeForAction() 
    {
        if(actionStarted) 
        {
            yield break;            
        }
        
        actionStarted = true;

        // Animate the hero near the enemy to attack
        Vector2 targetPosition = new(targetToAttack.transform.position.x - 1.5f, targetToAttack.transform.position.y);
        while(MoveTowardsTarget(targetPosition)) { yield return null; } // Change while loop to something else

        // Wait a bit
        yield return new WaitForSeconds(1f);

        // Do damage
        DoDamage();

        // Animate back to start position
        Vector2 initialPosition = startPosition;
        while(MoveTowardsTarget(initialPosition)) { yield return null; } // Change while loop to something else

        // Remove this performer from the list in the BSM
        BSM.performList.RemoveAt(0);

        // Reset the battle state to waiting after completing an action
        if(BSM.battleStates != BattleStateMachine.BattleStates.WIN && BSM.battleStates != BattleStateMachine.BattleStates.LOSE) 
        {
            BSM.battleStates = BattleStateMachine.BattleStates.WAIT;

            // Reset the hero state
            // curCooldown = 0f;
            currentState = TurnState.PROCESSING;
        } else 
        {
            // Set the hero state back to waiting (idle)
            currentState = TurnState.WAITING;
        }

        // End coroutine
        actionStarted = false;
    }

    private bool MoveTowardsTarget(Vector2 _targetPosition)
    {
        Vector2 newPosition = Vector2.MoveTowards(transform.position, _targetPosition, animSpeed * Time.deltaTime);
        transform.position = newPosition;
        return newPosition != _targetPosition;
    }

    public void DoDamage() 
    {
        float calc_damage = baseHero.curAtt + BSM.performList[0].chosenAction.actionPhysicalDmg;
        targetToAttack.GetComponent<EnemyStateMachine>().TakeDamge(calc_damage); // Get the esm which represents the hero being attacked
    }

    public void TakeDamge(float _damageAmount) 
    {
        baseHero.curHP -= _damageAmount;
        if(baseHero.curHP <= 0) 
        {
            baseHero.curHP = 0;
            currentState = TurnState.DEAD;
        }

        UpdateHeroPanel();
    }

    private void CreateHeroPanel() 
    {
        heroPanelUI = Instantiate(heroPanelUI);
        UI_stats = heroPanelUI.GetComponent<HeroPanelStats>();

        UI_stats.Name.text = baseHero.TheName.ToString();
        UI_stats.HP.text = baseHero.curHP.ToString();
        UI_stats.MP.text = baseHero.curMP.ToString();

        heroPanelUI.transform.SetParent(heroesPanel, false);
    }

    private void UpdateHeroPanel() 
    {
        UI_stats.HP.text = "HP: " + baseHero.curHP;
        UI_stats.MP.text = "MP: " + baseHero.curMP;
    }
}
