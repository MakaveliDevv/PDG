using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIBattleManager
{
    public GameObject heroPanelUIPrefab;
    public GameObject buttonPrefab, heroesPanel;
    public List<DictionaryEntry<HeroManager, GameObject>> heroPanelStatsEntry = new();
    // private readonly Dictionary<HeroManager, GameObject> heroPanelStats = new();
    public List<DictionaryEntry<string, GameObject>> targetButtonsEntry;
    public GameObject actionSprite;

    public void InstantiateHeroPanelUI(HeroManager hero, MonoBehaviour monoBehaviour) 
    {
        GameObject newHeroPanel = Object.Instantiate(heroPanelUIPrefab);
        newHeroPanel.transform.SetParent(heroesPanel.transform);
        newHeroPanel.name = "Hero Panel: " + hero.name;

        GameManager.instance.AddToDictionary(hero, newHeroPanel, heroPanelStatsEntry);
        
        if(hero.heroUIManager.heroPanelUI == null) 
        {
            hero.heroUIManager.heroPanelUI = newHeroPanel;
            monoBehaviour.StartCoroutine(hero.heroUIManager.AssignHeroUIStatsElements(monoBehaviour));
        }
        else 
        {
            Debug.LogWarning("Hero panel UI already exist");
        }
    }
}
