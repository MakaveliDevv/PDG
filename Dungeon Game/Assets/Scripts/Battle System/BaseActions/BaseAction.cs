using System.Collections;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Action", menuName = "Actions/Ability")]
public class BaseAction : ScriptableObject, IWeaponAttack
{
    public string Name = null;
    public string type;
    public string description = null;
    public float physicalDmg = 0f;
    public float magicDmg = 0f;
    public float manaCost = 0f; // Mana cost
    public float heal = 0f;
    public float physicalDEF = 0f;
    public float magicalDEF = 0f;
    public bool rangeType;

    public Sprite sprite;

    public void PerformAction(MonoBehaviour mono, GameObject go, Transform target)
    {
        Debug.Log($"Action: {Name} performed!");
        mono.StartCoroutine(AnimPlayer(go, target));

        EnemyManagement enemy = target.CompareTag("Enemy") ? target.GetComponent<EnemyManagement>() : null;
        HeroManager hero = target.CompareTag("Hero") ? target.GetComponent<HeroManager>() : null;

        if (enemy == null && hero == null)
        {
            Debug.Log("Target is neither an enemy nor a hero.");
            return;
        }

        // Determine who is the attacker
        EnemyManagement attackerEnemy = go.CompareTag("Enemy") ? go.GetComponent<EnemyManagement>() : null;
        HeroManager attackerHero = go.CompareTag("Hero") ? go.GetComponent<HeroManager>() : null;

        if (enemy != null && attackerHero != null) // Hero is attacking an enemy
        {
            enemy.enemyStats.TakeDamage(enemy.enemyStats, attackerHero.heroUIManager.weaponAttack.GetValue());
            Debug.Log("Damaged the enemy");
        }
        else if (hero != null && attackerEnemy != null) // Enemy is attacking a hero
        {
            hero.heroUIManager.TakeDamage(hero.heroUIManager, attackerEnemy.enemyStats.weaponAttack.GetValue());
            Debug.Log("Damaged the hero");
        }
    }


    private IEnumerator AnimPlayer(GameObject go, Transform target) 
    {
        Vector3 startPos = go.transform.position;
        Vector3 targetPos = target.position;
        float duration = 0.5f; 
        float elapsedTime = 0f;

        BattleManager.instance.UIBattleManager.actionSprite.SetActive(true);
        SpriteRenderer sprite = BattleManager.instance.UIBattleManager.actionSprite.GetComponent<SpriteRenderer>();
        sprite.sprite = this.sprite;

        // Move to target
        while (elapsedTime < duration)
        {
            go.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        go.transform.position = targetPos; 
        

        yield return new WaitForSeconds(1.25f); 
        
        // Move back to original position
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            go.transform.position = Vector3.Lerp(targetPos, startPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        go.transform.position = startPos; 

        if(go.transform.position == startPos)
        {
            yield return new WaitForSeconds(1f); 
            BattleManager.instance.UIBattleManager.actionSprite.SetActive(false);
        }

        yield break;
    }
}



