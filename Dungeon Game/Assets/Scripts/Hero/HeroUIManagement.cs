using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HeroUIManagement : HeroStats
{
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

    [Header("Buttons")]
    [SerializeField] private Button heroSelectBtn;
    [SerializeField] private Button heroDeselectBtn;
    [SerializeField] private Button toBattlePanelBtn;
    [SerializeField] private Button closeBattlePanelBtn;

    [Header("Panels")]
    public Transform heroStatsPanel = null;
    public Transform battlePanel = null;
    public Transform buttonInputContainer = null;
    public GameObject heroPanelSelector = null;

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
    }

    private void InitializeButtons()
    {
        heroSelectBtn = heroPanelUI.transform.GetChild(3).GetComponent<Button>();
        heroDeselectBtn = heroPanelUI.transform.GetChild(4).GetComponent<Button>();
        toBattlePanelBtn = heroStatsPanel.transform.GetChild(2).GetComponent<Button>();
        closeBattlePanelBtn = battlePanel.transform.GetChild(3).GetComponent<Button>();
    }

    public IEnumerator AssignHeroUIStatsElements(MonoBehaviour monoBehaviour)
    {
        if (!heroStatsPanel)
        {
            heroStatsPanel = heroPanelUI.transform.GetChild(0); // Hero stats panel

            // Assign hero stats UI elements
            Transform infoPanel = heroStatsPanel.GetChild(0).GetChild(0);
            Transform imagePanel = heroStatsPanel.GetChild(0).GetChild(1);

            lvl_text = GetTextComponent(infoPanel, new int[] { 0, 1 }); // Lvl
            heroName_text = GetTextComponent(infoPanel, new int[] { 1 }); // Name
            Image image = imagePanel.GetChild(0).GetComponent<Image>();
            image.sprite = icon;

            Transform statsPanel = heroStatsPanel.GetChild(1).GetChild(0).GetChild(1);

            hp_text = GetTextComponent(statsPanel, new int[] { 0, 1, 0 });
            mp_text = GetTextComponent(statsPanel, new int[] { 1, 1, 0 });
            watt_text = GetTextComponent(statsPanel, new int[] { 2, 1, 0 });
            matt_text = GetTextComponent(statsPanel, new int[] { 3, 1, 0 });
            weaponDef_text = GetTextComponent(statsPanel, new int[] { 4, 1, 0 });
            magicDef_text = GetTextComponent(statsPanel, new int[] { 5, 1, 0 });

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
            heroStatsPanel.gameObject.SetActive(false);
            battlePanel.gameObject.SetActive(true);

            closeBattlePanelBtn.onClick.RemoveAllListeners();
            closeBattlePanelBtn.onClick.AddListener(() => CloseBattlePanel());
        }
    }

    public void CloseBattlePanel()
    {
        Debug.Log("Closing battle panel...");
        battlePanel.gameObject.SetActive(false);
        heroStatsPanel.gameObject.SetActive(true);
        heroDeselectBtn.gameObject.SetActive(true); 
        isBattlePanelOpen = false;
    }

    private void CreateChooseActionToPerformButtons() 
    {

    }

    private void CreateActionTypeButton() 
    {
        // Create dynaically a button
        // So either an attack button or a defense button, etc.
    }

    public IEnumerator CreateTargetButtons(List<DictionaryEntry<string, Button>> dictionaryEntry, GameObject buttonPrefab)
    {
        // Wait for initialization to be done
        yield return new WaitForSeconds(1f);

        if (targetButtonsCreated) yield break; // Prevent multiple runs

        if (BattleManager.instance.enemiesInBattle.Count <= 0)
        {
            Debug.LogError("No enemies found!");
            yield break;
        }

        // Loop through the enemies in battle
        for (int i = 0; i < BattleManager.instance.enemiesInBattle.Count; i++)
        {
            // Store the enemy
            var enemy = BattleManager.instance.enemiesInBattle[i];

            // Only create if button hasn't been created yet
            if (!enemy.Value.hasTargetButtonCreated)
            {
                Debug.Log("Creating target button...");

                // Instantiate
                GameObject targetButton = Object.Instantiate(buttonPrefab);
                // GameObject targetButton = Object.Instantiate(uIBattleManager.buttonPrefab);
                targetButton.transform.SetParent(buttonInputContainer);
                targetButton.name = enemy.Value.enemyStats.Name;
                
                // Fetch the button compontent
                if (targetButton.TryGetComponent<Button>(out var btn))
                {
                    // Fetch the Text component
                    TextMeshProUGUI buttonText = targetButton.GetComponentInChildren<TextMeshProUGUI>();

                    if (buttonText == null)
                    {
                        Debug.LogError("Couldn't fetch the TextUGUI component");
                        yield break;
                    }

                    buttonText.text = enemy.Value.enemyStats.Name;
                    
                    // Add the button to the dictionary
                    dictionaryEntry.Add(new DictionaryEntry<string, Button>
                    {
                        Key = targetButton.name,
                        Value = btn
                    });
        

                    // dictionaryEntry.Add(entry);
                    // uIBattleManager.targetButtonsEntry.Add(new DictionaryEntry<string, Button>
                    // {
                    //     Key = targetButton.name,
                    //     Value = btn
                    // });

                    enemy.Value.hasTargetButtonCreated = true;
                    break; // Stop after creating one button
                }
                else
                {
                    Debug.LogError("Couldn't fetch the Button component");
                    yield break;
                }
            }
        }

        // Add listener to the buttons
        foreach (var buttonEntry in dictionaryEntry)
        {
            buttonEntry.Value.onClick.RemoveAllListeners(); 
            buttonEntry.Value.onClick.AddListener(() => SelectTarget(dictionaryEntry));
        }
        // foreach (var buttonEntry in uIBattleManager.targetButtonsEntry)
        // {
        //     buttonEntry.Value.onClick.RemoveAllListeners(); 
        //     buttonEntry.Value.onClick.AddListener(() => SelectTarget(uIBattleManager));
        // }

        targetButtonsCreated = true; 
    }

    public void SelectTarget(List<DictionaryEntry<string, Button>> dictionaryEntry)
    {
        Debug.Log("Select target button is pressed!");

        // Ensure the button works only once
        foreach (var entry in dictionaryEntry)
        {
            if (entry.Value == UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>())
            {
                GameObject button = entry.Value.gameObject;

                foreach (var element in BattleManager.instance.enemiesInBattle)
                {
                    if (button.name == element.Value.enemyStats.Name)
                    {
                        BattleManager.instance.enemyToAttack = element.Value;
                        return; // Exit after selection to prevent multiple triggers
                    }
                }
            }
        }
    }

    // public void SelectTarget(UIBattleManager uIBattleManager)
    // {
    //     Debug.Log("Select target button is pressed!");

    //     // Ensure the button works only once
    //     foreach (var entry in uIBattleManager.targetButtonsEntry)
    //     {
    //         if (entry.Value == UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>())
    //         {
    //             GameObject button = entry.Value.gameObject;

    //             foreach (var element in BattleManager.instance.enemiesInBattle)
    //             {
    //                 if (button.name == element.Value.enemyStats.Name)
    //                 {
    //                     BattleManager.instance.enemyToAttack = element.Value;
    //                     return; // Exit after selection to prevent multiple triggers
    //                 }
    //             }
    //         }
    //     }
    // }
}
