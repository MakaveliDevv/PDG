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
    private bool isGameplayTimerActive; 
    private bool isBattleTimerActive;


    // UI Management
    [Header("UI Management")]
    [SerializeField] private UIManager uIManager;


    // Battle Management
    [Header("Battle Management")]
    public List<DictionaryEntry<GameObject, EnemyManagement>>  enemiesToBattle;
    public EnemyManagement enemyToAttack;
    
    
    // Hero Management
    [Header("Hero Management")]
    public GameObject mainHeroPrefab; 
    public List<DictionaryEntry<int, HeroManager>> heroes = new();
    public HeroManager heroToAttackWith;

    
    // Enemy Stuff
    [Header("Enemy Stuff")]
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
            Destroy(gameObject); // Prevent multiple GameManager instances
        }
        else 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        uIManager.Initialize();
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
                StartCoroutine(BattleMode());
                
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

    private IEnumerator BattleMode() 
    {
        isGameplayTimerActive = false;
        isBattleTimerActive = true;

        // Open the UI
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < heroes.Count; i++)
        {
            var element = heroes.ElementAt(i);
            element.Value.heroUIManager.OpenHeroPanelUI();
        }
        
        // Open the battle panel
        if(heroToAttackWith != null) 
        {
            heroToAttackWith.heroUIManager.OpenBattlePanel();
        }
        
        yield break;
    }
}


[System.Serializable]
public class DictionaryEntry<TKey, TValue> 
{
    public TKey Key;
    public TValue Value;
}