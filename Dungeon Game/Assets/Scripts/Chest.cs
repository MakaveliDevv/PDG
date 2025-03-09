using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains multiple items and gives the player a random item upon interaction.
/// </summary>
public class Chest : MonoBehaviour
{
    public List<Item> items = new(); // List of items in the chest
    public bool inRange;

    private void Update()
    {
        if(inRange) 
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Item randomItem = GetRandomItem();
                if (randomItem != null)
                {
                    Debug.Log("You received: " + randomItem.itemName);
                }
                else
                {
                    Debug.Log("The chest is empty!");
                }
            }
        }
    }

    private Item GetRandomItem()
    {
        if (items.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, items.Count);
        Item selectedItem = items[randomIndex];

        Destroy(gameObject, .5f);
        return selectedItem;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Hero")) 
        {
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Hero")) 
        {
            inRange = false;
        }
    }
}
