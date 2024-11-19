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
    [SerializeField] private List<Button> ConfirmButtons = null;

    // Panels
    [Header("Panels")]
    public Transform statsPanel = null;
    public Transform battlePanel = null;
    public Transform buttonInputContainer = null;
    public GameObject heroPanelSelector = null;

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

        Transform selectTargetPanel = battlePanel.GetChild(0);
        Transform actionPanels = battlePanel.GetChild(1);

        // Select Action Panels
        Transform selectActionPanels = actionPanels.GetChild(0);

        Transform selectActionTypePanel = selectActionPanels.GetChild(0); 
        Transform selectAttackTypePanel = selectActionPanels.GetChild(1);
        Transform selectDefenseTypePanel = selectActionPanels.GetChild(2);

        // Perform Action Panels
        Transform performActionPanels = actionPanels.GetChild(1);
        Transform performAttackPanel = performActionPanels.GetChild(0);
        Transform performDefensePanel = performActionPanels.GetChild(1);

        // Perform Attack Panels
        Transform performWeaponAttackPanel = performAttackPanel.GetChild(0); // Watt
        Transform performMagicAttackPanel = performAttackPanel.GetChild(1); // Matt

        // Perform Defense Panels
        Transform performShieldDefPanel = performDefensePanel.GetChild(0);
        Transform performBuffDefPanel = performDefensePanel.GetChild(1);
        Transform performDebuffDefPanel = performDefensePanel.GetChild(2);

        // Initialize Confirm Buttons
        Button confirmButton_SelectTarget = selectTargetPanel.GetChild(2).GetComponent<Button>(); // Select target confirm button
        Button confirmButton_SelectActionType = selectActionTypePanel.GetChild(2).GetComponent<Button>(); // Select ation type confirm button
        Button confirmButton_SelectAttackType = selectAttackTypePanel.GetChild(2).GetComponent<Button>(); // Select attack type confirm button
        Button confirmButton_SelectDefTypePanel = selectDefenseTypePanel.GetChild(2).GetComponent<Button>(); // Select defense type confirm button

        Button confirmButton_WattPanel = performWeaponAttackPanel.GetChild(2).GetComponent<Button>(); // Watt panel
        Button confirmButton_MattPanel = performMagicAttackPanel.GetChild(2).GetComponent<Button>(); // Matt panel     

        Button confirm_ShieldDefPanel = performShieldDefPanel.GetChild(2).GetComponent<Button>(); // Shield def panel
        Button confirm_BuffDefPanel = performBuffDefPanel.GetChild(2).GetComponent<Button>(); // Buff def panel
        Button confirm_DebuffDefPanel = performDebuffDefPanel.GetChild(2).GetComponent<Button>(); // Debuff def panel

    }

    private void InitializeButtons()
    {
        heroSelectBtn = heroPanelUI.transform.GetChild(3).GetComponent<Button>();
        heroDeselectBtn = heroPanelUI.transform.GetChild(4).GetComponent<Button>();
        toBattlePanelBtn = statsPanel.transform.GetChild(2).GetComponent<Button>();
        closeBattlePanelBtn = battlePanel.transform.GetChild(3).GetComponent<Button>();
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

    public void CreateTargetButtons(List<DictionaryEntry<string, Button>> dictionaryEntry, GameObject buttonPrefab)
    {
        if (targetButtonsCreated) return;

        if (BattleManager.instance.enemiesInBattle.Count <= 0)
        {
            Debug.LogError("No enemies found!");
            return;
        }

        foreach (var enemy in BattleManager.instance.enemiesInBattle)
        {
            if (!enemy.Value.hasTargetButtonCreated)
            {
                GameObject targetButton = Object.Instantiate(buttonPrefab, buttonInputContainer);
                targetButton.name = enemy.Value.enemyStats.Name;
                
                // Set up button component and text
                if (targetButton.TryGetComponent<Button>(out var btn))
                {
                    var buttonText = targetButton.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        buttonText.text = enemy.Value.enemyStats.Name;
                    }

                    // Add button to the dictionary
                    dictionaryEntry.Add(new DictionaryEntry<string, Button>
                    {
                        Key = targetButton.name,
                        Value = btn
                    });

                    // Assign a click listener with debug
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() =>
                    {
                        Debug.Log("Button clicked! Attempting to select target.");
                        SelectTarget(dictionaryEntry);
                    });

                    enemy.Value.hasTargetButtonCreated = true;
                }
                else
                {
                    Debug.LogError("Button component missing.");
                }
            }
        }

        targetButtonsCreated = true;
    }

    public void SelectTarget(List<DictionaryEntry<string, Button>> dictionaryEntry)
    {
        var selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        Debug.Log($"Select target button pressed: {selectedButton}");

        if (selectedButton != null)
        {
            foreach (var entry in dictionaryEntry)
            {
                if (entry.Value.gameObject == selectedButton)
                {
                    foreach (var enemy in BattleManager.instance.enemiesInBattle)
                    {
                        if (selectedButton.name == enemy.Value.enemyStats.Name)
                        {
                            BattleManager.instance.targetToAttack = enemy.Value.gameObject;
                            Debug.Log($"Target selected: {enemy.Value.enemyStats.Name}");
                            return;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("No button selected.");
        }
    }

    
}
