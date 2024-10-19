using System.Collections;
using System.Linq;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    public HeroUIManagement heroUIManager;
    private HeroMovement heroMovement;
    private GameObject hero;
    private Rigidbody2D rb;
    private bool inRangeForBattle;
    public float timeToMove = .2f;

    void Awake() 
    {
        heroMovement = new();
        rb = GetComponent<Rigidbody2D>();
        hero = gameObject;
        heroUIManager.CustomAwake();
    }

    void Start() 
    {
        heroUIManager.CustomStart();
    }

    void Update() 
    {
        if(inRangeForBattle) 
        {
            heroUIManager.SelectHero();
        }
        
        InputManagment();
    }

    public void InputManagment()
    {
        // Movement
        if(Input.GetKey(KeyCode.W) && !heroMovement.isMoving) 
        {
            StartCoroutine(PlayerMovement(Vector3.up));
        }

        if(Input.GetKey(KeyCode.A) && !heroMovement.isMoving) 
        {
            StartCoroutine(PlayerMovement(Vector3.left));
        }

        if(Input.GetKey(KeyCode.S) && !heroMovement.isMoving) 
        {
            StartCoroutine(PlayerMovement(Vector3.down));
        }

        if(Input.GetKey(KeyCode.D) && !heroMovement.isMoving) 
        {
            StartCoroutine(PlayerMovement(Vector3.right));
        }

        // Toggle hero panel
        // if(Input.GetKeyDown(KeyCode.Tab)) 
        // {
        //     heroUIManager.ToggleHeroPanel();
        // }

        // if(heroUIManager.isPanelOpen) 
        // {
        //     heroUIManager.ToggleBattlePanel();
        // }
    }

    private IEnumerator PlayerMovement(Vector2 direction) 
    {
        heroMovement.isMoving = true;
        float elapsedTime = 0f;

        Vector2 targetPos = heroMovement.GetTargetPosition(rb, direction);

        while(elapsedTime < timeToMove) 
        {
            heroMovement.MoveToPosition(rb, targetPos, elapsedTime, timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        heroMovement.MoveToPosition(rb, targetPos, timeToMove, timeToMove);
        heroMovement.isMoving = false;
    }

    void OnTriggerEnter2D(Collider2D collider) 
    {
        if(collider.CompareTag("Enemy")) 
        {
            Debug.Log($"Made contact with the {collider.gameObject.name}");
            inRangeForBattle = true;

            // Fetch the EnemyManagement component
            if(collider.TryGetComponent<EnemyManagement>(out var enemy)) 
            {
                Debug.Log("Fetched the EnemyManagement component");

                var entry = new DictionaryEntry<GameObject, EnemyManagement> 
                {
                    Key = collider.gameObject,
                    Value = enemy
                };
    
                // Check if this entry already exists in the enemiesToBattle list
                bool entryExists = GameManager.instance.enemiesToBattle.Any(e => e.Value == enemy);

                // Add the entry only if it doesn't already exist
                if (!entryExists) 
                {
                    GameManager.instance.enemiesToBattle.Add(entry);
                    Debug.Log("Added new enemy to enemiesToBattle list.");

                    GameManager.instance.gameState = GameManager.GameState.BATTLE;

                }
                else
                {
                    Debug.Log("Enemy already exists in the enemiesToBattle list.");
                }

                heroUIManager.CreateSelectTargetButtons();  // Create buttons if needed
            }
            else 
            {
                Debug.Log("No component found!");
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider) 
    {
        if(collider.CompareTag("Enemy")) 
        {
            inRangeForBattle = false;
        }
    }
}
