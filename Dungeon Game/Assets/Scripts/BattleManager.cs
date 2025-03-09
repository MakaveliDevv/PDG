using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState { HERO_TURN, ENEMY_TURN }
public class BattleManager : MonoBehaviour
{
    public TurnState turnState;
    public static BattleManager instance;

    [Header("Heroes & Enemies in battle")]
    public List<DictionaryEntry<GameObject, HeroManager>> heroesInBattle = new();
    public List<DictionaryEntry<GameObject, EnemyManagement>> enemiesInBattle = new();

    // -------------
    private bool heroPanelInitialized = false;
    public GameObject targetToAttack = null;

    [Header("Heroes & Enemies positions")]
    [SerializeField] private List<GameObject> heroesPosition;
    [SerializeField] private List<GameObject> enemeisPosition;
    

    [Header("UI related stuff")]
    public UIBattleManager UIBattleManager; 
    
    private bool _bool;

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
        InitializePosition();

        turnState = TurnState.HERO_TURN;
    }

    void Update()
    {
        if(!_bool) 
        {
            Swag();

            _bool = false;
        }
    }

    private void Swag() 
    {
        foreach (var _hero in heroesInBattle)
        {
            if(_hero.Value.heroUIManager.actionButtonsCreated) 
            {
                _hero.Value.heroUIManager.SelectWeaponAttActionToPerform(this, _hero.Key);
            }
        }
    }

    private void InitializePosition() 
    {
        if(heroesInBattle.Count > 0 && enemeisPosition.Count > 0) 
        {
            if(heroesPosition != null && enemeisPosition != null) 
            {
                // Handle heroes
                for (int i = 0; i < heroesInBattle.Count; i++)
                {
                    heroesInBattle[i].Key.transform.position = heroesPosition[i].transform.position;
                }

                // Handle enemies
                for (int i = 0; i < enemiesInBattle.Count; i++)
                {
                    enemiesInBattle[i].Key.transform.position = enemeisPosition[i].transform.position;
                }
            }
            else { Debug.LogError("Positions need to be assigned!"); }
        }
        else { Debug.LogError("No heroes and/or enemies found!"); }
    }

    private IEnumerator InitializeHeroUI() 
    {
        InitializeHeroPanel();

        yield return new WaitForFixedUpdate();
        
        foreach (var _hero in heroesInBattle)
        {
            _hero.Value.heroUIManager.CreateTargetButtons(UIBattleManager.targetButtonsEntry, UIBattleManager.buttonPrefab);
            _hero.Value.heroUIManager.SelectHero();
            _hero.Value.heroUIManager.SelectAction();
            _hero.Value.heroUIManager.SelectAttack();
            _hero.Value.heroUIManager.SelectDefense();
            // StartCoroutine(_hero.Value.heroUIManager.SelectWeaponAttActionToPerform());
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
                // Debug.Log($"{hero.Value} found in the heroes in battle dictionary");
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
