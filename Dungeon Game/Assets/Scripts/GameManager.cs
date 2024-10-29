using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum GameState
    {
        MAINMENU,
        DUNGEON,
        BATTLE,
        ENDGAME
    }

    public GameState gameState;

    // Game Management
    [Header("Game Management")]
    [SerializeField] private float elapsedGameplayTime = 0f;
    [SerializeField] private float elapsedBattleTime = 0f;
    public List<DictionaryEntry<GameObject, EnemyManagement>> enemiesEncounterd = new();
    private bool isGameplayTimerActive; 
    private bool isBattleTimerActive;
    

    [Header("Battle Management")]
    public bool battleMode = false;
    
    
    // Hero Management
    [Header("Hero Management")]
    public GameObject mainHeroPrefab; 
    public List<DictionaryEntry<GameObject, HeroManager>> heroes = new();
    // public HeroManager heroToAttackWith;

    
    // Enemy Stuff
    [Header("Enemy Management")]
    public List<GameObject> enemyTypes = new();
    public Transform enemiesInSceneGameObjectContainer;
    public int amountOfEnemiesToGenerate;
    public float checkForSpawnPointRadius = 5f; 
    [HideInInspector] public List<GameObject> enemiesInGame = new();
    [HideInInspector] public int enemyCounter;


    void Awake()
    {
        if(instance != null && instance != this) 
        {
            Destroy(this); // Prevent multiple GameManager instances
        }
        else 
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        // battleManager.UIBattleManager.Initialize();
    }

    void Start() 
    {   
        if(IsSceneActive("DungeonScene"))
        {
            gameState = GameState.DUNGEON;
        }
    }

    void Update() 
    {        
        switch(gameState)
        {
            case GameState.DUNGEON:
                isGameplayTimerActive = true;
                isBattleTimerActive = false;

                // inBattle = false;

            break;

            case GameState.BATTLE:
                // Start battle
                BattleMode();

                // SceneManager.LoadScene("BattleScene2");
                
                // StartBattleScene();

            break;
        }

        UpdateTimer(isGameplayTimerActive, ref elapsedGameplayTime);
        UpdateTimer(isBattleTimerActive, ref elapsedBattleTime);
    }

    void UpdateTimer(bool condition, ref float timer)
    {
        if (condition)
        {
            timer += Time.deltaTime;
            // UpdateTimerText(timer);
        }
    }

    private bool IsSceneActive(string scenename) 
    {
        return SceneManager.GetActiveScene().name == scenename;
    }

    private IEnumerator ChangeToBattleScene() 
    {
        if(!battleMode) 
        {
            battleMode = true;
            if(heroes.Count > 0 && enemiesEncounterd.Count > 0) 
            {
                foreach (var hero in heroes)
                {
                    if(!BattleData.instance.heroesToBattle.Contains(hero)) 
                    {
                        BattleData.instance.heroesToBattle.Add(hero);
                        hero.Key.transform.SetParent(BattleData.instance.transform);

                        GameObject cam = hero.Key.transform.GetChild(0).gameObject;
                        if(cam != null) 
                        {
                            cam.SetActive(false);
                        }
                    }
                }
                
                foreach (var enemy in enemiesEncounterd)
                {
                    if(!BattleData.instance.enemiesToBattle.Contains(enemy)) 
                    {
                        BattleData.instance.enemiesToBattle.Add(enemy);
                        enemy.Key.transform.SetParent(BattleData.instance.transform);
                    }
                }
            }

            yield return new WaitForSeconds(2f);

            SceneManager.LoadScene("BattleScene2");

            isGameplayTimerActive = false;
            isBattleTimerActive = true;

            yield break;

        }


        

        // for (int i = 0; i < heroes.Count; i++)
        // {
        //     var element = heroes.ElementAt(i);
            
        //     // Open the UI
        //     element.Value.heroUIManager.OpenHeroPanelUI();
        // }

        // yield break;
    }

    private void BattleMode()
    {
        StartCoroutine(ChangeToBattleScene());

        // Need a way to pass down the enemies and heroes to the battle scene
        // Or only set the enemies to battle active and deactivate the rest
        
        // if(heroToAttackWith == null) return;

        // heroToAttackWith.heroUIManager.OpenBattlePanel();    
    }

    public void AddToDictionary<TKey, TValue>
    (
        // Dictionary<TKey, TValue> dictionary, 
        TKey key, 
        TValue value,
        List<DictionaryEntry<TKey, TValue>> dictionaryEntry
    ) 
    {
        // if(!dictionary.ContainsKey(key)) 
        // {
        //     dictionary.Add(key, value);
        // }

        var entry = new DictionaryEntry<TKey, TValue> 
        {
            Key = key,
            Value = value
        };

        dictionaryEntry.Add(entry);
    }
}


[System.Serializable]
public class DictionaryEntry<TKey, TValue> 
{
    public TKey Key;
    public TValue Value;
}