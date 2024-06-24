using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RandomWalkGen : MonoBehaviour
{
    [SerializeField] protected Vector2Int startPos = Vector2Int.zero;

    [SerializeField] private int iterations = 10;
    [SerializeField] private int walkLength = 10;
    public bool startRandom;

    public void RunProcGen() 
    {
        HashSet<Vector2Int> floorPos =  RunRandWalk();
        foreach (var pos in floorPos)
        {
            Debug.Log(pos);
        }
    }

    protected HashSet<Vector2Int> RunRandWalk() 
    {
        var currentPos = startPos;
        HashSet<Vector2Int> floorPos = new();
        for (int i = 0; i < iterations; i++)
        {
            var path = ProcGenAlgor.RandomWalk(currentPos, walkLength);
            floorPos.UnionWith(path);
            if(startRandom)
                currentPos = floorPos.ElementAt(Random.Range(0, floorPos.Count));
        }

        return floorPos;
    }
}
