using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIManager
{
    // Dictionary for each hero and the stats assigned to the hero
    [SerializeField] public List<Dictionary<string, HeroPanelStats>> heroPanelStats;
}
