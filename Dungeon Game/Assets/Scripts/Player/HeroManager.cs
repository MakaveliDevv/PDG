using System.Collections;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    public float timeToMove = .2f;
    public HeroStats heroStats;
    public HeroPanelStats heroPanelStats;
    private HeroMovement heroMovement;
    private GameObject hero;
    private Rigidbody2D rb;
    private bool inRangeForBattle;

    void Awake() 
    {
        heroMovement = new();
        rb = GetComponent<Rigidbody2D>();
        hero = gameObject;
    }

    void Start() 
    {

    }

    void Update() 
    {
        InputManagment();
    }

    public void InputManagment()
    {
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
            inRangeForBattle = true;
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
