using System.Collections.Generic;
using UnityEngine;

public class CharacterStats
{
    [Header("HP & MP Stats")]
    public Stat maxHP;
    public Stat currentHP;
    public Stat maxMP;
    public Stat currentMP;

    [Header("Ability Points Stats")]
    public Stat STR;
    public Stat DEX;
    public Stat INT;
    public Stat LUK;

    [Header("Attack & Defense Stats")]
    public Stat weaponAttack;
    public Stat magicAttack;
    public Stat weaponDEF;
    public Stat magicDEF;

    [Header("Other Stats")]
    public Stat accuracy;
    public Stat evasion;
    public Stat rareItemDropChance;
    public Stat rareChestItemChance;

    [Header("Attacks")]
    public List<BaseAction> physicalAttacks = new();
    public List<BaseAction> magicAttacks = new();

    [Header("Defense")]
    public List<BaseAction> shieldAbilities = new();
    
    [Header("Buffs")]
    public List<BaseAction> buffs = new();

    public GameObject characterObject;
    public HPBar HPbar;
    
    public virtual void CustomAwake(GameObject character) 
    {
        currentHP = maxHP;
        currentMP = maxMP;

        characterObject = character;
        HPbar = character.transform.GetChild(character.transform.childCount - 1).GetChild(0).gameObject.GetComponent<HPBar>();
        Debug.Log($"{character.name} has {HPbar}");
        Debug.Log($"{character.name} maxhp -> {maxHP.GetValue()} currenthp -> {currentHP.GetValue()}");

        HPbar.InitializeHPBar(currentHP.GetValue(), maxHP.GetValue());
    }

    public void TakeDamage(CharacterStats target, float incomingDamage) 
    {
        currentHP.SetValue((int)(currentHP.GetValue() - incomingDamage));
    	
        if (target.characterObject.CompareTag("Hero")) 
        {
            HPbar.TakeDamage(currentHP.GetValue(), maxHP.GetValue(), target.weaponAttack.GetValue());
            
            // Find and remove the hero from the battle list when their HP is zero or below
            for (int i = 0; i < BattleManager.instance.heroesInBattle.Count; i++)
            {
                var entry = BattleManager.instance.heroesInBattle[i];

                if (entry.Key == target.characterObject && entry.Value.heroUIManager.currentHP.GetValue() <= 0) 
                {
                    BattleManager.instance.heroesInBattle.RemoveAt(i);
                    Debug.Log($"{target.characterObject.name} is dead and is removed from heroes in battle.");
                    break; 
                }
            }
        }
        else if (target.characterObject.CompareTag("Enemy")) 
        {
            HPbar.TakeDamage(currentHP.GetValue(), maxHP.GetValue(), target.weaponAttack.GetValue());

            // Find and remove the enemy from the battle list when their HP is zero or below
            for (int i = 0; i < BattleManager.instance.enemiesInBattle.Count; i++)
            {
                var entry = BattleManager.instance.enemiesInBattle[i];

                if (entry.Key == target.characterObject && entry.Value.enemyStats.currentHP.GetValue() <= 0) 
                {
                    BattleManager.instance.enemiesInBattle.RemoveAt(i);
                    Debug.Log($"{target.characterObject.name} is dead and is removed from enemies in battle.");
                    break; 
                }
            }
        }
    }
}

public class HeroStats : CharacterStats 
{
    [Header("Hero Info")]
    public int heroID;
    public string Name;
    public int level = 1;
    public float EXP = 0f;

}
