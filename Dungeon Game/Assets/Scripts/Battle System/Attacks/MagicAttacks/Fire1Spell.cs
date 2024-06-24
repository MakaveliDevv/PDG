using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire1Spell : BaseAttack
{
    public Fire1Spell () 
    {
        AttackName = "Fire1";
        AttackDescription = "Basic fire spell which burns nothing.";
        attackDamage = 20f;
        attackCost = 10f;
    }
}
