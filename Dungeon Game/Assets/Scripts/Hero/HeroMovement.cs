using System.Collections;
using UnityEngine;

public class HeroMovement
{
    public bool isMoving;
    public Vector2 initialPos;

    public Vector2 GetTargetPosition(Rigidbody2D rb, Vector2 direction) 
    {
        initialPos = rb.position;
        return initialPos + direction;
    }

    public void MoveToPosition(Rigidbody2D rb, Vector2 targetPos, float elapsedTime, float timeToMove) 
    {
        rb.MovePosition(Vector3.Lerp(initialPos, targetPos, elapsedTime / timeToMove));
    }
}
