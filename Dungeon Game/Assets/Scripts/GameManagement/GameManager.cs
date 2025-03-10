using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GamePlay { TUTORIAL, GAMEPLAY }
public class GameManager : MonoBehaviour
{
    public GamePlay gamePlay;
    public static GameManager instance;
    public enum GameState
    {
        DUNGEON,
        BATTLE
    }

    public GameState gameState;

    // Game Management
    [Header("Game Management")]
    public List<DictionaryEntry<GameObject, EnemyManagement>> enemiesEncounterd = new();
    [SerializeField] private float elapsedGameplayTime = 0f;
    [SerializeField] private float elapsedBattleTime = 0f;
    private bool isGameplayTimerActive; 
    private bool isBattleTimerActive;
    

    [Header("Battle Management")]
    public bool battleMode = false;
    
    
    // Hero Management
    [Header("Hero Management")]
    public GameObject mainHeroPrefab; 
    public List<DictionaryEntry<GameObject, HeroManager>> heroes = new();
    
    // Enemy Stuff
    [Header("Enemy Management")]
    public List<GameObject> enemyTypes = new();
    public Transform enemiesInSceneGameObjectContainer;
    public int amountOfEnemiesToGenerate;
    [HideInInspector] public List<GameObject> enemiesInGame = new();
    [HideInInspector] public int enemyCounter;

    [Header("Items Management")]
    public List<GameObject> chest = new();
    public int amountOfItemsToGenerate;
    public float spawnRadius = 5f;

    public TutorialScript tutScript;

    void Awake()
    {
        if(instance != null && instance != this) 
        {
            Destroy(this); 
        }
        else 
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        if(IsSceneActive("DungeonScene"))
        {
            gamePlay = GamePlay.GAMEPLAY;
            gameState = GameState.DUNGEON;
        }

        if(IsSceneActive("DungeonTutorial"))
        {
            gamePlay = GamePlay.TUTORIAL;
            gameState = GameState.DUNGEON;
        }

        tutScript = GetComponent<TutorialScript>();
    }

    void Update() 
    {        
        switch(gameState)
        {
            case GameState.DUNGEON:
                isGameplayTimerActive = true;
                isBattleTimerActive = false;

                battleMode = false;

            break;

            case GameState.BATTLE:
                // Start battle
                BattleMode();

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

            if(gamePlay == GamePlay.GAMEPLAY) 
            {
                SceneManager.LoadScene("BattleScene2");
            } 
            else if(gamePlay == GamePlay.TUTORIAL)
            {
                SceneManager.LoadScene("BattleTutorial");
            }

            isGameplayTimerActive = false;
            isBattleTimerActive = true;

            yield break;

        }
    }

    private void BattleMode()
    {
        StartCoroutine(ChangeToBattleScene());   
    }

    public void AddToDictionary<TKey, TValue>
    (
        // Dictionary<TKey, TValue> dictionary, 
        TKey key, 
        TValue value,
        List<DictionaryEntry<TKey, TValue>> dictionaryEntry
    ) 
    {
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