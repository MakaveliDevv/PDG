using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Tutorial Stuff")]
    public GameObject introTextForBattle;
    public GameObject selectHeroText;
    public GameObject openBattlePanelText;
    public GameObject selectTargetText;
    public GameObject selectActionText;
    public GameObject selectAttackText;
    public GameObject performAttackText;
    public GameObject actionPerformedText;
    public GameObject defeated;

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

        foreach (var e in heroesInBattle)
        {
            // Turn on the canvas
            GameObject character = e.Key;
            GameObject canvas = character.transform.GetChild(transform.childCount - 1).gameObject;
            canvas.SetActive(true);

            SpriteRenderer spriteRenderer = e.Value.GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = false;
        }

        foreach (var e in enemiesInBattle)
        {
            // Turn on the canvas
            GameObject character = e.Key;
            GameObject canvas = character.transform.GetChild(transform.childCount - 1).gameObject;
            canvas.SetActive(true);

            SpriteRenderer spriteRenderer = e.Value.GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = true;
        }

        StartCoroutine(InitializeBattleText());
    }

    private IEnumerator InitializeBattleText() 
    {
        if(GameManager.instance.gamePlay == GamePlay.TUTORIAL) 
        {
            if(introTextForBattle == null) introTextForBattle = GameObject.FindGameObjectWithTag("IntroTextForBattle");
            
            if(selectHeroText == null) selectHeroText = GameObject.FindGameObjectWithTag("SelectHeroText");

            yield return new WaitForSeconds(1f);

            // StartCoroutine(GameManager.instance.tutScript.DisplayBattleText(introTextForBattle, selectHeroText));
        }

        yield break;
    }

    void Update()
    {
        InitializeActionToPerformListener();

        if(heroesInBattle.Count <= 0) 
        {
            StartCoroutine(GameOver());
        } 
        else if(enemiesInBattle.Count <= 0) 
        {
            StartCoroutine(ReturnToDungeon());
        }
    }

    private void InitializeActionToPerformListener() 
    {
        foreach (var _hero in heroesInBattle)
        {
            if(_hero.Value.heroUIManager.actionButtonsCreated) 
            {
                _hero.Value.heroUIManager.SelectWeaponAttActionToPerform(this, _hero.Key);
                // _hero.Value.heroUIManager.SelectMattActionToPerform(this, _hero.Key);
            }
        }
    }

    private IEnumerator ReturnToDungeon() 
    {
        yield return new WaitForSeconds(2f);

        for (int i = BattleData.instance.heroesToBattle.Count - 1; i >= 0; i--)
        {
            var entry = BattleData.instance.heroesToBattle[i];
            // Destroy(entry.Key); 
            BattleData.instance.heroesToBattle.RemoveAt(i);
        }

        for (int i = BattleData.instance.enemiesToBattle.Count - 1; i >= 0; i--)
        {
            var entry = BattleData.instance.enemiesToBattle[i];
            // Destroy(entry.Key);
            BattleData.instance.enemiesToBattle.RemoveAt(i);
        }

        BattleData.instance.heroesToBattle.Clear();
        BattleData.instance.enemiesToBattle.Clear();

        // Debug.Log("Defeated all the enemies, returning to the dungeon");
        yield return new WaitForSeconds(2f);
        
        SceneManager.LoadScene("DungeonScene");
        GameManager.instance.gameState = GameManager.GameState.DUNGEON;
    }


    private IEnumerator GameOver() 
    {
        // for (int i = BattleData.instance.heroesToBattle.Count - 1; i >= 0; i--)
        // {
        //     var entry = BattleData.instance.heroesToBattle[i];
        //     Destroy(entry.Key);
        //     BattleData.instance.heroesToBattle.RemoveAt(i);
        // }

        // for (int i = BattleData.instance.enemiesToBattle.Count - 1; i >= 0; i--)
        // {
        //     var entry = BattleData.instance.enemiesToBattle[i];
        //     Destroy(entry.Key);
        //     BattleData.instance.enemiesToBattle.RemoveAt(i);
        // }

        Debug.Log("Game Over!");
        yield return new WaitForSeconds(2f);

        // display defeated ui
        defeated.SetActive(true);

        yield return new WaitForSeconds(2f);

        defeated.SetActive(false);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("GameOver");
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
            _hero.Value.heroUIManager.CreateTargetButtons(UIBattleManager.targetButtonsEntry, UIBattleManager.buttonPrefab, this);
            _hero.Value.heroUIManager.SelectHero(this);
            _hero.Value.heroUIManager.SelectAction(this);
            _hero.Value.heroUIManager.SelectAttack(this);
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
