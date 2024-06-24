using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProcGenAlgor
{
    public static HashSet<Vector2Int> RandomWalk(Vector2Int _startPosition, int _walkLength) 
    {
        HashSet<Vector2Int> path = new()
        {
            _startPosition
        };
        
        var previousPosition = _startPosition;

        for (int i = 0; i < _walkLength; i++)
        {
            var newPosition = previousPosition + Direction.RandomCardinalDirection();
            path.Add(newPosition);

            previousPosition = newPosition;
        }

        return path;
    }


public static class Direction
{
    // METHOD FOR THE DIRECTIONS
    public static List<Vector2Int> cardinalDirectionsList = new()
    {
        new Vector2Int(0, 1), // UP 
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(1, 0), // RIGHT
        new Vector2Int(-1, 0), // LEFT
    };

    public static Vector2Int RandomCardinalDirection() 
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}

}
