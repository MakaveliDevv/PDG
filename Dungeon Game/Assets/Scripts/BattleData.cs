using System.Collections.Generic;
using UnityEngine;

public class BattleData : MonoBehaviour
{
    public static BattleData instance;
    public List<DictionaryEntry<GameObject, HeroManager>> heroesToBattle = new();
    public List<DictionaryEntry<GameObject, EnemyManagement>> enemiesToBattle;


    void Awake() 
    {
        if(instance != null && instance != null) 
        {
            Destroy(this);
        }
        else 
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void Start() 
    {
        
    }

    public void ClearList() 
    {
        heroesToBattle.Clear();
        enemiesToBattle.Clear();
    }
    
}
