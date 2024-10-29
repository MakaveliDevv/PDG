using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public List<DictionaryEntry<GameObject, HeroManager>> heroesInBattle = new();
    public List<DictionaryEntry<GameObject, EnemyManagement>> enemiesInBattle = new();
    public EnemyManagement enemyToAttack;
    public bool battleMode = false;
    

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

        // UIBattleManager.InitializeInstance();

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
        // Initialize hero panel
       StartCoroutine(InitializeHeroPanel());
    }


    private IEnumerator InitializeHeroPanel() 
    {
        yield return new WaitForSeconds(1.5f);  // Initial wait to ensure everything is set up.

        if (heroesInBattle.Count == 0)
        {
            Debug.LogError("No heroes found in heroesInBattle");
            yield break;
        }

        foreach (var hero in heroesInBattle)
        {
            if(hero.Value != null)
            {
                Debug.Log($"{hero.Value} found in the heroes in battle dictionary");
                UIBattleManager.InstantiateHeroPanelUI(hero.Value, this);
            }
            else
            {
                Debug.LogError("Hero entry is null in heroesInBattle.");
            }
        }
    }

}
