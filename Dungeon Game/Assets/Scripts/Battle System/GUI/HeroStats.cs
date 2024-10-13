using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Create a new HeroPanelStats to assign it to the hero
[System.Serializable]
public class HeroStats : CharacterStats
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
    
    // public CharacterStats characterStats;
    
    public void AssignHeroUIElements() 
    {
        // characterStats = new();

        Transform heroStuff = heroPanelUI.transform.GetChild(0);
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
        currentHP = maxHP;
        hp_text.text = currentHP.ToString();
        
        currentMP = maxMP;
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

    private void AssignImage() 
    {

    }

    // public void AssignStats() 
    // {        
    //     heroName_text.text = characterStats.name.ToString();
    //     lvl_text.text = characterStats.level.ToString();

    //     hp_text.text = characterStats.currentHP.GetValue().ToString(); // HP
    //     mp_text.text = characterStats.currentMP.GetValue().ToString(); // MP

    //     watt_text.text = characterStats.weaponAttack.GetValue().ToString(); // Weapon Attack
    //     matt_text.text = characterStats.magicAttack.GetValue().ToString(); // Magic Attack

    //     weaponDef_text.text = characterStats.weaponDEF.GetValue().ToString(); // Weapon def
    //     magicDef_text.text = characterStats.magicDEF.GetValue().ToString(); // Magic def
    // }
}
