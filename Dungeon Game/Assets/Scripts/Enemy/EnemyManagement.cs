using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManagement : MonoBehaviour 
{
    public EnemyStats enemyStats;
    public bool hasTargetButtonCreated = false;
    public string currentSceneName;

    void Awake() 
    {
        enemyStats.CustomAwake(gameObject);
        StartCoroutine(SceneCheckCoroutine());
    }

    void Start() 
    {
        if(GameManager.instance.enemiesInGame.Count == 0) 
        {
            GameManager.instance.enemiesInGame.Add(gameObject);
        }
    }

    private void CheckSceneName()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        // Debug.Log($"Current Scene: {currentSceneName}");

        if (currentSceneName == "BattleScene2")
        {
            if (BattleManager.instance.turnState == TurnState.ENEMY_TURN)
            {
                PerformAction();
            }
        }
    }

    IEnumerator SceneCheckCoroutine()
    {
        while (true)
        {
            CheckSceneName();
            yield return new WaitForSeconds(2f); 
        }
    }

    private BaseAction FetchRandomAttack() 
    {
        if (BattleManager.instance.turnState == TurnState.ENEMY_TURN) 
        {
            if (enemyStats.physicalAttacks.Count == 0)
            {
                Debug.LogWarning("No physical attacks available for this enemy.");
                return null;
            }

            int randomIndex = Random.Range(0, enemyStats.physicalAttacks.Count);
            BaseAction selectedAttack = enemyStats.physicalAttacks[randomIndex];

            Debug.Log($"Enemy selected attack: {selectedAttack.Name}");

            return selectedAttack;
        }

        return null;
    }

    private GameObject ChooseRandomTarget() 
    {
        if(BattleManager.instance.turnState == TurnState.ENEMY_TURN) 
        {
            if(BattleManager.instance.heroesInBattle.Count == 0) 
            {
                Debug.LogWarning($"No heroes found in the 'heroes in battle' list.");
                return null;
            }

            int randomIndex = Random.Range(0, BattleManager.instance.heroesInBattle.Count);
            GameObject selectedHero = BattleManager.instance.heroesInBattle[randomIndex].Key;

            return selectedHero;

        }

        return null;
    }

    private void PerformAction() 
    {
        BaseAction action = FetchRandomAttack();
        GameObject player = ChooseRandomTarget(); 
        
        action.PerformAction(this, gameObject, player.transform);
        BattleManager.instance.turnState = TurnState.HERO_TURN;
    }
}

[System.Serializable]
public class EnemyStats : CharacterStats 
{
    [Header("Enemy Info")]
    public string Name;
    public int level = 1;

    // public override void CustomAwake(GameObject go) 
    // {
        
    //     maxHP.SetValue(Random.Range(maxHP.ReturnMinValue(), maxHP.ReturnMaxValue()));
    //     maxMP.SetValue(Random.Range(maxMP.ReturnMinValue(), maxMP.ReturnMaxValue()));
        
    //     STR.SetValue(Random.Range(STR.ReturnMinValue(), STR.ReturnMaxValue()));
    //     DEX.SetValue(Random.Range(DEX.ReturnMinValue(), DEX.ReturnMaxValue()));
    //     INT.SetValue(Random.Range(INT.ReturnMinValue(), INT.ReturnMaxValue()));
    //     LUK.SetValue(Random.Range(LUK.ReturnMinValue(), LUK.ReturnMaxValue()));
        
    //     ApplyStatFormulas();

    //     base.CustomAwake(go);
        
    //     // STR:
    //         // - Weapon Attack (random.range)
    //         // - Weapon DEF (random.range)
    //             // Weapon Attack: is determined by the amount of STR -> every 1 point of STR gives you a 3 weapon attack.
    //             // Weapon DEF: is also determined by the amount of STR -> every 1 point of STR gives you a 2 weapon DEF

    //     // DEX:
    //         // - Accuracy (hit or miss)
    //         // - Evading (hit or miss from enemy)       
    //             // Accuracy: is determined by the amount of DEX -> the standard percentage of a hit is at default 60%. (so there is a 60% chance that you hit the target and dont miss)
    //             // -Every 2 points on DEX increases the amount with 1%.  
    //             // Evading: is also determiend by the amount of DEX -> the standard evasion is at 35%. (so there is a 35% chance to evade the attack)
    //             // -Every 3 points on DEX increases the amount with 1%

    //     // INT:
    //         // - Magic Attack (random.range)
    //         // - Magic DEF (random.range)
    //             // Magic Attack: is determined by the amount of INT -> every 1 point of INT gives you a 2 magic attack
    //             // Magic DEF: is also determined by the amount of INT -> every 1 point of INT gives you a 1 magic DEF
        
    //     // LUK:
    //         // - Chance in obtaining better items after a fight
    //         // - Chance to obtain a better item from a chest
    //             // The default chance to obtain a rare item after a fight is at 35%
    //             // The default chance to obtain a rare item from a chest is at 20%
    //             // Every point of LUK increases the chance of obtaining a rare item after a fight with 1.25%
    //             // Every point of LUK increases the chance of obtaining a rare item from a chest with  0.75%

    // }

    // private void ApplyStatFormulas()
    // {
    //     // STR-based formulas
    //     weaponAttack.SetValue(Mathf.RoundToInt(STR.GetValue() * 3)); // 1 STR = 3 weapon attack
    //     weaponDEF.SetValue(Mathf.FloorToInt(STR.GetValue() * 2));    // 1 STR = 2 weapon DEF
        
    //     // DEX-based formulas for accuracy and evasion
    //     accuracy.SetValue(Mathf.RoundToInt(60 + (DEX.GetValue() / 2))); // Base accuracy is 60%, every 2 DEX adds 1%
    //     evasion.SetValue(Mathf.RoundToInt(35 + (DEX.GetValue() / 3))); // Base evasion is 35%, every 3 DEX adds 1%

    //     // Debug.Log($"{Name}'s Accuracy: {accuracy}%");
    //     // Debug.Log($"{Name}'s Evasion: {evasion}%");

    //     // INT-based formulas
    //     magicAttack.SetValue(Mathf.RoundToInt(INT.GetValue() * 2));   // 1 INT = 2 magic attack
    //     magicDEF.SetValue(Mathf.RoundToInt(INT.GetValue() * 1.25f));      // 1 INT = 1 magic DEF

    //     // LUK-based formulas for item drop rates
    //     rareItemDropChance.SetValue(Mathf.RoundToInt(35 + (LUK.GetValue() * 1.05f))); // Base is 35%, every 1 LUK adds 1.25%
    //     rareChestItemChance.SetValue(Mathf.RoundToInt(20 + (LUK.GetValue() * 1.025f))); // Base is 20%, every 1 LUK adds 0.75%

    //     // Debug.Log($"{Name}'s Rare Item Drop Chance: {rareItemDropChance}%");
    //     // Debug.Log($"{Name}'s Rare Chest Item Chance: {rareChestItemChance}%");
    // }
}