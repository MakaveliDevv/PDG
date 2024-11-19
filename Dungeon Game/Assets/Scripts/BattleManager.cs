using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public List<DictionaryEntry<GameObject, HeroManager>> heroesInBattle = new();
    public List<DictionaryEntry<GameObject, EnemyManagement>> enemiesInBattle = new();
    public bool battleMode = false;

    // -------------
    public bool heroPanelInitialized = false;
    public GameObject targetToAttack = null;
    

    [Header("UI Battle Manager")]
    public UIBattleManager UIBattleManager; 
    

    void Awake() 
    {
        if(instance != null && instance != this) 
        {
            Destroy(this);
        } 
        else
        {
            instance = this;
        }

        // Check
        if(BattleData.instance.heroesToBattle.Count <= 0 && 
        BattleData.instance.enemiesToBattle.Count <= 0) return;

        // Fetch the heroes
        foreach (var hero in BattleData.instance.heroesToBattle)
        {
            heroesInBattle.Add(hero);
        }

        // Fetch the enemies
        foreach (var enemy in BattleData.instance.enemiesToBattle)
        {
            enemiesInBattle.Add(enemy);
        }
    }

    void Start() 
    {
        StartCoroutine(InitializeHeroUI());
    }


    private IEnumerator InitializeHeroUI() 
    {
        InitializeHeroPanel();

        yield return new WaitForEndOfFrame();
        
        foreach (var element in heroesInBattle)
        {
            var hero = element.Value;
            hero.heroUIManager.CreateTargetButtons(UIBattleManager.targetButtonsEntry, UIBattleManager.buttonPrefab);
            hero.heroUIManager.SelectHero();
        }

        yield break;
    }

    private void InitializeHeroPanel() 
    {
        if(heroPanelInitialized) return;

        if (heroesInBattle.Count == 0)
        {
            Debug.LogError("No heroes found in heroesInBattle");
            return;
        }

        foreach (var hero in heroesInBattle)
        {
            if(hero.Value != null)
            {
                Debug.Log($"{hero.Value} found in the heroes in battle dictionary");
                UIBattleManager.InstantiateHeroPanelUI(hero.Value, this);
                heroPanelInitialized = true;
            }
            else
            {
                Debug.LogError("Hero entry is null in heroesInBattle.");
            }
        }
    }
}
