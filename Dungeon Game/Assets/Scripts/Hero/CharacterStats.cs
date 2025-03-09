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
    
    public virtual void CustomAwake() 
    {
        currentHP = maxHP;
        currentMP = maxMP;
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

// public void TakeDamage(GameObject damagable, float incomingDamage) 
// {
//     currentHP.SetValue(currentHP.GetValue() - incomingDamage);
//     if(damagable.CompareTag("Hero")) 
//     {
//         heroPanelStats.hp_text.text = currentHP.GetValue().ToString();
//     }
// }