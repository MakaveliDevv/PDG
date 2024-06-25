using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    // [SerializeField] [Range(.1f, 1f)] private float roomPercent = .8f;

    protected override void RunProceduralGeneration()
    {
        DungeonGenerator();
    }

    private void DungeonGenerator()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();

        Vector2Int currentPosition = startPosition;
        var currentDirection = Direction2D.GetRandomCardinalDirection();

        // Generate the primary corridor path from the starting position
        var primaryCorridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, currentDirection, corridorLength);
        corridors.Add(primaryCorridor);
        floorPositions.UnionWith(primaryCorridor);

        currentPosition = primaryCorridor[primaryCorridor.Count - 1]; // Update current position to end of primary corridor

        // Generate additional corridors with occasional direction changes
        for (int i = 1; i < corridorCount; i++)
        {
            // Ensure corridors do not start from the first room
            if (currentPosition == startPosition)
            {
                currentDirection = Direction2D.GetRandomCardinalDirection(); // Change direction to avoid starting from the first room
            }
            else
            {
                // Randomly decide if we should change direction
                if (Random.value < changeDirectionProbability)
                {
                    currentDirection = Direction2D.GetRandomCardinalDirection();
                }
            }

            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, currentDirection, corridorLength);
            corridors.Add(corridor);
            currentPosition = corridor[corridor.Count - 1]; // Update current position to end of corridor

            floorPositions.UnionWith(corridor);
        }

        // Create rooms along the corridors
        HashSet<Vector2Int> roomPositions = CreateRooms(corridors);

        // Find all dead ends and potentially create rooms at dead ends
        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);
        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        // Increase brush size for corridors (if desired)
        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = IncreaseBrushSize(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
        }

        // Paint floor tiles and create walls
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        HashSet<Vector2Int> walls = WallGenerator.CreateWall(floorPositions, tilemapVisualizer);

        // Generate spawn points
        HashSet<Vector2Int> floorSpawnPoints = GenerateSpawnPoints(floorPositions, walls, maxSpawnPoints);
        foreach (Vector2Int point in floorSpawnPoints)
        {
            if (CanSpawnEnemyAt(point))
            {
                SpawnSpawnPointAt(point);
                SpawnEnemyAt(point);
            }
        }

        // Spawn the player (assuming roomsList[0] is the first room)
        SpawnPlayer(heroPref);

        // Visualize spawn points using TilemapVisualizer
        tilemapVisualizer.PaintSpawnPoints(floorSpawnPoints);
    }

    private List<Vector2Int> IncreaseBrushSize(List<Vector2Int> _corridor)
    {
        List<Vector2Int> newCorridor = new();

        for (int i = 1; i < _corridor.Count; i++)
        {
            for (int x = -1; x < brushSizeX; x++)
            {
                for (int y = -1; y < brushSizeY; y++)
                {
                    newCorridor.Add(_corridor[i - 1] + new Vector2Int(x, y));
                }
            }            
        }

        return newCorridor;
    }

    private HashSet<Vector2Int> CreateRoomAtPosition(Vector2Int position)
    {
        var roomFloor = StartRandomWalk(randomWalkParameters, position);
        var roomBounds = CalculateBounds(roomFloor);

        // Ensure there are no overlaps with existing rooms
        if (roomsList.Any(bounds => BoundsOverlap(bounds, roomBounds)))
        {
            return new HashSet<Vector2Int>(); // Return an empty set if overlapping
        }

        roomsList.Add(roomBounds); // Add room bounds to roomsList
        roomCount++;
        return roomFloor;
    }

    private HashSet<Vector2Int> CreateRooms(List<List<Vector2Int>> corridors)
    {
        HashSet<Vector2Int> roomPositions = new();
        List<BoundsInt> roomBoundsList = new();

        foreach (var corridor in corridors)
        {
            Vector2Int roomPosition = corridor[corridor.Count / 2];
            var roomFloor = StartRandomWalk(randomWalkParameters, roomPosition);
            var roomBounds = CalculateBounds(roomFloor);

            // Check for overlaps
            bool overlaps = roomBoundsList.Any(bounds => BoundsOverlap(bounds, roomBounds));
            if (!overlaps)
            {
                roomPositions.UnionWith(roomFloor);
                roomBoundsList.Add(roomBounds);
                roomsList.Add(roomBounds); // Add room bounds to roomsList
                roomCount++;
            }
        }

        return roomPositions;
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new();
        foreach (var position in floorPositions)
        {
            int neighboursCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                if (floorPositions.Contains(position + direction))
                    neighboursCount++;
            }

            if (neighboursCount == 1)
            {
                deadEnds.Add(position);
            }
        }

        return deadEnds;
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            // Skip creating a room at the start position (the first room)
            if (position == startPosition) continue;

            if (!roomFloors.Contains(position))
            {
                var newRoom = StartRandomWalk(randomWalkParameters, position);
                var newRoomBounds = CalculateBounds(newRoom);

                // Check for overlaps and merge
                bool merged = false;
                for (int i = 0; i < roomsList.Count; i++)
                {
                    if (BoundsOverlap(roomsList[i], newRoomBounds))
                    {
                        roomFloors.UnionWith(newRoom); // Merge new room with existing room
                        roomsList[i] = CalculateBounds(roomFloors); // Update bounds
                        merged = true;
                        break;
                    }
                }

                if (!merged)
                {
                    roomFloors.UnionWith(newRoom);
                    roomsList.Add(newRoomBounds); // Add new room bounds to roomsList
                    roomCount++;
                }
            }
        }
    }

    // Generate spawn points

    private void SpawnSpawnPointAt(Vector2Int position) 
    {
        Vector2 spawnPosition = new(position.x, position.y);

        GameObject spawnPoint = Instantiate(spawnPointPref, spawnPosition, Quaternion.identity);
        spawnPoints.Add(spawnPoint);
        spawnPoint.transform.SetParent(spawnPointsHolder);
    }

    private HashSet<Vector2Int> GenerateSpawnPoints(HashSet<Vector2Int> floor, HashSet<Vector2Int> walls, int maxSpawnPoints)
    {
        HashSet<Vector2Int> floorSpawnPoints = new(); 

        float minDistanceToWallSquared = 5f;
        float minDistanceBetweenSpawnsSquared = 10f; // Minimum squared distance between spawn points
        int maxAttemptsPerPoint = 10;

        BoundsInt firstRoomBounds = roomsList[0];
        Vector2Int firstRoomCenter = new((firstRoomBounds.min.x + firstRoomBounds.max.x) / 2, (firstRoomBounds.min.y + firstRoomBounds.max.y) / 2);

        for (int i = 0; i < maxSpawnPoints; i++)
        {
            int attempts = 0;
            Vector2Int spawnPoint;
            bool validSpawnPoint = false;

            while (!validSpawnPoint && attempts < maxAttemptsPerPoint * floor.Count)
            {
                spawnPoint = floor.ElementAt(Random.Range(0, floor.Count));

                // Check if the spawn point is within the first room
                if (IsWithinBounds(spawnPoint, firstRoomBounds))
                {
                    attempts++;
                    continue;
                }

                // Check if the spawn point is too close to any wall
                if (IsTooCloseToWall(spawnPoint, walls, minDistanceToWallSquared))
                {
                    spawnPoint = FindSpawnPointAwayFromWall(floor, walls, minDistanceToWallSquared);
                }

                // Check if the spawn point is far enough from the center of the first room
                float distanceToFirstRoomCenterSquared = (spawnPoint - firstRoomCenter).sqrMagnitude;
                if (distanceToFirstRoomCenterSquared < minDistanceToFirstRoom * minDistanceToFirstRoom)
                {
                    attempts++;
                    continue;
                }

                // Check if the spawn point is far enough from existing spawn points
                if (floorSpawnPoints.Any(existingSpawn => (existingSpawn - spawnPoint).sqrMagnitude < minDistanceBetweenSpawnsSquared))
                {
                    attempts++;
                    continue;
                }

                // Check if the spawn point is accessible
                if (SpawnPointAccesible(spawnPoint, walls))
                {
                    floorSpawnPoints.Add(spawnPoint);
                    validSpawnPoint = true;
                }

                attempts++;
            }
        }

        return floorSpawnPoints;
    }

    private bool IsWithinBounds(Vector2Int position, BoundsInt bounds)
    {
        return position.x >= bounds.xMin && position.x < bounds.xMax &&
            position.y >= bounds.yMin && position.y < bounds.yMax;
    }

    private bool SpawnPointAccesible(Vector2Int position, HashSet<Vector2Int> walls)
    {
        foreach (var direction in Direction2D.cardinalDirectionsList)
        {
            Vector2Int neighborPosition = position + direction;
            if (!walls.Contains(neighborPosition))
            {
                return true;
            }
        }
        return false;
    }

    private Vector2Int FindSpawnPointAwayFromWall(HashSet<Vector2Int> floor, HashSet<Vector2Int> walls, float minDistanceToWallSquared)
    {
        foreach (Vector2Int position in floor)
        {
            if (!IsTooCloseToWall(position, walls, minDistanceToWallSquared))
            {
                return position;
            }
        }
        return floor.ElementAt(Random.Range(0, floor.Count));
    }

    private bool IsTooCloseToWall(Vector2Int position, HashSet<Vector2Int> walls, float minDistanceSquared)
    {
        foreach (Vector2Int wallPosition in walls)
        {
            float distanceSquared = (position - wallPosition).sqrMagnitude;
            if (distanceSquared < minDistanceSquared)
            {
                return true;
            }
        }
        return false;
    }


    // End generate spawn points

    // Spawn enemy
    private bool CanSpawnEnemyAt(Vector2Int position)
    {
        foreach (var enemy in enemies)
        {
            Vector2 enemyPosition = new(enemy.transform.position.x, enemy.transform.position.y);
            float distance = Vector2.Distance(position, enemyPosition);

            if (distance < checkRadius)
            {
                return false;
            }            
        }

        return true;
    }

    private void SpawnEnemyAt(Vector2Int position)
    {
        // Get amount of enemies
        float amount = Random.Range(0, enemiesAmount + 1);

        // Get type of enemies
        int typeIndex = Random.Range(0, enemyTypes.Count); 

        GameObject enemyPrefab = enemyTypes[typeIndex];

        Vector2 spawnPosition = new(position.x, position.y);


        // Spawn the enemies at the position
        for (int i = 0; i < amount; i++)
        {
            // Instantiate the enemy prefab at the position
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity) as GameObject;
            enemies.Add(newEnemy);
            newEnemy.transform.SetParent(enemiesHolder);

            // Set a counter
            enemyCounter++;
        }

        // Debug.Log($"Spawned {amount} of {enemyPrefab.name} at position {position}");
    }
    // End spawn enemy

    private void SpawnPlayer(GameObject _player) 
    {
        // Get the last room index
        // int lastRoomIndex = roomsList.Count > 0 ? roomsList.Count - 1 : -1;
        
        // Get the center from that room
        Vector3 roomCenter = roomsList[0].center;

        // Spawn the player at the center
        GameObject newPlayer = Instantiate(_player, roomCenter, Quaternion.identity) as GameObject;
        newPlayer.name = "HeroCharacter";
        heroes.Add(newPlayer);
    }

    private void OnDrawGizmos()
    {
        HashSet<BoundsInt> mergedRooms = new();

        for (int i = 0; i < roomsList.Count; i++)
        {
            BoundsInt currentRoom = roomsList[i];
            bool isMerged = false;

            // Check if this room overlaps with any of the merged rooms
            foreach (var mergedRoom in mergedRooms)
            {
                if (BoundsOverlap(currentRoom, mergedRoom))
                {
                    isMerged = true;
                    break;
                }
            }

            if (isMerged)
            {
                DrawRoomBounds(currentRoom, Color.yellow); // Merged room in yellow
            }
            else if (i == 0)
            {
                DrawRoomBounds(currentRoom, Color.green); // First room in red
            }
            else if(i == roomsList.Count - 1)
            {
                DrawRoomBounds(roomsList[i], Color.red); // Other rooms in blue
            }
            else 
            {
                DrawRoomBounds(currentRoom, Color.blue); // Other standalone rooms in blue

            }

            // Add current room to mergedRooms set
            mergedRooms.Add(currentRoom);
        }
    }

    private void DrawRoomBounds(BoundsInt roomBounds, Color color)
    {
        Vector3Int min = roomBounds.min;
        Vector3Int max = roomBounds.max;

        Vector3 topLeft = new(min.x, min.y, 0);
        Vector3 topRight = new(max.x, min.y, 0);
        Vector3 bottomRight = new(max.x, max.y, 0);
        Vector3 bottomLeft = new(min.x, max.y, 0);

        Gizmos.color = color; // Use the specified color
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    private BoundsInt CalculateBounds(HashSet<Vector2Int> positions)
    {
        Vector2Int min = positions.Aggregate((acc, pos) => new Vector2Int(Mathf.Min(acc.x, pos.x), Mathf.Min(acc.y, pos.y)));
        Vector2Int max = positions.Aggregate((acc, pos) => new Vector2Int(Mathf.Max(acc.x, pos.x), Mathf.Max(acc.y, pos.y)));
        return new BoundsInt(new Vector3Int(min.x, min.y, 0), new Vector3Int(max.x - min.x + 1, max.y - min.y + 1, 1));
    }

    private bool BoundsOverlap(BoundsInt boundsA, BoundsInt boundsB)
    {
        return boundsA.min.x < boundsB.max.x &&
               boundsA.max.x > boundsB.min.x &&
               boundsA.min.y < boundsB.max.y &&
               boundsA.max.y > boundsB.min.y;
    }
}