using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HeroUIManagement : HeroStats
{
    // Hero Panel Elements
    [Header("Hero Panel Elements")]
    public TextMeshProUGUI heroName_text;
    public TextMeshProUGUI lvl_text;
    public TextMeshProUGUI hp_text;
    public TextMeshProUGUI mp_text;
    public TextMeshProUGUI watt_text;
    public TextMeshProUGUI matt_text;
    public TextMeshProUGUI weaponDef_text;
    public TextMeshProUGUI magicDef_text;
    public Sprite icon;

    public GameObject heroPanelUI; // Will be assigned when a hero gets instantiated into the scene

    // Buttons
    [Header("Buttons")]
    [SerializeField] private Button heroSelectBtn = null;
    [SerializeField] private Button heroDeselectBtn = null;
    [SerializeField] private Button toBattlePanelBtn = null;
    [SerializeField] private Button closeBattlePanelBtn = null;
    [SerializeField] private List<DictionaryEntry<string, Button>> confirmButtons = new();

    // Panels
    [Header("Panels")]
    public Transform statsPanel = null;
    public Transform battlePanel = null;
    public Transform buttonInputContainer = null;
    public GameObject heroPanelSelector = null;

    [Header("Panels 2")]
    [SerializeField] private Transform selectTargetPanel = null;
    [SerializeField] private Transform actionPanels = null;

    [SerializeField] private Transform selectActionPanels = null;

    [SerializeField] private Transform selectActionTypePanel = null;
    [SerializeField] private Transform selectAttackTypePanel = null;
    [SerializeField] private Transform selectDefenseTypePanel = null;

    [SerializeField] private Transform performActionPanels = null;
    [SerializeField] private Transform performAttackPanel = null;
    [SerializeField] private Transform performDefensePanel = null;

    [SerializeField] private Transform performWeaponAttackPanel = null;
    [SerializeField] private Transform performMagicAttackPanel = null;

    [SerializeField] private Transform performShieldDefPanel = null;
    [SerializeField] private Transform performBuffDefPanel = null;
    [SerializeField] private Transform performDebuffDefPanel = null;

    [Header("Confirm Buttons")]
    [SerializeField] private Button confirmButton_SelectTarget = null;
    [SerializeField] private Button confirmButton_SelectActionType = null;
    [SerializeField] private Button confirmButton_SelectAttackType = null;
    [SerializeField] private Button confirmButton_SelectDefType = null;

    [SerializeField] private Button confirmButton_Watt = null;
    [SerializeField] private Button confirmButton_Matt = null;

    [SerializeField] private Button confirm_ShieldDef = null;
    [SerializeField] private Button confirm_BuffDef = null;
    [SerializeField] private Button confirm_DebuffDef = null;

    // Booleans
    [Header("Booleans")]
    public bool isHeroPanelOpen = false;
    public bool isBattlePanelOpen = false;
    public bool isHeroSelected = false;
    private bool targetButtonsCreated = false; 

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

        // Confirm Buttons

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
        Debug.Log("Assigning stats...");
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
            isHeroPanelOpen = true;
        }
        else if (heroPanelUI == null)
        {
            Debug.LogWarning("Panel not found");
        }
    }

    public void SelectHero()
    {
        if (heroSelectBtn != null)
        {
            heroSelectBtn.onClick.RemoveAllListeners(); 

            heroSelectBtn.onClick.AddListener(() =>
            {
                if (!isHeroSelected)
                {
                    // Debug.Log("Select hero...");
                    HeroManager newHero = null;

                    // Loop through the heroes only when the button is clicked
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
                        Debug.Log($"{Name} is equal to {newHero.heroUIManager.Name}");
                        isHeroSelected = true;
                        heroPanelSelector.SetActive(false);
                        toBattlePanelBtn.gameObject.SetActive(true);
                        heroSelectBtn.gameObject.SetActive(false);
                        heroDeselectBtn.gameObject.SetActive(true);

                        // Activate the toBattlePanelBtn and set its listener
                        toBattlePanelBtn.gameObject.SetActive(true);
                        toBattlePanelBtn.onClick.RemoveAllListeners();
                        toBattlePanelBtn.onClick.AddListener(() =>
                        {
                            if (isHeroSelected)
                            {
                                OpenBattlePanel();
                            }
                        });

                        heroDeselectBtn.onClick.RemoveAllListeners();
                        heroDeselectBtn.onClick.AddListener(() => DeselectHero());
                        Debug.Log($"{newHero.name} has been selected.");                  
                    }
                    else
                    {
                        Debug.LogWarning("No matching hero found.");
                    }
                }
            });
        }
    }

    public void DeselectHero()
    {
        Debug.Log("Deselecting the hero");
        isHeroSelected = false; // Reset hero selection
        
        // Deactivate the battle button and hide the battle panel
        toBattlePanelBtn.gameObject.SetActive(false);
        heroDeselectBtn.gameObject.SetActive(false);
        heroSelectBtn.gameObject.SetActive(true);
        heroPanelSelector.SetActive(true);
    }

    public void OpenBattlePanel()
    {
        if (!isBattlePanelOpen) 
        {
            Debug.Log("Battle Panel Open...");
            isBattlePanelOpen = true;
            heroDeselectBtn.gameObject.SetActive(false);
            statsPanel.gameObject.SetActive(false);
            battlePanel.gameObject.SetActive(true);

            closeBattlePanelBtn.onClick.RemoveAllListeners();
            closeBattlePanelBtn.onClick.AddListener(() => CloseBattlePanel());
        }
    }

    public void CloseBattlePanel()
    {
        Debug.Log("Closing battle panel...");
        battlePanel.gameObject.SetActive(false);
        statsPanel.gameObject.SetActive(true);
        heroDeselectBtn.gameObject.SetActive(true); 
        isBattlePanelOpen = false;
    }

    public void CreateTargetButtons(List<DictionaryEntry<string, GameObject>> dictionaryEntry, GameObject buttonPrefab)
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
                    
                    foreach (var _element in dictionaryEntry)
                    {
                        
                    }
                    // Add button to the dictionary
                    dictionaryEntry.Add(new DictionaryEntry<string, GameObject>
                    {
                        Key = targetButton.name,
                        Value = _targetButton.gameObject
                    });

                    // Assign a click listener with debug
                    _targetButton.onClick.RemoveAllListeners();
                    _targetButton.onClick.AddListener(() =>
                    {
                        Debug.Log($"Enemy: {_targetButton.name} is selected");
                        SelectTarget(dictionaryEntry);
                    });

                }
                else
                {
                    Debug.LogError("Button component missing.");
                }
            }
        }

        targetButtonsCreated = true;
    }

    public void SelectTarget(List<DictionaryEntry<string, GameObject>> dictionaryEntry)
    {
        var selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        Debug.Log($"Select target button pressed: {selectedButton}");

        Debug.Log($"{selectedButton.name}");

        if (selectedButton != null)
        {
            var buttonEntry = new DictionaryEntry<string, GameObject> 
            {
                Key = selectedButton.gameObject.name,
                Value = selectedButton
            };
            
            dictionaryEntry.Add(buttonEntry);

            Debug.Log("Button entry added");

            foreach (var enemy in BattleManager.instance.enemiesInBattle)
            {
                Debug.Log("Inside the foreach loop");
                if (selectedButton.name == enemy.Value.enemyStats.Name)
                {
                    Debug.Log("Selected button name is the same as the enemy name");
                    Button btn = selectedButton.GetComponent<Button>();
                    
                    btn.onClick.AddListener(() => 
                    {
                        BattleManager.instance.targetToAttack = enemy.Value.gameObject;
                        Debug.Log($"Target selected: {enemy.Value.enemyStats.Name}");
                    });
                    
                    return;
                }
            }
        }
        else
        {
            Debug.LogError("No button selected.");
        }
    }
}
