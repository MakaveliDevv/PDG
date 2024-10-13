using System.Collections.Generic;
using UnityEngine;

public class Stats
{    
    [Header("HP & MP")]
    public float maxHP;
    public float currentHP;
    public float maxMP;
    public float currentMP;

    [Header("Attack Stats")]
    public float baseAtt;
    public float curAtt;
    public float baseMatt;
    public float curMatt;

    [Header("Defense Stats")]
    public float basePhysicalDEF;
    public float curPhysicalDEF;
    public float baseMattDEF;
    public float curMattDEF;

    

    [Header("Attacks")]
    public List<BaseAction> physicalAttacks = new();
    public List<BaseAction> magicAttacks = new();
    public List<BaseAction> buffs = new();
}
