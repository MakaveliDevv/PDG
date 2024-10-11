using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    public bool inRangeForBattle;
    private void OnTriggerEnter2D(Collider2D collider) 
    {
        if(collider.CompareTag("Enemy"))
        {
            Debug.Log("Made collision with the enemy");
            inRangeForBattle = true;
            
            // Start Battle 

            GameManager.instance.gameState = GameManager.GameState.BATTLE;
            GameManager.instance.enemiesToBattle.Add(collider.gameObject);
        }
    }
}
    