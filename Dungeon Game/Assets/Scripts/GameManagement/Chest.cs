using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject spriteUI;

    public List<Item> items = new();
    public bool inRange;
    private HeroManager hero;
    private bool hasDisplayedChestDesc;

    private void Update()
    {
        if(GameManager.instance.gamePlay == GamePlay.TUTORIAL) 
        {
            if (inRange && !hasDisplayedChestDesc)             
            {
                hasDisplayedChestDesc = true;
                StartCoroutine(GameManager.instance.tutScript.DisplayChestDescUI());
            }
        }
        
        if(inRange) 
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Item randomItem = GetRandomItem();
                Debug.Log($"Random item: {randomItem}");
                if (randomItem != null)
                {
                    Debug.Log("You received: " + randomItem.itemName);
                    
                    if(randomItem.action.type == "WeaponAttack") 
                    {
                        // Add to the weapon att list
                        hero.heroUIManager.physicalAttacks.Add(randomItem.action);
                    }
                    else if(randomItem.action.type == "MagicAttack") 
                    {
                        // Add to the magic att list
                        hero.heroUIManager.magicAttacks.Add(randomItem.action);
                    }
                }
                else
                {
                    Debug.Log("The chest is empty!");
                }
            }
        }
    }

    // Fetch the sprite from the item
    private IEnumerator DisplaySprite(Item item) 
    {
        spriteUI.SetActive(true);
        SpriteRenderer renderer = spriteUI.GetComponent<SpriteRenderer>();
        renderer.sprite = item.sprite; 

        yield return new WaitForSeconds(2f);
        
        spriteUI.SetActive(false);

        Destroy(gameObject, .5f);

        yield break;
    }

    // Get a random item from the chest
    private Item GetRandomItem()
    {
        if (items.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, items.Count);
        Item selectedItem = items[randomIndex];
        StartCoroutine(DisplaySprite(selectedItem));
        return selectedItem;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Hero")) 
        {
            inRange = true;
            hero = other.gameObject.GetComponent<HeroManager>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Hero")) 
        {
            inRange = false;
            hero = null;
        }
    }
}
