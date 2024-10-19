using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UIManager
{
    public static UIManager instance;
    public GameObject heroesPanel, buttonPrefab;
    public List<DictionaryEntry<HeroManager, GameObject>> heroPanelStatsEntry = new();
    private readonly Dictionary<HeroManager, GameObject> heroPanelStats = new(); // GameObject and the hero panel of the game object
    public List<DictionaryEntry<string, Button>> targetButtonsEntry;

    // [SerializeField] private List<DictionaryEntry<HeroManager, HeroPanelStats>> heroPanelStatsEntry;
    // private Dictionary<HeroManager, HeroPanelStats> heroPanelStats; // GameObject and the hero panel of the game object

    public GameObject heroPanelUI;

    public void Initialize() 
    {
        if(instance != null && instance != this) 
        {
            Object.Destroy(GameManager.instance);
        } 
        else 
        {
            instance = this;
            Object.DontDestroyOnLoad(GameManager.instance);
        }
    }

    // Use this when a hero gets instantiated into the scene
    public void InstantiateHeroPanelUI(HeroManager hero) 
    {
        GameObject newHeroPanel = GameObject.Instantiate(heroPanelUI);
        newHeroPanel.transform.SetParent(heroesPanel.transform);
        newHeroPanel.name = "Hero Panel: " + hero.name;

        AddToDictionary(heroPanelStats, hero, newHeroPanel, heroPanelStatsEntry);
        
        hero.heroUIManager.heroPanelUI = newHeroPanel;
        hero.heroUIManager.AssignHeroUIElements();

        // return newHeroPanel;
    }

    private void AddToDictionary<TKey, TValue>
    (
        Dictionary<TKey, TValue> dictionary, 
        TKey key, 
        TValue value,
        List<DictionaryEntry<TKey, TValue>> dictionaryEntry
    ) 
    {
        if(!dictionary.ContainsKey(key)) 
        {
            dictionary.Add(key, value);
        }

        var entry = new DictionaryEntry<TKey, TValue> 
        {
            Key = key,
            Value = value
        };

        dictionaryEntry.Add(entry);
    }
}
