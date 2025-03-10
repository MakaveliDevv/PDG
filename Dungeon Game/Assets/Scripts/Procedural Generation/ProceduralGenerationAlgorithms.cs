using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    // METHOD TO GENERATE A RANDOM PATH
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int _startPosition, int _walkLength) 
    {
        HashSet<Vector2Int> path = new()
        {
            _startPosition
        };
        
        var previousPosition = _startPosition;

        for (int i = 0; i < _walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);

            previousPosition = newPosition;
        }

        return path;
    }

    public static HashSet<Vector2Int> SimpleLinearPath(Vector2Int _startPosition, Vector2Int _direction, int _pathLength)
    {
        HashSet<Vector2Int> path = new()
        {
            _startPosition
        };

        var previousPosition = _startPosition;

        for (int i = 0; i < _pathLength; i++)
        {
            var newPosition = previousPosition + _direction;
            path.Add(newPosition);

            previousPosition = newPosition;
        }

        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int _startPosition, Vector2Int _direction, int _corridorLength) 
    {
        List<Vector2Int> corridor = new();
        
        var currentPosition = _startPosition;
        corridor.Add(currentPosition);
        
        for (int i = 0; i < _corridorLength; i++)
        {
            currentPosition += _direction;
            corridor.Add(currentPosition);
        }

        return corridor;
    }
}

public static class Direction2D
{
    // METHOD FOR THE DIRECTIONS
    public static List<Vector2Int> cardinalDirectionsList = new()
    {
        new Vector2Int(0, 1), // UP 
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(1, 0), // RIGHT
        new Vector2Int(-1, 0), // LEFT
    };

    public static Vector2Int GetRandomCardinalDirection() 
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}