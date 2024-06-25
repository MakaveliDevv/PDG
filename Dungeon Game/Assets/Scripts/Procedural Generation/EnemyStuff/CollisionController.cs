using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    public bool inRangeForBattle;
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Enemy"))
        {
            Debug.Log("Made collision with the enemy");
            inRangeForBattle = true;

            GameManager.instance.gameState = GameManager.GameState.BATTLE_STATE;
            GameManager.instance.enemiesToBattle.Add(other.gameObject);
        }
    }
}
