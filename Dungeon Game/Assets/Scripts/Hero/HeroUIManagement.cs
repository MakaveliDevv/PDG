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
    [SerializeField] private readonly List<GameObject> targetButtons = new();

   
    [Header("Panels")]
    public Transform heroStatsPanel = null;
    public Transform battlePanel = null;
    public Transform buttonInputContainer = null;
    public GameObject heroPanelSelector = null;

   
    [Header("Booleans")]
    public bool isHeroPanelOpen = false;
    public bool isBattlePanelOpen = false;  
    public bool isheroSelected = false;


    public void CustomStart() 
    {
        // (Need to find a way to move this piece of code to the method below)

        // Panels
        battlePanel = heroPanelUI.transform.GetChild(1);
        buttonInputContainer = battlePanel.GetChild(0).GetChild(1);
        heroPanelSelector = heroPanelUI.transform.GetChild(2).gameObject;

        // Buttons
        heroSelectBtn = heroPanelUI.transform.GetChild(3).gameObject.GetComponent<Button>();
        heroDeselectBtn = heroPanelUI.transform.GetChild(4).gameObject.GetComponent<Button>();
        toBattlePanelBtn = heroStatsPanel.transform.GetChild(2).gameObject.GetComponent<Button>();
        closeBattlePanelBtn = battlePanel.transform.GetChild(3).gameObject.GetComponent<Button>();
    }

    public void AssignHeroUIElements() 
    {
        heroStatsPanel = heroPanelUI.transform.GetChild(0); // Hero stats panel
        Transform infoPanel = heroStatsPanel.transform.GetChild(0).GetChild(0); // HeroID -> Info
        Transform imagePanel = heroStatsPanel.transform.GetChild(0).GetChild(1); // HeroID -> Image Panel 

        lvl_text = GetTextComponent(infoPanel, new int[] {0, 1}); // Lvl
        heroName_text = GetTextComponent(infoPanel, new int[] {1}); // Name
        Image image = imagePanel.transform.GetChild(0).gameObject.GetComponent<Image>();
        image.sprite = icon; 
    
        Transform statsPanel = heroStatsPanel.GetChild(1).GetChild(0).GetChild(1); // HeroStats -> Stats -> statsPanel
        
        hp_text = GetTextComponent(statsPanel, new int[] {0, 1, 0});
        mp_text = GetTextComponent(statsPanel, new int[] {1, 1, 0});
        watt_text = GetTextComponent(statsPanel, new int[] {2, 1, 0});
        matt_text = GetTextComponent(statsPanel, new int[] {3, 1, 0});
        weaponDef_text = GetTextComponent(statsPanel, new int[] {4, 1, 0});
        magicDef_text = GetTextComponent(statsPanel, new int[] {5, 1, 0});

        AssignStats();
    }

    public void AssignStats() 
    {   
        hp_text.text = currentHP.ToString();
        mp_text.text = currentMP.ToString();

        heroName_text.text = Name.ToString();
        lvl_text.text = level.ToString();

        hp_text.text = currentHP.GetValue().ToString(); // HP
        mp_text.text = currentMP.GetValue().ToString(); // MP

        watt_text.text = weaponAttack.GetValue().ToString(); // Weapon Attack
        matt_text.text = magicAttack.GetValue().ToString(); // Magic Attack

        weaponDef_text.text = weaponDEF.GetValue().ToString(); // Weapon def
        magicDef_text.text = magicDEF.GetValue().ToString(); // Magic def
    }

    private TextMeshProUGUI GetTextComponent(Transform parent, int[] path) 
    {
        foreach (var index in path)
        {
            parent = parent.GetChild(index);
        }

        return parent.GetComponent<TextMeshProUGUI>();
    }

    private void ToggleGameObject(Transform parent, int[] path, bool isActive)
    {
        foreach (var index in path)
        {
            parent = parent.GetChild(index);
        }

        GameObject targetObject = parent.gameObject;
        targetObject.SetActive(isActive);
    }
    
    public void OpenHeroPanelUI() 
    {
        // Toggle panel logic
        // isHeroPanelOpen = !isHeroPanelOpen;

        // Close Panel
        if(!heroPanelUI.activeInHierarchy)
        {
            // Open Panel
            heroPanelUI.SetActive(true);
            isHeroPanelOpen = true;
        }
        else if(heroPanelUI == null)
        {
            Debug.LogWarning("Panel not found");
        }
    }

    public void SelectHero() 
    {
        // Create new values
        HeroManager newHero = null;

        if(!isheroSelected) 
        {            
            // Loop through the list of Heroes
            for (int i = 0; i < GameManager.instance.heroes.Count; i++)
            {
                // Get the name of each hero
                var hero = GameManager.instance.heroes.ElementAt(i);

                // If this hero's name is the same as one of the heroes in the Heroes list
                if(Name == hero.Value.heroUIManager.Name)
                {
                    // Then assign the new hero
                    newHero = hero.Value;

                    // Remove existing listeners before adding a new one
                    heroSelectBtn.onClick.RemoveAllListeners();

                    // Add listener to the button
                    heroSelectBtn.onClick.AddListener(() => 
                    {             
                        // Debug.Log($"{Name} is equal to {newHero.heroUIManager.Name}");
           
                        isheroSelected = true;

                        // Disable the background
                        heroPanelSelector.SetActive(false);

                        // Assign the hero to attack with
                        GameManager.instance.heroToAttackWith = newHero;
                        
                        // Activate the battle panel button
                        toBattlePanelBtn.gameObject.SetActive(true);

                        // Debug.Log($"{heroSelectBtn.gameObject.name}");

                        // Deactivate the 'hero select' button at the end
                        heroSelectBtn.gameObject.SetActive(false);  

                        // Activate the deselect button
                        heroDeselectBtn.gameObject.SetActive(true);
                        
                        // Remove existing listeners before adding a new one
                        heroDeselectBtn.onClick.RemoveAllListeners();  

                        heroDeselectBtn.onClick.AddListener(() => DeselectHero()); // Assign the method to deselect the hero                     
                    });

                }
            }
        }
    }

    public void DeselectHero() 
    {
        // Remove the hero to attack with
        GameManager.instance.heroToAttackWith = null;

        // Deactivate the 'to battle panel' button
        toBattlePanelBtn.gameObject.SetActive(false);
        heroDeselectBtn.gameObject.SetActive(false);
        heroSelectBtn.gameObject.SetActive(true);

        // Activate the background of the hero panel UI
        heroPanelSelector.SetActive(true);
    }

    public void OpenBattlePanel() 
    {
        if(isheroSelected && !isBattlePanelOpen) 
        {                    
            toBattlePanelBtn.onClick.RemoveAllListeners();
            toBattlePanelBtn.onClick.AddListener(() => 
            {
                heroDeselectBtn.gameObject.SetActive(false);
                heroStatsPanel.gameObject.SetActive(false);
                battlePanel.gameObject.SetActive(true);
            });
            
            isBattlePanelOpen = true;
        }

        if(isBattlePanelOpen) 
        {
            closeBattlePanelBtn.onClick.RemoveAllListeners();
            
            closeBattlePanelBtn.onClick.AddListener(() => CloseBattlePanel());
        }
    }

    public void CloseBattlePanel() 
    {
        // Deactivate the battle panel
        battlePanel.gameObject.SetActive(false);
        
        // Activate the hero stats panel
        heroStatsPanel.gameObject.SetActive(true);

        // Activate the deselect button again
        heroDeselectBtn.gameObject.SetActive(true);
    }

    // Create buttons method
    public IEnumerator CreateSelectTargetButtons() 
    {
        GameObject targetButton = null;

        // Loop through the enemies to battle list
        for (int i = 0; i < GameManager.instance.enemiesToBattle.Count; i++)
        {
            // Assign the enemy
            var enemy = GameManager.instance.enemiesToBattle[i];
        
            if(!enemy.Value.hasTargetButtonCreated)
            {
                // Create target button
                targetButton = Object.Instantiate(UIManager.instance.buttonPrefab);
                targetButton.transform.SetParent(buttonInputContainer);

                // Name the button
                targetButton.name = enemy.Value.enemyStats.Name;
            
                // Fetch the button component
                if(targetButton.TryGetComponent<Button>(out var btn)) 
                {
                    TextMeshProUGUI buttonText = targetButton.GetComponentInChildren<TextMeshProUGUI>();

                    if(buttonText == null) 
                    {
                        Debug.LogError("Couldn't fetch the TextUGUI component");
                    }
                    
                    buttonText.text = enemy.Value.enemyStats.Name;

                    var entry = new DictionaryEntry<string, Button>
                    {
                        Key = targetButton.name,
                        Value = btn
                    };

                    bool entryExists = UIManager.instance.targetButtonsEntry.Any(e => e.Key == targetButton.name);

                    if(!entryExists) 
                    {
                        UIManager.instance.targetButtonsEntry.Add(entry);
                    }

                    enemy.Value.hasTargetButtonCreated = true;

                    break;
                } 
                else
                {
                    Debug.LogError("Couldn't fetch the Button component");
                }
            }
           
        }

        // Add listener to the buttons
        for (int i = 0; i < UIManager.instance.targetButtonsEntry.Count; i++)
        {
            var button = UIManager.instance.targetButtonsEntry.ElementAt(i);
            button.Value.onClick.AddListener(() => SelectTarget());
        }

        yield break;
    }


    public void SelectTarget() 
    {
        Debug.Log("Select target button is pressed!");

        // Loop through the target buttons list
        for (int i = 0; i < UIManager.instance.targetButtonsEntry.Count; i++)
        {
            var entry = UIManager.instance.targetButtonsEntry[i];

            // Fetch the button object 
            GameObject button = entry.Value.gameObject;

            // Check if the button name is the same as one of the enemies in battle
            foreach (var element in GameManager.instance.enemiesToBattle)
            {
                if(button.name == element.Value.enemyStats.Name) 
                {
                    // Store the enemy as selected enemy to attack
                    GameManager.instance.enemyToAttack = element.Value;

                    break;
                }
            }
        }
    }

    // When target selected, create a 'target selector' to visualize it

    // So when clicking the target button, store the enemy game object as the target to attack
}
