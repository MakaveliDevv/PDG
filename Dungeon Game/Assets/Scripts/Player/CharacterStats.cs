using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStats
{
    [Header("Hero Info")]
    public int heroID;
    public string name;
    public int level = 1;
    public float EXP = 0f;

    [Header("HP & MP Stats")]
    public Stat maxHP;
    public Stat currentHP;
    public Stat maxMP;
    public Stat currentMP;

    [Header("Ability Points Stats")]
    public Stat STR;
    public Stat INT;
    public Stat DEX;
    public Stat LUK;

    [Header("Attack & Defense Stats")]
    public Stat weaponAttack;
    public Stat magicAttack;
    public Stat weaponDEF;
    public Stat magicDEF;

    [Header("Attacks")]
    public List<BaseAction> physicalAttacks = new();
    public List<BaseAction> magicAttacks = new();
    public List<BaseAction> buffs = new();

    // private HeroStats heroPanelStats;

    // public void TakeDamage(GameObject damagable, float incomingDamage) 
    // {
    //     currentHP.SetValue(currentHP.GetValue() - incomingDamage);
    //     if(damagable.CompareTag("Hero")) 
    //     {
    //         heroPanelStats.hp_text.text = currentHP.GetValue().ToString();
    //     }
    // }
}
