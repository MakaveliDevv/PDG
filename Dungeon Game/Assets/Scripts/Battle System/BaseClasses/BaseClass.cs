using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass
{
    public string TheName;
    public float baseHP;
    public float curHP;

    public float baseMP;
    public float curMP;

    // Physical att
    public float baseAtt;
    public float curAtt;

    // Magical att
    public float baseMatt;
    public float curMatt;

    public float basePhysicalDEF;
    public float curPhysicalDEF;

    public float baseMattDEF;
    public float curMattDEF;

    public List<BaseAction> physicalAttacks = new();
    public List<BaseAction> magicAttacks = new();
    public List<BaseAction> defense = new();
}
