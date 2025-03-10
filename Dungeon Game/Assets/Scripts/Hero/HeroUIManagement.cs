using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HeroUIManagement : HeroStats
{
    // HERO PANEL STATS ELEMENTS
    private TextMeshProUGUI heroName_text;
    private TextMeshProUGUI lvl_text;
    private TextMeshProUGUI hp_text;
    private TextMeshProUGUI mp_text;
    private TextMeshProUGUI watt_text;
    private TextMeshProUGUI matt_text;
    private TextMeshProUGUI weaponDef_text;
    private TextMeshProUGUI magicDef_text;
    public Sprite icon;

    public GameObject heroPanelUI; // Will be assigned when a hero gets instantiated into the scene

    // BUTTONS
    private Button heroSelectBtn = null;
    private Button heroDeselectBtn = null;
    private Button toBattlePanelBtn = null;
    private Button closeBattlePanelBtn = null;

    // LIST OF BUTTONS
    private List<Button> selectActionTypeBtns = new();
    private List<Button> selectAttackTypeBtns = new();
    private List<Button> selectDefenseTypeBtns = new();

    // MAIN PANELS
    private Transform statsPanel = null;
    private Transform battlePanel = null;
    private Transform buttonInputContainer = null;
    private GameObject heroPanelSelector = null;

    // SUB PANELS
    private Transform selectTargetPanel = null;
    private Transform actionPanels = null;

    private Transform selectActionPanels = null;

    private Transform selectActionTypePanel = null;
    private Transform selectAttackTypePanel = null;
    private Transform selectDefenseTypePanel = null;

    private Transform performActionPanels = null;
    private Transform performAttackPanel = null;
    private Transform performDefensePanel = null;

    private Transform performWeaponAttackPanel = null;
    private Transform performMagicAttackPanel = null;

    private Transform performShieldDefPanel = null;
    private Transform performBuffDefPanel = null;
    private Transform performDebuffDefPanel = null;

    // CONFIRM BUTTONS
    private Button confirmButton_SelectTarget = null;
    private Button confirmButton_SelectActionType = null;
    private Button confirmButton_SelectAttackType = null;
    private Button confirmButton_SelectDefType = null;

    private Button confirmButton_Watt = null;
    private Button confirmButton_Matt = null;

    private Button confirm_ShieldDef = null;
    private Button confirm_BuffDef = null;
    private Button confirm_DebuffDef = null;

    public List<GameObject> actionButtons = new();

    private bool isBattlePanelOpen = false;
    private bool isHeroSelected = false;
    private bool targetButtonsCreated = false; 
    private bool targetSelected;

    // OTHER STUFF
    private string defenseType = "";
    private bool defenseTypeSelected;

    private string actionType = "";
    private bool actionTypeSelected;
    public bool actionButtonsCreated;

    private string attackType = "";
    private bool attackTypeSelected;

    public string wattActionToPerform = "";
    private bool wattActionToPerformSelected;

    public string mattActionToPerform = "";
    private bool mattActionToPerformSelected;

    private void InitializePanels()
    {
        battlePanel = heroPanelUI.transform.GetChild(1);
        buttonInputContainer = battlePanel.GetChild(0).GetChild(1);
        heroPanelSelector = heroPanelUI.transform.GetChild(2).gameObject;

        selectTargetPanel = battlePanel.GetChild(0);
        actionPanels = battlePanel.GetChild(1);

        // Select Action Panels
        selectActionPanels = actionPanels.GetChild(0);

        selectActionTypePanel = selectActionPanels.GetChild(0); 
        selectAttackTypePanel = selectActionPanels.GetChild(1);
        selectDefenseTypePanel = selectActionPanels.GetChild(2);

        // Perform Action Panels
        performActionPanels = actionPanels.GetChild(1);
        performAttackPanel = performActionPanels.GetChild(0);
        performDefensePanel = performActionPanels.GetChild(1);

        // Perform Attack Panels
        performWeaponAttackPanel = performAttackPanel.GetChild(0); // Watt
        performMagicAttackPanel = performAttackPanel.GetChild(1); // Matt

        // Perform Defense Panels
        performShieldDefPanel = performDefensePanel.GetChild(0);
        performBuffDefPanel = performDefensePanel.GetChild(1);
        performDebuffDefPanel = performDefensePanel.GetChild(2);
    }

    private void InitializeButtons()
    {
        heroSelectBtn = heroPanelUI.transform.GetChild(3).GetComponent<Button>();
        heroDeselectBtn = heroPanelUI.transform.GetChild(4).GetComponent<Button>();
        toBattlePanelBtn = statsPanel.transform.GetChild(2).GetComponent<Button>();
        closeBattlePanelBtn = battlePanel.transform.GetChild(3).GetComponent<Button>();

        // Action type buttons
        selectActionTypeBtns = new()
        {
            selectActionTypePanel.GetChild(1).GetChild(0).gameObject.GetComponent<Button>(), // Select attack btn
            selectActionTypePanel.GetChild(1).GetChild(1).gameObject.GetComponent<Button>() // Select defense btn
        };

        // Attack type buttons
        selectAttackTypeBtns = new()
        {
            selectAttackTypePanel.GetChild(1).GetChild(0).gameObject.GetComponent<Button>(), // Select weapon attack panel btn
            selectAttackTypePanel.GetChild(1).GetChild(1).gameObject.GetComponent<Button>() // Select magic attack panel btn
        };

        // Defense type buttons
        selectDefenseTypeBtns = new()
        {
            selectDefenseTypePanel.GetChild(1).GetChild(0).gameObject.GetComponent<Button>(), // Select shield panel btn
            selectDefenseTypePanel.GetChild(1).GetChild(1).gameObject.GetComponent<Button>(), // Select buff panel btn
            selectDefenseTypePanel.GetChild(1).GetChild(2).gameObject.GetComponent<Button>() // Select debuff panel btn
        };
         
        // Select target confirm button
        confirmButton_SelectTarget = selectTargetPanel.GetChild(2).transform.GetChild(0).GetComponent<Button>(); 
        confirmButton_SelectTarget.name = "ConfirmButton_SelectTarget";

        // Select ation type confirm button
        confirmButton_SelectActionType = selectActionTypePanel.GetChild(2).transform.GetChild(0).GetComponent<Button>(); 
        confirmButton_SelectActionType.name = "ConfirmButton_SelectActionType";

        // Select attack type confirm button
        confirmButton_SelectAttackType = selectAttackTypePanel.GetChild(2).transform.GetChild(0).GetComponent<Button>(); 
        confirmButton_SelectAttackType.name = "ConfirmButton_SelectAttackType";

        // Select defense type confirm button
        confirmButton_SelectDefType = selectDefenseTypePanel.GetChild(2).transform.GetChild(0).GetComponent<Button>(); 
        confirmButton_SelectDefType.name = "ConfirmButton_SelectDefType";
        
        // Watt panel
        confirmButton_Watt = performWeaponAttackPanel.GetChild(2).transform.GetChild(0).GetComponent<Button>(); 
        confirmButton_Watt.name = "ConfirmButton_Watt";
        
        // Matt panel
        confirmButton_Matt = performMagicAttackPanel.GetChild(2).transform.GetChild(0).GetComponent<Button>();      
        confirmButton_Matt.name = "ConfirmButton_Matt";

        // Shield def panel
        confirm_ShieldDef = performShieldDefPanel.GetChild(2).transform.GetChild(0).GetComponent<Button>(); 
        confirm_ShieldDef.name = "ConfirmButton_ShieldDef";
        
        // Buff def panel
        confirm_BuffDef = performBuffDefPanel.GetChild(2).transform.GetChild(0).GetComponent<Button>(); 
        confirm_BuffDef.name = "ConfirmButton_BuffDef";
        
        // Debuff def panel
        confirm_DebuffDef = performDebuffDefPanel.GetChild(2).transform.GetChild(0).GetComponent<Button>(); 
        confirm_DebuffDef.name = "ConfirmButon_DebuffDef";
    }

    public IEnumerator AssignHeroUIStatsElements(MonoBehaviour monoBehaviour)
    {
        if (!statsPanel)
        {
            statsPanel = heroPanelUI.transform.GetChild(0); // Hero stats panel

            // Assign hero stats UI elements
            Transform infoPanel = statsPanel.GetChild(0).GetChild(0);
            Transform imagePanel = statsPanel.GetChild(0).GetChild(1);

            lvl_text = GetTextComponent(infoPanel, new int[] { 0, 1 }); // Lvl
            heroName_text = GetTextComponent(infoPanel, new int[] { 1 }); // Name
            Image image = imagePanel.GetChild(0).GetComponent<Image>();
            image.sprite = icon;

            // Transform statsPanel = heroStatsPanel.GetChild(1).GetChild(0).GetChild(1);
            Transform panelTransform = statsPanel.GetChild(1).GetChild(0).GetChild(1);

            hp_text = GetTextComponent(panelTransform, new int[] { 0, 1, 0 });
            mp_text = GetTextComponent(panelTransform, new int[] { 1, 1, 0 });
            watt_text = GetTextComponent(panelTransform, new int[] { 2, 1, 0 });
            matt_text = GetTextComponent(panelTransform, new int[] { 3, 1, 0 });
            weaponDef_text = GetTextComponent(panelTransform, new int[] { 4, 1, 0 });
            magicDef_text = GetTextComponent(panelTransform, new int[] { 5, 1, 0 });

            monoBehaviour.StartCoroutine(AssignStats());

            InitializePanels();
            InitializeButtons();

            yield break;
        }
        else
        {
            Debug.LogError("The hero stats panel already exists, something went wrong");
        }
    }

    public IEnumerator AssignStats()
    {
        // Debug.Log("Assigning stats...");
        yield return new WaitForSeconds(.5f);

        hp_text.text = currentHP.ToString();
        mp_text.text = currentMP.ToString();
        heroName_text.text = Name.ToString();
        lvl_text.text = level.ToString();
        hp_text.text = currentHP.GetValue().ToString();
        mp_text.text = currentMP.GetValue().ToString();
        watt_text.text = weaponAttack.GetValue().ToString();
        matt_text.text = magicAttack.GetValue().ToString();
        weaponDef_text.text = weaponDEF.GetValue().ToString();
        magicDef_text.text = magicDEF.GetValue().ToString();
    }

    private TextMeshProUGUI GetTextComponent(Transform parent, int[] path)
    {
        foreach (var index in path)
        {
            parent = parent.GetChild(index);
        }

        return parent.GetComponent<TextMeshProUGUI>();
    }
    
    public void OpenHeroPanelUI()
    {
        if (!heroPanelUI.activeInHierarchy)
        {
            heroPanelUI.SetActive(true);
            // isHeroPanelOpen = true;
        }
        else if (heroPanelUI == null)
        {
            Debug.LogWarning("Panel not found");
        }
    }

    public void SelectHero(MonoBehaviour mono)
    {
        if (heroSelectBtn != null)
        {
            heroSelectBtn.onClick.RemoveAllListeners(); 

            heroSelectBtn.onClick.AddListener(() =>
            {
                if(BattleManager.instance.turnState == TurnState.HERO_TURN)
                {
                    if (!isHeroSelected)
                    {
                        // Debug.Log("Select hero...");
                        HeroManager newHero = null;

                        // Loop through the heroes
                        for (int i = 0; i < GameManager.instance.heroes.Count; i++)
                        {
                            var hero = GameManager.instance.heroes.ElementAt(i);

                            // Check if the name matches
                            if (Name == hero.Value.heroUIManager.Name)
                            {
                                newHero = hero.Value;
                                break; 
                            }
                        }

                        if (newHero != null)
                        {
                            isHeroSelected = true;
                            heroPanelSelector.SetActive(false);
                            toBattlePanelBtn.gameObject.SetActive(true);
                            heroSelectBtn.gameObject.SetActive(false);
                            heroDeselectBtn.gameObject.SetActive(true);

                            // Activate the toBattlePanelBtn and set its listener
                            toBattlePanelBtn.gameObject.SetActive(true);
                            toBattlePanelBtn.onClick.RemoveAllListeners();
                            toBattlePanelBtn.onClick.AddListener(() => OpenBattlePanel(mono));

                            heroDeselectBtn.onClick.RemoveAllListeners();
                            heroDeselectBtn.onClick.AddListener(() => DeselectHero());
                            // Debug.Log($"{newHero.name} has been selected.");                  
                        }
                        else
                        {
                            Debug.LogWarning("No matching hero found.");
                        }
                    }
                }
            });
        }
    
    }

    private void DeselectHero()
    {
        // Debug.Log("Deselecting the hero");
        isHeroSelected = false; // Reset hero selection
        
        // Deactivate the battle button and hide the battle panel
        toBattlePanelBtn.gameObject.SetActive(false);
        heroDeselectBtn.gameObject.SetActive(false);
        heroSelectBtn.gameObject.SetActive(true);
        heroPanelSelector.SetActive(true);
    }

    private void OpenBattlePanel(MonoBehaviour mono)
    {
        if (!isBattlePanelOpen) 
        {
            // Debug.Log("Battle Panel Open...");
            isBattlePanelOpen = true;
            heroDeselectBtn.gameObject.SetActive(false);
            statsPanel.gameObject.SetActive(false);

            battlePanel.gameObject.SetActive(true);
            selectTargetPanel.gameObject.SetActive(true);
            

            closeBattlePanelBtn.onClick.RemoveAllListeners();
            closeBattlePanelBtn.onClick.AddListener(() => CloseBattlePanel());

            // if(GameManager.instance.gamePlay == GamePlay.TUTORIAL) 
            // {
            //     mono.StartCoroutine(GameManager.instance.tutScript.DisplayText(BattleManager.instance.selectTargetText, 2.25f));
            // }
        }
    }

    private void CloseBattlePanel()
    {
        // Debug.Log("Closing battle panel...");
        battlePanel.gameObject.SetActive(false);
        statsPanel.gameObject.SetActive(true);
        heroDeselectBtn.gameObject.SetActive(true);

        isBattlePanelOpen = false;
    }

    public void CreateTargetButtons(List<DictionaryEntry<string, GameObject>> dictionaryEntry, GameObject buttonPrefab, MonoBehaviour mono)
    {   
        if (targetButtonsCreated) return; // If  button already created, return

        // Check for enemies
        if (BattleManager.instance.enemiesInBattle.Count <= 0)
        {
            Debug.LogError("No enemies found!");
            return;
        }

        // Loop through the dictionary
        foreach (var element in BattleManager.instance.enemiesInBattle)
        {
            // If button hasn't been created yet (just a double check)
            if (!element.Value.hasTargetButtonCreated)
            {
                // Create button
                element.Value.hasTargetButtonCreated = true;
                
                // Create target button
                GameObject targetButton = Object.Instantiate(buttonPrefab, buttonInputContainer);
                
                // Assign the enemy name to the target button
                targetButton.name = element.Value.enemyStats.Name;
                
                // Fetch button component and text
                if (targetButton.TryGetComponent<Button>(out var _targetButton))
                {
                    var buttonText = targetButton.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        // Assign the name of the enemy to the UI text
                        buttonText.text = element.Value.enemyStats.Name;
                    }
                    
                    // Add button to the dictionary if not already present
                    var newEntry = new DictionaryEntry<string, GameObject> 
                    {
                        Key = targetButton.name,
                        Value = _targetButton.gameObject
                    };

                    if (!dictionaryEntry.Any(entry => entry.Key == targetButton.name))
                    {
                        dictionaryEntry.Add(newEntry);
                    }

                    // if(!dictionaryEntry.Contains(newEntry)) 
                    // {
                    //     dictionaryEntry.Add(newEntry);
                    // }

                    // Assign click listener
                    _targetButton.onClick.RemoveAllListeners();
                    _targetButton.onClick.AddListener(() => SelectTarget(mono));
                }
                else
                {
                    Debug.LogError("Button component missing.");
                }
            }
        }

        targetButtonsCreated = true;
    }
    
