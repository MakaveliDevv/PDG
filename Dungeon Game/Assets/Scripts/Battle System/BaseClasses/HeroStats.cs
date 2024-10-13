using UnityEngine;

[System.Serializable]
public class HeroStats : Stats
{
    [Header("Hero Info")]
    public int heroID;
    public string name;
    public int level = 1;
    public float EXP = 0f;

    [Header("Ability Points")]
    public int STR = 0;
    public int INT = 0;
    public int DEX = 0;
    public int LUK = 0;
}
