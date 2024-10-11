using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public bool isMoving;
    Vector3 initialPos, targetPos;
    private Vector2 curPosition, lastPosition;
    public float timeToMove = .2f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    } 

    void FixedUpdate() 
    {
        curPosition = transform.position;

        if(curPosition == lastPosition) 
        {
            GameManager.instance.isWalking = false;

        } else 
        {
            GameManager.instance.isWalking = true;
        }

        lastPosition = curPosition;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.W) && !isMoving) 
        {
            StartCoroutine(MovePlayer(Vector3.up));
        }

        if(Input.GetKey(KeyCode.A) && !isMoving) 
        {
            StartCoroutine(MovePlayer(Vector3.left));
        }

        if(Input.GetKey(KeyCode.S) && !isMoving) 
        {
            StartCoroutine(MovePlayer(Vector3.down));
        }

        if(Input.GetKey(KeyCode.D) && !isMoving) 
        {
            StartCoroutine(MovePlayer(Vector3.right));
        }
    }

    private IEnumerator MovePlayer(Vector3 _direction) 
    {
        isMoving = true;

        float elapsedTime = 0f;

        initialPos = transform.position;
        targetPos = initialPos + _direction;

        while(elapsedTime < timeToMove) 
        {
            rb.MovePosition(Vector3.Lerp(initialPos, targetPos, (elapsedTime / timeToMove)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPos);

        isMoving = false;
    }
}
