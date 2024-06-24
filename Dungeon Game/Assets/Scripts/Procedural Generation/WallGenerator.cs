using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class WallGenerator
{
    // public static void CreateWalls(HashSet<Vector2Int> _floorPositions, TilemapVisualizer _tilemapVisualizer) 
    // {
    //     var basicWallPosition = FindWallsInDirections(_floorPositions, Direction2D.cardinalDirectionsList);
    //     foreach (var _position in basicWallPosition)
    //     {
    //         _tilemapVisualizer.PaintSingleBasicWall(_position);   
    //     }
    // }

    public static HashSet<Vector2Int> CreateWall(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        List<Vector2Int> directionList = Direction2D.cardinalDirectionsList; 
        HashSet<Vector2Int> wallPositions = FindWallsInDirections(floorPositions, directionList);

        foreach (var position in wallPositions)
        {
            tilemapVisualizer.PaintSingleBasicWall(position);   
        }

        return wallPositions;
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> _floorPositions, List<Vector2Int> _directionList)
    {
        HashSet<Vector2Int> wallPositions = new();
        foreach (var _position in _floorPositions)
        {
            foreach (var _direction in _directionList)
            {
                var neighbourPosition = _position + _direction;
                if(!_floorPositions.Contains(neighbourPosition)) 
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }

        return wallPositions;
    }
}
