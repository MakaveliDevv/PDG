using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroManager : MonoBehaviour
{
    public HeroUIManagement heroUIManager;
    private HeroMovement heroMovement;
    private Rigidbody2D rb;
    // private bool inRangeForBattle;
    public float timeToMove = .2f;
    private bool targetButtonsCreated = false;

    void Awake() 
    {
        heroMovement = new();
        rb = GetComponent<Rigidbody2D>();
        heroUIManager.CustomAwake();
    }
    
    void Update() 
    {  
        InputManagment();
        Swag();
    }

    private void Swag () 
    {
        // If battle scene is active
        if(SceneManager.GetActiveScene().name == "BattleScene2") 
        {
            // StartCoroutine(heroUIManager.CreateTargetButtons(BattleManager.instance.UIBattleManager));
            StartCoroutine(heroUIManager.CreateTargetButtons(BattleManager.instance.UIBattleManager.targetButtonsEntry, BattleManager.instance.UIBattleManager.buttonPrefab));
            heroUIManager.SelectHero();
        }   
    }

    public void InputManagment()
    {
        // Movement
        if(Input.GetKey(KeyCode.UpArrow) && !heroMovement.isMoving) 
        {
            StartCoroutine(PlayerMovement(Vector3.up));
        }

        if(Input.GetKey(KeyCode.LeftArrow) && !heroMovement.isMoving) 
        {
            StartCoroutine(PlayerMovement(Vector3.left));
        }

        if(Input.GetKey(KeyCode.DownArrow) && !heroMovement.isMoving) 
        {
            StartCoroutine(PlayerMovement(Vector3.down));
        }

        if(Input.GetKey(KeyCode.RightArrow) && !heroMovement.isMoving) 
        {
            StartCoroutine(PlayerMovement(Vector3.right));
        }
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
            // inRangeForBattle = true;

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
                bool entryExists = GameManager.instance.enemiesEncounterd.Any(e => e.Value == enemy);

                // Add the entry only if it doesn't already exist
                if (!entryExists) 
                {
                    GameManager.instance.enemiesEncounterd.Add(entry);
                    Debug.Log("Added new enemy to enemiesToBattle list.");

                    GameManager.instance.gameState = GameManager.GameState.BATTLE;

                }
                else
                {
                    Debug.Log("Enemy already exists in the enemiesToBattle list.");
                }

                // heroUIManager.CreateSelectTargetButtons();  // Create buttons if needed
                // StartCoroutine(heroUIManager.CreateSelectTargetButtons());
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
            // inRangeForBattle = false;
        }
    }
}
