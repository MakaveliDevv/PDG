using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        DUNGEON_STATE,
        BATTLE_STATE,
        IDLE_STATE
    }
    public GameState gameState;

    #region Singleton
    public static GameManager instance;

    void Awake()
    {
        if(instance == null) 
            instance = this;

        else if(instance != null)
            Destroy(gameObject);
        

        DontDestroyOnLoad(gameObject);

        // if(!GameObject.Find("HeroCharacter")) 
        // {
        //     GameObject hero = Instantiate(heroCharacter, nextHeroPosition, Quaternion.identity) as GameObject;
        //     hero.name = "HeroCharacter";
        // }
    }

    #endregion

    // Hero
    public GameObject heroCharacter;

    // Bool
    public bool isWalking, canGetEncounter, gotAttacked;
    public bool inBattle;

    // Battle
    public List<GameObject> enemiesToBattle = new();
    public int enemyAmount;

    // Gamemanager stuff
    [Header("GameManagementStuff")]
    // public TextMeshProUGUI timerText; 
    public bool gameTimerActive; 
    public float elapsedGameplayTime = 0f;

    public bool battleTimerActive;
    public float elapsedBattleTime = 0f;
    // public bool menuIsOpen;

    void Start() 
    {   
        if(IsSceneActive("DungeonScene"))
        {
            gameState = GameState.DUNGEON_STATE;
        }
    }

    void Update() 
    {        
        switch(gameState)
        {
            case(GameState.DUNGEON_STATE):
                gameTimerActive = true;
                battleTimerActive = false;

                inBattle = false;

            break;

            case(GameState.BATTLE_STATE):
                gameTimerActive = false;
                battleTimerActive = true;
                
                inBattle = true;

            break;
        }

        UpdateTimer(gameTimerActive, ref elapsedGameplayTime);
        UpdateTimer(battleTimerActive, ref elapsedBattleTime);
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
}
