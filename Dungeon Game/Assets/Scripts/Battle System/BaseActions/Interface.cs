using UnityEngine;

public interface IWeaponAttack
{
    void PerformAction(MonoBehaviour mono, GameObject go, Transform transform);
}

public interface IMagicAttack 
{
    void Attack();
}

public interface IShieldDefense 
{
    public void Defense();
}

public interface IBuff 
{
    public void Buff();
}

