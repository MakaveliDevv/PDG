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
        HashSet<Vector2Int> path = new HashSet<Vector2Int>
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
        List<Vector2Int> corridor = new List<Vector2Int>();
        
        var currentPosition = _startPosition;
        corridor.Add(currentPosition);
        
        for (int i = 0; i < _corridorLength; i++)
        {
            currentPosition += _direction;
            corridor.Add(currentPosition);
        }

        return corridor;
    }

    // ALGORITHM TO GENERATE SPLITTED ROOMS IN AN AREA
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt _spaceToSplit, int _minWidth, int _minHeight) 
    {
        Queue<BoundsInt> roomsQueue = new();
        List<BoundsInt> roomsList = new();

        roomsQueue.Enqueue(_spaceToSplit);

        while(roomsQueue.Count > 0) 
        {
            var room = roomsQueue.Dequeue();
            if(room.size.x >= _minWidth && room.size.y >= _minHeight) 
            {
                if(Random.value < .5f) 
                {
                    if(room.size.y >= _minHeight * 2) 
                        SplitHorizontally(_minHeight, roomsQueue, room);

                    else if(room.size.x >= _minWidth * 2) 
                        SplitVertically(_minWidth, roomsQueue, room);

                    else
                    roomsList.Add(room);                    
                    
                } else 
                {
                    if(room.size.x >= _minWidth * 2) 
                        SplitVertically(_minWidth, roomsQueue, room);
 
                    else if(room.size.y >= _minHeight * 2) 
                        SplitHorizontally(_minHeight, roomsQueue, room);
                    
                    else
                    roomsList.Add(room);    
                }
            }
        }

        return roomsList;
    }

    private static void SplitVertically(int _minWidth, Queue<BoundsInt> _roomsQueue, BoundsInt _room)
    {
        var xSplit = Random.Range(1, _room.size.x);
        BoundsInt room1 = new(_room.min, new Vector3Int(xSplit, _room.size.y, _room.size.z));
        BoundsInt room2 = new(new Vector3Int(_room.min.x + xSplit, _room.min.y, _room.min.z), new Vector3Int(_room.size.x - xSplit, _room.size.y, _room.size.z));

        _roomsQueue.Enqueue(room1);
        _roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int _minHeight, Queue<BoundsInt> _roomsQueue, BoundsInt _room)
    {
        var ySplit = Random.Range(1, _room.size.y); // minHeight, room.size.y - minHeight
        BoundsInt room1 = new(_room.min, new Vector3Int(_room.size.x, ySplit, _room.size.z));
        BoundsInt room2 = new(new Vector3Int(_room.min.x, _room.min.y + ySplit, _room.min.z), new Vector3Int(_room.size.x, _room.size.y - ySplit, _room.size.z));

        _roomsQueue.Enqueue(room1);
        _roomsQueue.Enqueue(room2);
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






  // METHOD TO GENERATE A RANDOM CORRIDOR
    // public static List<Vector2Int> RandomWalkCorridor(Vector2Int _startPosition, int _corridorLength) 
    // {
    //     List<Vector2Int> corridor = new();

    //     var direction = Direction2D.GetRandomCardinalDirection();
    //     var currentPosition = _startPosition;
    //     corridor.Add(currentPosition);
        
    //     for (int i = 0; i < _corridorLength; i++)
    //     {
    //         currentPosition += direction;
    //         corridor.Add(currentPosition);
    //     }

    //     return corridor;
    // }


    

// OLD ONE

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEditor;
// using UnityEngine;

// public class CorridorDungeonGenerator : SimpleRandomWalkDungeonGenerator
// {
//     [SerializeField] private int corridorLength = 14, corridorCount = 5;
//     [SerializeField] [Range(.1f, 1f)] private float roomPercent = .8f;

//     protected override void RunProceduralGeneration()
//     {
//         CorridorGenerator();
//     }

//     private void CorridorGenerator()
//     {
//         HashSet<Vector2Int> floorPositions = new();
//         HashSet<Vector2Int> potentialRoomPositions = new();

//         List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);

//         HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);
//         List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

//         CreateRoomsAtDeadEnd(deadEnds, roomPositions);
//         floorPositions.UnionWith(roomPositions);

//         for (int i = 0; i < corridors.Count; i++)
//         {
//             // corridors[i] = IncreaseCorridorSizeByOne(corridors[i]);
//             corridors[i] = IncreaseCorridorBrush3By3(corridors[i]);
//             floorPositions.UnionWith(corridors[i]);    
//         }

//         tilemapVisualizer.PaintFloorTiles(floorPositions);
//         WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
//     }

//     private List<Vector2Int> IncreaseCorridorBrush3By3(List<Vector2Int> _corridor)
//     {
//         List<Vector2Int> newCorridor = new();

//         for (int i = 1; i < _corridor.Count; i++)
//         {
//             for (int x = -1; x < 2; x++)
//             {
//                 for (int y = -1; y < 2; y++)
//                 {
//                     newCorridor.Add(_corridor[i - 1] + new Vector2Int(x, y));
//                 }
//             }            
//         }

//         return newCorridor;
//     }

//     private void CreateRoomsAtDeadEnd(List<Vector2Int> _deadEnds, HashSet<Vector2Int> _roomFloors)
//     {
//         foreach (var _position in _deadEnds)
//         {
//             if(_roomFloors.Contains(_position) == false) 
//             {
//                 var room = StartRandomWalk(randomWalkParameters, _position);
//                 _roomFloors.UnionWith(room);
//             }
//         }
//     }

//     private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> _floorPositions)
//     {
//         List<Vector2Int> deadEnds = new();
//         foreach (var _position in _floorPositions)
//         {
//             int neighboursCount = 0;
//             foreach (var _direction in Direction2D.cardinalDirectionsList)
//             {
//                 if(_floorPositions.Contains(_position + _direction)) 
//                     neighboursCount ++;
//             }

//             if(neighboursCount == 1) 
//             {
//                 deadEnds.Add(_position);
//             }
//         }

//         return deadEnds;
//     }

//     private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> _potentialRoomPositions)
//     {
//         HashSet<Vector2Int> roomPositions = new();
//         int roomToCreateCount = Mathf.RoundToInt(_potentialRoomPositions.Count * roomPercent);

//         List<Vector2Int> roomToCreate = _potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

//         foreach (var _roomPosition in roomToCreate)
//         {
//             var roomFloor = StartRandomWalk(randomWalkParameters, _roomPosition);
//             roomPositions.UnionWith(roomFloor);
//         }

//         return roomPositions;
//     }

//     private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> _floorPositions, HashSet<Vector2Int> _potentialRoomPositions)
//     {
//         var currentPosition = startPosition;
//         _potentialRoomPositions.Add(currentPosition);

//         List<List<Vector2Int>> corridors = new();

//         for (int i = 0; i < corridorCount; i++)
//         {
//             var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
//             corridors.Add(corridor);
            
//             currentPosition = corridor[corridor.Count - 1];
//             _potentialRoomPositions.Add(currentPosition);
//             _floorPositions.UnionWith(corridor);
//         }

//         return corridors;
//     }

//     private void OnDrawGizmos() 
//     {
//         DrawFirstRoomBounds(roomsList.First());
//         DrawLastRoomBounds(roomsList.Last());
//         // for (int i = 0; i < ; i++)
//         // {
            
//         // }
//     } 

//     private void DrawFirstRoomBounds(BoundsInt roomBounds) 
//     {
//         Vector3Int min = roomBounds.min;
//         Vector3Int max = roomBounds.max;

//         Vector3 topLeft = new(min.x, min.y, 0);
//         Vector3 topRight = new(max.x, min.y, 0);
//         Vector3 bottomRight = new(max.x, max.y, 0);
//         Vector3 bottomLeft = new(min.x, max.y, 0);

//         // Draw lines around the room bounds
//         Debug.DrawLine(topLeft, topRight, Color.green);
//         Debug.DrawLine(topRight, bottomRight, Color.green);
//         Debug.DrawLine(bottomRight, bottomLeft, Color.green);
//         Debug.DrawLine(bottomLeft, topLeft, Color.green);
//     }

//     private void DrawLastRoomBounds(BoundsInt roomBounds) 
//     {
//         Vector3Int min = roomBounds.min;
//         Vector3Int max = roomBounds.max;

//         Vector3 topLeft = new(min.x, min.y, 0);
//         Vector3 topRight = new(max.x, min.y, 0);
//         Vector3 bottomRight = new(max.x, max.y, 0);
//         Vector3 bottomLeft = new(min.x, max.y, 0);

//         // Draw lines around the room bounds
//         Debug.DrawLine(topLeft, topRight, Color.red);
//         Debug.DrawLine(topRight, bottomRight, Color.red);
//         Debug.DrawLine(bottomRight, bottomLeft, Color.red);
//         Debug.DrawLine(bottomLeft, topLeft, Color.red);
//     }

    // public List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> _corridor)
    // {
    //     List<Vector2Int> newCorridor = new();
    //     Vector2Int previousDirection = Vector2Int.zero;

    //     for (int i = 1; i < _corridor.Count; i++)
    //     {
    //         Vector2Int directionFromCell = _corridor[i] - _corridor[i - 1];
    //         if(previousDirection != Vector2Int.zero && directionFromCell != previousDirection) 
    //         {
    //             // HANDLE CORNER
    //             for (int x = -1; x < 2; x++)
    //             {
    //                 for (int y = -1; y < 2; y++)
    //                 {
    //                     newCorridor.Add(_corridor[i - 1] + new Vector2Int(x, y));
    //                 }
    //             }

    //             previousDirection = directionFromCell;

    //         } else 
    //         {
    //             // Add a single cell in the direction + 90 degrees
    //             Vector2Int newCorridorTileOffset = GetDirection90From(directionFromCell);
    //             newCorridor.Add(_corridor[i - 1]);
    //             newCorridor.Add(_corridor[i - 1] + newCorridorTileOffset);
    //         }
    //     }

    //     return newCorridor;
    // }

    // private Vector2Int GetDirection90From(Vector2Int _directionFromCell)
    // {
    //     if(_directionFromCell == Vector2Int.up) return Vector2Int.right;
    //     if(_directionFromCell == Vector2Int.right) return Vector2Int.down;
    //     if(_directionFromCell == Vector2Int.down) return Vector2Int.left;
    //     if(_directionFromCell == Vector2Int.left) return Vector2Int.up;

    //     return Vector2Int.zero;
    // }
// }