// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEditor;
// using UnityEngine;

// public class RoomDungeonGenerator : SimpleRandomWalkDungeonGenerator
// {
//     [SerializeField] private int minRoomWidth = 4, minRoomHeight = 4;
//     [SerializeField] private int dungeonWidth = 20, dungeonHeight = 20;
//     [SerializeField] [Range(0, 10)] private int offset = 1;
//     [SerializeField] private bool randomWalkRooms = false;

//     protected override void RunProceduralGeneration()
//     {
//         roomsList.Clear();
//         CreateRooms();
//         if (roomsList.Count > 0)
//         {
//             DrawFirstRoomBounds(roomsList.First());
//             DrawLastRoomBounds(roomsList.Last());
//         }
//     }

//     private void CreateRooms()
//     {
//         var generatedRooms = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
//             new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)),
//             minRoomWidth, minRoomHeight
//         );

//         HashSet<Vector2Int> floor;
//         if (randomWalkRooms)
//         {
//             floor = CreateRoomsRandomly(generatedRooms);
//         }
//         else
//         {
//             floor = CreateSimpleRooms(generatedRooms);
//         }

//         List<Vector2Int> roomCenters = generatedRooms.Select(room => (Vector2Int)Vector3Int.RoundToInt(room.center)).ToList();
//         HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
//         floor.UnionWith(corridors);

//         tilemapVisualizer.PaintFloorTiles(floor);
//         WallGenerator.CreateWall(floor, tilemapVisualizer);
//     }

//     private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> generatedRooms)
//     {
//         HashSet<Vector2Int> floor = new();
//         foreach (var roomBounds in generatedRooms)
//         {
//             var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
//             var roomFloor = StartRandomWalk(randomWalkParameters, roomCenter);

//             foreach (var position in roomFloor)
//             {
//                 if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) &&
//                     position.y >= (roomBounds.yMin + offset) && position.y <= (roomBounds.yMax - offset))
//                 {
//                     floor.Add(position);
//                 }
//             }

//             // Add room bounds to roomsList
//             roomsList.Add(CalculateBounds(roomFloor));
//         }

//         return floor;
//     }

//     private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> generatedRooms)
//     {
//         HashSet<Vector2Int> floor = new();
//         foreach (var room in generatedRooms)
//         {
//             for (int col = offset; col < room.size.x - offset; col++)
//             {
//                 for (int row = offset; row < room.size.y - offset; row++)
//                 {
//                     Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
//                     floor.Add(position);
//                 }
//             }

//             // Add room bounds to roomsList
//             roomsList.Add(room);
//         }

//         return floor;
//     }

//     private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
//     {
//         HashSet<Vector2Int> corridors = new();
//         var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
//         roomCenters.Remove(currentRoomCenter);

//         while (roomCenters.Count > 0)
//         {
//             Vector2Int closestPoint = FindClosestPointTo(currentRoomCenter, roomCenters);
//             roomCenters.Remove(closestPoint);

//             HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closestPoint);
//             currentRoomCenter = closestPoint;
//             corridors.UnionWith(newCorridor);
//         }

//         return corridors;
//     }

//     private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
//     {
//         HashSet<Vector2Int> corridor = new();
//         var position = currentRoomCenter;
//         corridor.Add(position);

//         while (position.y != destination.y)
//         {
//             if (destination.y > position.y)
//             {
//                 position += Vector2Int.up;
//             }
//             else if (destination.y < position.y)
//             {
//                 position += Vector2Int.down;
//             }

//             corridor.Add(position);
//         }

//         while (position.x != destination.x)
//         {
//             if (destination.x > position.x)
//             {
//                 position += Vector2Int.right;
//             }
//             else if (destination.x < position.x)
//             {
//                 position += Vector2Int.left;
//             }

//             corridor.Add(position);
//         }

//         return corridor;
//     }

//     private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
//     {
//         Vector2Int closestPoint = Vector2Int.zero;
//         float distance = float.MaxValue;

//         foreach (var position in roomCenters)
//         {
//             float currentDistance = Vector2.Distance(position, currentRoomCenter);
//             if (currentDistance < distance)
//             {
//                 distance = currentDistance;
//                 closestPoint = position;
//             }
//         }

//         return closestPoint;
//     }

//     // private void OnDrawGizmos()
//     // {   
//     //     if(roomsList.Count > 0)
//     //     {
//     //         DrawFirstRoomBounds(roomsList.First());
//     //         DrawLastRoomBounds(roomsList.Last());
//     //     }
//     // }

//     private void DrawFirstRoomBounds(BoundsInt roomBounds)
//     {
//         Vector3Int min = roomBounds.min;
//         Vector3Int max = roomBounds.max;

//         Vector3 topLeft = new(min.x, min.y, 0);
//         Vector3 topRight = new(max.x, min.y, 0);
//         Vector3 bottomRight = new(max.x, max.y, 0);
//         Vector3 bottomLeft = new(min.x, max.y, 0);

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

//         Debug.DrawLine(topLeft, topRight, Color.red);
//         Debug.DrawLine(topRight, bottomRight, Color.red);
//         Debug.DrawLine(bottomRight, bottomLeft, Color.red);
//         Debug.DrawLine(bottomLeft, topLeft, Color.red);
//     }

//     private BoundsInt CalculateBounds(HashSet<Vector2Int> positions)
//     {
//         Vector2Int min = positions.Aggregate((acc, pos) => new Vector2Int(Mathf.Min(acc.x, pos.x), Mathf.Min(acc.y, pos.y)));
//         Vector2Int max = positions.Aggregate((acc, pos) => new Vector2Int(Mathf.Max(acc.x, pos.x), Mathf.Max(acc.y, pos.y)));
//         return new BoundsInt(new Vector3Int(min.x, min.y, 0), new Vector3Int(max.x - min.x + 1, max.y - min.y + 1, 1));
//     }
// }
