using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{
    public string performerName;
    public string type;
    public GameObject performer;
    public GameObject performersTarget;

    public BaseAction chosenAction;
}