#pragma warning disable IDE0059 // Unnecessary assignment of a value
    private void ChangeButtonColor(Button button) 
    {
        ColorBlock cb = button.colors;
        cb.normalColor = Color.blue;
        button.colors = cb;
    }
#pragma warning restore IDE0059 // Unnecessary assignment of a value

    private void SelectTarget(MonoBehaviour mono)
    {
        var selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if(selectedButton == null)
        {
            Debug.LogError("No select button found!");
            return;
        }

        // Find the enemy matching the button name
        foreach (var enemy in BattleManager.instance.enemiesInBattle)
        {
            // enemy.Key.transform.GetChild(0).gameObject.SetActive(false);

            if(selectedButton.name == enemy.Value.name)
            {
                if(BattleManager.instance.targetToAttack == null) 
                {
                    // Set the selected target in the BattleManager
                    BattleManager.instance.targetToAttack = enemy.Value.gameObject;

                    // Show the selector UI on the target
                    enemy.Key.transform.GetChild(0).gameObject.SetActive(true);

                    ChangeButtonColor(confirmButton_SelectTarget);

                    targetSelected = true;
                }
                else { return; }
            }
        }       

        // Confirm Target
        confirmButton_SelectTarget.onClick.RemoveAllListeners();
        confirmButton_SelectTarget.onClick.AddListener(() => ConfirmTarget(mono));
    }

    private void ConfirmTarget(MonoBehaviour mono) 
    {
        if(targetSelected) 
        {
            // Deactivate the select target panel
            selectTargetPanel.gameObject.SetActive(false);

            // Activate the action panels
            actionPanels.gameObject.SetActive(true);
            selectActionPanels.gameObject.SetActive(true);
            selectActionTypePanel.gameObject.SetActive(true);

            // if(GameManager.instance.gamePlay == GamePlay.TUTORIAL) 
            // {
            //     mono.StartCoroutine(GameManager.instance.tutScript.DisplayText(BattleManager.instance.selectActionText, 2.25f));
            // }
        }
    }

    public void SelectAction(MonoBehaviour mono) 
    {
        selectActionTypeBtns[0].onClick.RemoveAllListeners();
        selectActionTypeBtns[1].onClick.RemoveAllListeners();

        selectActionTypeBtns[0].onClick.AddListener(() => SelectActionType(mono));
        selectActionTypeBtns[1].onClick.AddListener(() => SelectActionType(mono));
    }

    private void SelectActionType(MonoBehaviour mono) 
    {
        var selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if(selectedButton == null) 
        {
            Debug.LogError("No select button found!");
            return;
        }

        // Debug.Log($"Button clicked: {selectedButton.name}");


        // Fetch the action type from the text
        GameObject textGo = selectedButton.transform.GetChild(0).gameObject;
        TextMeshProUGUI text = textGo.GetComponent<TextMeshProUGUI>();

        // Debug.Log($"Previous actionType: {actionType}");
        actionType = text.text.Trim(); 
        // Debug.Log($"New actionType: {actionType}");


        // Change the color of the button
        ChangeButtonColor(confirmButton_SelectActionType);

        actionTypeSelected = true;

        confirmButton_SelectActionType.onClick.RemoveAllListeners();
        confirmButton_SelectActionType.onClick.AddListener(() => ConfirmActionType(actionType, mono));
    
    }

    private void ConfirmActionType(string actionType, MonoBehaviour mono) 
    {
        if(actionTypeSelected) 
        {
            // Debug.Log($"Selected Action Type: {actionType}");
            if(selectAttackTypePanel != null && selectDefenseTypePanel != null)
            {
                // Activate the right panel
                List<GameObject> panels = new()
                {
                    selectAttackTypePanel.gameObject,
                    selectDefenseTypePanel.gameObject
                };

                // Debug.Log($"{panels[0].name}, {panels[1].name}");

                foreach (var panel in panels)
                {
                    // Debug.Log($"Panels name: {panel.name}");
                    
                    if(panel.name.Contains(actionType)) 
                    {
                        // Debug.Log($"Action type: {actionType}");
                        panel.SetActive(true);
                    }
                }

                // Deactivate the 'select action type panel'
                selectActionTypePanel.gameObject.SetActive(false);
                // actionTypeSelected = false;
            }
            else 
            {
                Debug.LogError("No action type panels are assigned! (SelectAttackTypePanel and SelectDefenseTypePanel)");
            }
            
            // if(GameManager.instance.gamePlay == GamePlay.TUTORIAL) 
            // {
            //     mono.StartCoroutine(GameManager.instance.tutScript.DisplayText(BattleManager.instance.selectAttackText, 2.25f));
            // }
        }
    }

    public void SelectAttack(MonoBehaviour mono) 
    {
        selectAttackTypeBtns[0].onClick.RemoveAllListeners();
        selectAttackTypeBtns[1].onClick.RemoveAllListeners();

        selectAttackTypeBtns[0].onClick.AddListener(() => SelectAttackType(mono));
        selectAttackTypeBtns[1].onClick.AddListener(() => SelectAttackType(mono));
    }

    private void SelectAttackType(MonoBehaviour mono) 
    {
        var selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if(selectedButton == null) 
        {
            Debug.LogError("No select button found!");
            return;
        }

        // Debug.Log($"Button clicked: {selectedButton.name}");

        // Fetch the text component
        GameObject textGo = selectedButton.transform.GetChild(0).gameObject;
        TextMeshProUGUI text = textGo.GetComponent<TextMeshProUGUI>();
        
        // Debug.Log($"Previous attackType: {attackType}");
        attackType = text.text.Trim(); 
        // Debug.Log($"New attackType: {attackType}");

        ChangeButtonColor(confirmButton_SelectAttackType);

        attackTypeSelected = true;

        confirmButton_SelectAttackType.onClick.RemoveAllListeners();
        confirmButton_SelectAttackType.onClick.AddListener(() => ConfirmAttackType(attackType, mono));
    }

    private void ConfirmAttackType(string attackType, MonoBehaviour mono)
    {
        if(attackTypeSelected) 
        {
            // Debug.Log($"Selected Action Type: {attackType}");

            if(performActionPanels != null && performAttackPanel != null && performWeaponAttackPanel != null && performMagicAttackPanel != null) 
            {
                List<GameObject> panels = new() 
                {
                    performWeaponAttackPanel.gameObject,
                    performMagicAttackPanel.gameObject
                };

                // Debug.Log($"{panels[0].name}, {panels[1].name}");

                foreach (var panel in panels)
                {
                    if(panel.name.Contains(attackType)) 
                    {
                        panel.SetActive(true);

                        if(attackType.Contains("Weapon")) 
                        {
                            CreateActionButton(physicalAttacks, panel.transform);

                        }
                        else if(attackType.Contains("Magic")) 
                        {
                            CreateActionButton(magicAttacks, panel.transform);
                        }
                    }
                }

                // Deactivate the current panels
                selectAttackTypePanel.gameObject.SetActive(false);
                selectActionPanels.gameObject.SetActive(false);

                // Activate perform action panels
                performActionPanels.gameObject.SetActive(true);
                performAttackPanel.gameObject.SetActive(true);

                // attackTypeSelected = false;

                // if(GameManager.instance.gamePlay == GamePlay.TUTORIAL) 
                // {
                //     mono.StartCoroutine(GameManager.instance.tutScript.DisplayText(BattleManager.instance.performAttackText, 2.25f));
                // }

            } else { Debug.Log($"Something is missing: PerformActionPanels -> {performActionPanels}, PerformAttackPanel -> {performAttackPanel}, PerformWeaponAttackPanel -> {performWeaponAttackPanel}"); }

        }
        else { return; }
    }

    public void SelectDefense() 
    {
        selectDefenseTypeBtns[0].onClick.RemoveAllListeners();
        selectDefenseTypeBtns[1].onClick.RemoveAllListeners();
        selectDefenseTypeBtns[2].onClick.RemoveAllListeners();

        selectDefenseTypeBtns[0].onClick.AddListener(SelectDefenseType);
        selectDefenseTypeBtns[1].onClick.AddListener(SelectDefenseType);
        selectDefenseTypeBtns[2].onClick.AddListener(SelectDefenseType);
    }

    private void SelectDefenseType() 
    {
        var selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if(selectedButton == null) 
        {
            Debug.LogError("No select button found!");
            return;
        }

        Debug.Log($"Button clicked: {selectedButton.name}");

        // Fetch the text component
        GameObject textGo = selectedButton.transform.GetChild(0).gameObject;
        TextMeshProUGUI text = textGo.GetComponent<TextMeshProUGUI>();
        
        Debug.Log($"Previous defenseType: {defenseType}");
        defenseType = text.text.Trim(); 
        Debug.Log($"New defenseType: {defenseType}");

        ChangeButtonColor(confirmButton_SelectDefType);

        defenseTypeSelected = true;

        confirmButton_SelectDefType.onClick.RemoveAllListeners();
        confirmButton_SelectDefType.onClick.AddListener(() => ConfirmDefenseType(defenseType));
    }

    private void ConfirmDefenseType(string defenseType) 
    {
        if(defenseTypeSelected) 
        {
            Debug.Log($"Selected Action Type: {defenseType}");

            if(performActionPanels != null && performDefensePanel != null) 
            {
                List<GameObject> panels = new() 
                {
                    performShieldDefPanel.gameObject,
                    performBuffDefPanel.gameObject,
                    performDebuffDefPanel.gameObject
                };

                Debug.Log($"Panel -> {panels[0].name}, Panel -> {panels[1].name}, Panel -> {panels[2].name}");

                foreach (var panel in panels)
                {
                    if(panel.name.Contains(defenseType)) 
                    {
                        panel.SetActive(true);
                        
                        if(defenseType.Contains("Shield")) 
                        {
                            CreateActionButton(shieldAbilities, panel.transform);

                        }
                        else if(attackType.Contains("Buff")) 
                        {
                            CreateActionButton(buffs, panel.transform);
                        }
                    }
                }

                // Deactivate the current panels
                selectActionPanels.gameObject.SetActive(false);
                selectDefenseTypePanel.gameObject.SetActive(false);
              
                // Activate perform action panels
                performActionPanels.gameObject.SetActive(true);
                performDefensePanel.gameObject.SetActive(true);
              
                // defenseTypeSelected = false;

            } else { Debug.Log($"Something is missing: PerformActionPanels -> {performActionPanels}, PerformAttackPanel -> {performAttackPanel}, PerformWeaponAttackPanel -> {performWeaponAttackPanel}"); }

        }
        else { return; }
    }

    public void CreateActionButton(List<BaseAction> baseActions, Transform panel) 
    {
        Transform input = panel.GetChild(1);

        foreach (var element in baseActions)
        {
            string newButtonName = element.Name;
            bool buttonExists = false;

            foreach (var btn in actionButtons)
            {
                if (btn != null && btn.name == newButtonName)
                {
                    buttonExists = true;
                    break; 
                }
            }

            if (!buttonExists)
            {
                GameObject btn = Object.Instantiate(BattleManager.instance.UIBattleManager.buttonPrefab);
                btn.name = newButtonName;
                btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = newButtonName;
                btn.transform.SetParent(input);

                actionButtons.Add(btn);
            }
        }

        actionButtonsCreated = true;
    }

    public void SelectWeaponAttActionToPerform(MonoBehaviour mono, GameObject hero) 
    {
        // Debug.Log("SelectWeaponAttActionToPerform method executed...");

        foreach (var button in actionButtons)
        {
            // Debug.Log("In the foreach loop");
            Button btn = button.GetComponent<Button>();

            // Debug.Log($"Button name: {btn.name}");

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => SelectPerformWattAction(mono, hero));
        }
    }

    private void SelectPerformWattAction(MonoBehaviour mono, GameObject hero) 
    {
        var selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        // Fetch the text component
        TextMeshProUGUI text = selectedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        wattActionToPerform = text.text;

        wattActionToPerformSelected = true;

        confirmButton_Watt.onClick.RemoveAllListeners();
        confirmButton_Watt.onClick.AddListener(() =>ConfirmWeaponAttackAction(mono, hero));
    }

    private void ConfirmWeaponAttackAction(MonoBehaviour mono, GameObject hero) 
    {
        if(wattActionToPerformSelected)
        {
            foreach (var action in physicalAttacks) 
            {
                if(wattActionToPerform.Contains(action.name.ToString())) 
                {
                    action.PerformAction(mono, hero, BattleManager.instance.targetToAttack.transform);
                    BattleManager.instance.targetToAttack.transform.GetChild(0).gameObject.SetActive(false);
                }
                else 
                {
                    Debug.LogWarning("No such name found!");
                }
            }

            // if(GameManager.instance.gamePlay == GamePlay.TUTORIAL) 
            // {
            //     mono.StartCoroutine(GameManager.instance.tutScript.DisplayText(BattleManager.instance.actionPerformedText, 3f));
            // }

            mono.StartCoroutine(WaitBeforeTurningState());
        }
    }

    // Magic att
    public void SelectMattActionToPerform(MonoBehaviour mono, GameObject hero) 
    {
        // Debug.Log("SelectWeaponAttActionToPerform method executed...");

        foreach (var button in actionButtons)
        {
            // Debug.Log("In the foreach loop");
            Button btn = button.GetComponent<Button>();

            // Debug.Log($"Button name: {btn.name}");

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => SelectPerformMattAction(mono, hero));
        }
    }

    private void SelectPerformMattAction(MonoBehaviour mono, GameObject hero) 
    {
        var selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        // Fetch the text component
        TextMeshProUGUI text = selectedButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        mattActionToPerform = text.text;

        mattActionToPerformSelected = true;

        confirmButton_Matt.onClick.RemoveAllListeners();
        confirmButton_Matt.onClick.AddListener(() =>ConfirmMattAction(mono, hero));
    }

    private void ConfirmMattAction(MonoBehaviour mono, GameObject hero) 
    {
        if(mattActionToPerformSelected)
        {
            foreach (var action in magicAttacks) 
            {
                if(mattActionToPerform.Contains(action.name.ToString())) 
                {
                    action.PerformAction(mono, hero, BattleManager.instance.targetToAttack.transform);
                    BattleManager.instance.targetToAttack.transform.GetChild(0).gameObject.SetActive(false);
                }
                else 
                {
                    Debug.LogWarning("No such name found!");
                }
            }

            // if(GameManager.instance.gamePlay == GamePlay.TUTORIAL) 
            // {
            //     mono.StartCoroutine(GameManager.instance.tutScript.DisplayText(BattleManager.instance.actionPerformedText, 3f));
            // }

            mono.StartCoroutine(WaitBeforeTurningState());
        }
    }

    private IEnumerator WaitBeforeTurningState() 
    {
        yield return new WaitForSeconds(2f);

        // Panels
        performAttackPanel.gameObject.SetActive(false);
        performActionPanels.gameObject.SetActive(false);
        actionPanels.gameObject.SetActive(false);
        battlePanel.gameObject.SetActive(false);
        
        // Button
        toBattlePanelBtn.gameObject.SetActive(false);
        
        // Bools
        // targetSelected = false;
        isBattlePanelOpen = false;
        defenseTypeSelected = false;
        attackTypeSelected = false;
        actionTypeSelected = false;
        actionButtonsCreated = false;
        wattActionToPerformSelected = false;
        isHeroSelected = false;

        statsPanel.gameObject.SetActive(true);
        heroSelectBtn.gameObject.SetActive(true);
        heroPanelSelector.SetActive(true);

        yield return new WaitForSeconds(1f);

        BattleManager.instance.turnState = TurnState.ENEMY_TURN;
    }
}
