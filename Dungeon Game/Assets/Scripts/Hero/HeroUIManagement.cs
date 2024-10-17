using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Create a new HeroPanelStats to assign it to the hero
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
    public bool isPanelOpen;

    public Transform heroStuff;
    public Transform battlePanel;
    public Transform buttonInputContainer;
    public bool battlePanelOpened; 
    public bool isheroSelected;

    public void CustomStart() 
    {
        battlePanel = heroPanelUI.transform.GetChild(1);
        buttonInputContainer = battlePanel.GetChild(0).GetChild(1);
    }

    public void AssignHeroUIElements() 
    {
        heroStuff = heroPanelUI.transform.GetChild(0);
        Transform infoPanel = heroStuff.transform.GetChild(0).GetChild(0); // HeroID -> Info
        Transform imagePanel = heroStuff.transform.GetChild(0).GetChild(1); // HeroID -> Image Panel 

        lvl_text = GetTextComponent(infoPanel, new int[] {0, 1}); // Lvl
        heroName_text = GetTextComponent(infoPanel, new int[] {1}); // Name
        Image image = imagePanel.transform.GetChild(0).gameObject.GetComponent<Image>();
        image.sprite = icon; 
    
        Transform statsPanel = heroStuff.GetChild(1).GetChild(0).GetChild(1); // HeroStats -> Stats -> statsPanel
        
        hp_text = GetTextComponent(statsPanel, new int[] {0, 1, 0});
        mp_text = GetTextComponent(statsPanel, new int[] {1, 1, 0});
        watt_text = GetTextComponent(statsPanel, new int[] {2, 1, 0});
        matt_text = GetTextComponent(statsPanel, new int[] {3, 1, 0});
        weaponDef_text = GetTextComponent(statsPanel, new int[] {4, 1, 0});
        magicDef_text = GetTextComponent(statsPanel, new int[] {5, 1, 0});

        AssignStats();
    }

    private TextMeshProUGUI GetTextComponent(Transform parent, int[] path) 
    {
        foreach (var index in path)
        {
            parent = parent.GetChild(index);
        }

        return parent.GetComponent<TextMeshProUGUI>();
    }

    public void AssignStats() 
    {   
        hp_text.text = currentHP.ToString();
        mp_text.text = currentMP.ToString();

        heroName_text.text = name.ToString();
        lvl_text.text = level.ToString();

        hp_text.text = currentHP.GetValue().ToString(); // HP
        mp_text.text = currentMP.GetValue().ToString(); // MP

        watt_text.text = weaponAttack.GetValue().ToString(); // Weapon Attack
        matt_text.text = magicAttack.GetValue().ToString(); // Magic Attack

        weaponDef_text.text = weaponDEF.GetValue().ToString(); // Weapon def
        magicDef_text.text = magicDEF.GetValue().ToString(); // Magic def
    }
    

    public void ToggleHeroPanel() 
    {
        // Toggle panel logic
        isPanelOpen = !isPanelOpen;

        // Close Panel
        if(heroPanelUI.activeInHierarchy)
        {
            heroPanelUI.SetActive(false);
            isPanelOpen = false;
            battlePanelOpened = false;
        }
        else 
        {
            // Open Panel
            heroPanelUI.SetActive(true);
            isPanelOpen = true;
        }
    }

    public void SelectHero() 
    {
        if(!isheroSelected) 
        {
            Button btn = heroStuff.transform.GetChild(2).GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            
            // Go through the list of Heroes
            for (int i = 0; i < GameManager.instance.heroes.Count; i++)
            {
                // Get the name of the heroes
                var hero = GameManager.instance.heroes.ElementAt(i);
                
                // Compare this name with that name
                if(name == hero.Value.name)
                {
                    // Only then add the listener 
                    btn.onClick.AddListener(() => 
                    {
                        Debug.Log($"{name} is equal to {hero.Value.name}");
                    });
                }
            }

            isheroSelected = true;
        }
    }

    // To battle panel button
    public void ToggleBattlePanel() 
    {
        if(!battlePanelOpened) 
        {        
            Button btn = heroStuff.transform.GetChild(2).GetComponent<Button>();
            
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => 
            {
                heroStuff.gameObject.SetActive(false);
                battlePanel.gameObject.SetActive(true);
            });

            battlePanelOpened = true;
        }
        else if(battlePanelOpened) 
        {
            // Get button
            Button btn2 = battlePanel.transform.GetChild(2).GetChild(1).GetComponent<Button>();
            btn2.onClick.RemoveAllListeners();
            btn2.onClick.AddListener(() => 
            {
                battlePanel.gameObject.SetActive(false);
                heroStuff.gameObject.SetActive(true);
            });
        }
    }

    // Create buttons method
    public void CreateSelectTargetButtons() 
    {
        // Transform buttonInputContainer = battlePanel.GetChild(0).GetChild(1);
         
        for (int i = 0; i < GameManager.instance.enemiesToBattle.Count; i++)
        {
            var enemy = GameManager.instance.enemiesToBattle[i];
            GameObject buttonObject = GameObject.Instantiate(UIManager.instance.buttonPrefab);
            buttonObject.name = enemy.Value.enemyStats.name;
            buttonObject.transform.SetParent(buttonInputContainer);

            if(buttonObject.TryGetComponent<Button>(out var btn)) 
            {
                var entry = new DictionaryEntry<string, Button>
                {
                    Key = buttonObject.name,
                    Value = btn
                };

                if(!UIManager.instance.targetButtonsEntry.Contains(entry)) 
                {
                    UIManager.instance.targetButtonsEntry.Add(entry); 
                }

                break;
            }
        }

        // Add listener to the buttons
        for (int i = 0; i < UIManager.instance.targetButtonsEntry.Count; i++)
        {
            var button = UIManager.instance.targetButtonsEntry.ElementAt(i);
            button.Value.onClick.AddListener(() => SelectTarget());
        }
    }

    public void SelectTarget() 
    {
        // Loop through the target buttons list
        for (int i = 0; i < UIManager.instance.targetButtonsEntry.Count; i++)
        {
            var entry = UIManager.instance.targetButtonsEntry[i];

            // Fetch the button 
            GameObject button = entry.Value.gameObject;

            // Check if the button name is the same as one of the enemies in battle
            foreach (var element in GameManager.instance.enemiesToBattle)
            {
                if(button.name == element.Value.enemyStats.name) 
                {
                    // Store the enemy as selected enemy to attack
                    GameManager.instance.EnemyToAttack = element.Value;

                    break;
                }
            }
        }
    }

    // When target selected, create a 'target selector' to visualize it

    // So when clicking the target button, store the enemy game object as the target to attack
}
