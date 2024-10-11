using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MAINMENU,
        DUNGEON,
        BATTLE,
        ENDGAME
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
            gameState = GameState.DUNGEON;
        }
    }

    void Update() 
    {        
        switch(gameState)
        {
            case(GameState.DUNGEON):
                gameTimerActive = true;
                battleTimerActive = false;

                inBattle = false;

            break;

            case(GameState.BATTLE):
                // Start battle
                StartBatlle();
                
                // StartBattleScene();

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

    private void StartBattleScene() 
    {
        SceneManager.LoadScene("BattleScene");
    }

    private void StartBatlle() 
    {
        gameTimerActive = false;
        battleTimerActive = true;
        inBattle = true;

        // Open battle UI
    }
}
