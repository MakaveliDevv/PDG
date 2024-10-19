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
    public EnemyManagement EnemyToAttack;
    private bool isGameplayTimerActive; 
    [SerializeField] private float elapsedGameplayTime = 0f;
    private bool isBattleTimerActive;
    [SerializeField] private float elapsedBattleTime = 0f;

    [Header("UI Management")]
    [SerializeField] private UIManager uIManager;

    [Header("Hero Stuff")]
    public GameObject mainHeroPrefab; 
    public List<DictionaryEntry<int, HeroManager>> heroes = new();

    [Header("Enemy Stuff")]
    public List<DictionaryEntry<GameObject, EnemyManagement>>  enemiesToBattle;
    public List<GameObject> enemyTypes = new();
    [HideInInspector] public List<GameObject> enemiesInGame = new();
    public Transform enemiesInSceneGameObjectContainer;
    public int amountOfEnemiesToGenerate;
    [HideInInspector] public int enemyCounter;
    public float checkForSpawnPointRadius = 5f; 

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
            case(GameState.DUNGEON):
                isGameplayTimerActive = true;
                isBattleTimerActive = false;

                // inBattle = false;

            break;

            case(GameState.BATTLE):
                // Start battle
                BattleMode();
                
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

    private void BattleMode() 
    {
        isGameplayTimerActive = false;
        isBattleTimerActive = true;

        // Open the UI
        for (int i = 0; i < heroes.Count; i++)
        {
            var element = heroes.ElementAt(i);
            GameObject heroUIPanel = element.Value.heroUIManager.heroPanelUI;
            heroUIPanel.SetActive(true);
        }
        
        // inBattle = true;

        // Open battle UI
    }
}


[System.Serializable]
public class DictionaryEntry<TKey, TValue> 
{
    public TKey Key;
    public TValue Value;
}