using System.Collections;
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

    void Update() 
    {
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
        if(Input.GetKeyDown(KeyCode.Tab)) 
        {
            heroUIManager.ToggleHeroPanel();
        }

        if(heroUIManager.isPanelOpen) 
        {
            heroUIManager.ToggleBattlePanel();
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
            inRangeForBattle = true;

            // Fetch the EnemyManagement component
            if(collider.TryGetComponent<EnemyManagement>(out var enemy)) 
            {
                var entry = new DictionaryEntry<GameObject, EnemyManagement> 
                {
                    Key = collider.gameObject,
                    Value = enemy
                };

                GameManager.instance.enemiesToBattle.Add(entry);
                heroUIManager.CreateSelectTargetButtons();
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
