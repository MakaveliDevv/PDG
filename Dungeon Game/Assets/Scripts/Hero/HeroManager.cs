using System.Collections;
using System.Linq;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    public HeroUIManagement heroUIManager;
    private HeroMovement heroMovement;
    private Rigidbody2D rb;
    public float timeToMove = .2f;
    public bool enemyEncounter;

    void Awake() 
    {
        heroMovement = new();
        rb = GetComponent<Rigidbody2D>();
        heroUIManager.CustomAwake(gameObject);
    }

    void Start()
    {
        if(GameManager.instance.heroes.Count == 0) 
        {
            var entry = new DictionaryEntry<GameObject, HeroManager> 
            {
                Key = gameObject,
                Value = this
            };

            GameManager.instance.heroes.Add(entry);
        }
    }

    void Update() 
    {  
        InputManagment();
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
        // Check for enemy
        if(collider.CompareTag("Enemy")) 
        {
            // Check if game is in tutorial mode
            if(GameManager.instance.gamePlay == GamePlay.TUTORIAL && GameManager.instance.gameState != GameManager.GameState.BATTLE) 
            {
                StartCoroutine(GameManager.instance.tutScript.DisplayEnemyEncounterUI());
            }

            enemyEncounter = true;

            // Start the encounter logic
            StartCoroutine(Encounter(collider));
        }
    }

    private IEnumerator Encounter(Collider2D collider) 
    {
        if(GameManager.instance.gamePlay == GamePlay.TUTORIAL) 
        {
            yield return new WaitForSeconds(2f);
        }

        // Fetch the EnemyManagement component
        if(collider.TryGetComponent<EnemyManagement>(out var enemy)) 
        {
            // Debug.Log("Fetched the EnemyManagement component");

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
                GameManager.instance.gameState = GameManager.GameState.BATTLE;
            }
        }
        else 
        {
            Debug.LogError("Couldnt fetch the component from the enemy");
            yield break;
        }
        

        yield break;
    }
}
