using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    protected override void RunProceduralGeneration()
    {
        DungeonGenerator();
    }

    private void DungeonGenerator()
    {
        HashSet<Vector2Int> floorPositions = new();
        List<List<Vector2Int>> corridors = new();

        Vector2Int currentPosition = startPosition;
        var currentDirection = Direction2D.GetRandomCardinalDirection();

        // Generate the initial room at the starting position
        var initialRoom = StartRandomWalk(randomWalkParameters, currentPosition);
        floorPositions.UnionWith(initialRoom);
        roomsList.Add(CalculateBounds(initialRoom)); // Add the initial room bounds to roomsList
        roomCount++;

        // Generate the primary corridor path from the starting position
        currentPosition = GetRoomCenter(roomsList[0]); // Update to the center of the first room
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
        FindAllDeadEnds(floorPositions);
        // Find all dead ends and potentially create rooms at dead ends
        // List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);
        // CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        // Increase brush size for corridors
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
           HandleEnemySpawning(point);
        }

        // Spawn the player
        GeneratePlayer(GameManager.instance.mainHeroPrefab);

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

    private HashSet<Vector2Int> CreateRooms(List<List<Vector2Int>> corridors)
    {
        HashSet<Vector2Int> roomPositions = new();
        List<BoundsInt> roomBoundsList = new();

        foreach (var corridor in corridors)
        {
            Vector2Int roomPosition = corridor[corridor.Count / 2];
            var roomFloor = StartRandomWalk(randomWalkParameters, roomPosition);
            var roomBounds = CalculateBounds(roomFloor);

            Debug.Log($"New Room Bounds: {roomBounds}");

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
    
    private HashSet<Vector2Int> GenerateSpawnPoints(HashSet<Vector2Int> floor, HashSet<Vector2Int> walls, int maxSpawnPoints)
    {
        HashSet<Vector2Int> floorSpawnPoints = new();
        int maxAttemptsPerPoint = 10;

        // Precompute constants for readability
        float minDistanceToWallSquared = 5f;
        float minDistanceBetweenSpawnsSquared = 10f;

        BoundsInt firstRoomBounds = roomsList[0];
        Vector2Int firstRoomCenter = GetRoomCenter(firstRoomBounds);
        float minDistanceToFirstRoomSquared = 3f * 3f;

        for (int i = 0; i < maxSpawnPoints; i++)
        {
            Vector2Int spawnPoint = FindValidSpawnPoint(floor, walls, floorSpawnPoints, maxAttemptsPerPoint, firstRoomBounds, firstRoomCenter, minDistanceToWallSquared, minDistanceBetweenSpawnsSquared, minDistanceToFirstRoomSquared);
            
            if (spawnPoint != Vector2Int.zero) // zero means no valid spawn point was found
            {
                floorSpawnPoints.Add(spawnPoint);
            }
        }

        return floorSpawnPoints;
    }

    private Vector2Int FindValidSpawnPoint(HashSet<Vector2Int> floor, HashSet<Vector2Int> walls, HashSet<Vector2Int> floorSpawnPoints, int maxAttempts, BoundsInt firstRoomBounds, Vector2Int firstRoomCenter, float minDistanceToWallSquared, float minDistanceBetweenSpawnsSquared, float minDistanceToFirstRoomSquared)
    {
        for (int attempt = 0; attempt < maxAttempts * floor.Count; attempt++)
        {
            Vector2Int spawnPoint = floor.ElementAt(Random.Range(0, floor.Count));

            if (IsWithinBounds(spawnPoint, firstRoomBounds) || IsTooCloseToWall(spawnPoint, walls, minDistanceToWallSquared))
                continue;

            if (!IsFarEnoughFromFirstRoom(spawnPoint, firstRoomCenter, minDistanceToFirstRoomSquared))
                continue;

            if (IsTooCloseToExistingSpawns(spawnPoint, floorSpawnPoints, minDistanceBetweenSpawnsSquared))
                continue;

            if (SpawnPointAccessible(spawnPoint, walls))
                return spawnPoint;
        }

        return Vector2Int.zero; // Return zero if no valid spawn point found
    }

    private bool IsFarEnoughFromFirstRoom(Vector2Int spawnPoint, Vector2Int roomCenter, float minDistanceSquared)
    {
        return (spawnPoint - roomCenter).sqrMagnitude >= minDistanceSquared;
    }

    private bool IsTooCloseToExistingSpawns(Vector2Int spawnPoint, HashSet<Vector2Int> existingSpawns, float minDistanceSquared)
    {
        return existingSpawns.Any(existingSpawn => (existingSpawn - spawnPoint).sqrMagnitude < minDistanceSquared);
    }

    private Vector2Int GetRoomCenter(BoundsInt roomBounds)
    {
        return new Vector2Int((roomBounds.min.x + roomBounds.max.x) / 2, (roomBounds.min.y + roomBounds.max.y) / 2);
    }

    private bool IsTooCloseToWall(Vector2Int position, HashSet<Vector2Int> walls, float minDistanceSquared)
    {
        foreach (Vector2Int wall in walls)
        {
            if ((position - wall).sqrMagnitude < minDistanceSquared)
            {
                return true;
            }
        }
        return false;
    }

    private bool SpawnPointAccessible(Vector2Int spawnPoint, HashSet<Vector2Int> walls)
    {
        foreach (var direction in Direction2D.cardinalDirectionsList)
        {
            Vector2Int neighbor = spawnPoint + direction;
            if (!walls.Contains(neighbor))
            {
                return true;
            }
        }
        return false;
    }


    private bool IsWithinBounds(Vector2Int position, BoundsInt bounds)
    {
        return position.x >= bounds.xMin && position.x < bounds.xMax &&
            position.y >= bounds.yMin && position.y < bounds.yMax;
    }

    private void HandleEnemySpawning(Vector2Int spawnPosition)
    {
        // Check if the total number of spawned enemies has reached the limit
        if (GameManager.instance.enemyCounter >= GameManager.instance.amountOfEnemiesToGenerate)
        {
            Debug.Log("Enemy limit reached.");
            return;
        }

        // Ensure this spawn point is valid (no nearby enemies)
        if (!CanSpawnEnemyAt(spawnPosition))
        {
            Debug.Log("Cannot spawn enemy too close to an existing one.");
            return;
        }

        // Get a random enemy type from the available enemy types
        int enemyTypeIndex = Random.Range(0, GameManager.instance.enemyTypes.Count);
        GameObject enemyPrefab = GameManager.instance.enemyTypes[enemyTypeIndex];

        // Convert spawn position to world space
        Vector2 worldPosition = new Vector2(spawnPosition.x, spawnPosition.y);

        // Instantiate the enemy at the position
        GameObject newEnemy = Instantiate(enemyPrefab, worldPosition, Quaternion.identity);
        GameManager.instance.enemiesInGame.Add(newEnemy);

        // Set the enemy as a child of the designated enemies parent object
        newEnemy.transform.SetParent(GameManager.instance.enemiesInSceneGameObjectContainer);

        // Increment the enemy counter
        GameManager.instance.enemyCounter++;

        Debug.Log($"Spawned {enemyPrefab.name} at {worldPosition}. Total enemies: {GameManager.instance.enemyCounter}");
    }

    private bool CanSpawnEnemyAt(Vector2Int position)
    {
        foreach (var enemy in GameManager.instance.enemiesInGame)
        {
            Vector2 enemyPosition = new(enemy.transform.position.x, enemy.transform.position.y);
            float distance = Vector2.Distance(position, enemyPosition);

            // Check if the position is too close to an existing enemy
            if (distance < GameManager.instance.checkForSpawnPointRadius)
            {
                return false;
            }
        }

        return true;
    }

    private void GeneratePlayer(GameObject _player) 
    {     
        Vector3 roomCenter = roomsList[0].center;

        // Spawn the player at the center
        GameObject heroGameObject = Instantiate(_player, roomCenter, Quaternion.identity);
        heroGameObject.name = "Main Hero";

        // Fetch the HeroManager script
        HeroManager hero = heroGameObject.GetComponent<HeroManager>();
        UIManager.instance.InstantiateHeroPanelUI(hero);

        hero.heroUIManager.heroID = 1;
        int heroID = hero.heroUIManager.heroID;

        // Add it to the dictionary entry
        var entry = new DictionaryEntry<int, HeroManager> 
        {
            Key = heroID,
            Value = hero
        };

        GameManager.instance.heroes.Add(entry);
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
            else 
            if (i == 0)
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
        if (positions.Count == 0)
        {
            Debug.LogError("CalculateBounds called with an empty positions set.");
            return new BoundsInt(Vector3Int.zero, Vector3Int.zero);
        }

        // Log positions
        Debug.Log($"Positions: {string.Join(", ", positions.Select(p => p.ToString()).ToArray())}");

        Vector2Int min = positions.Aggregate((acc, pos) => new Vector2Int(Mathf.Min(acc.x, pos.x), Mathf.Min(acc.y, pos.y)));
        Vector2Int max = positions.Aggregate((acc, pos) => new Vector2Int(Mathf.Max(acc.x, pos.x), Mathf.Max(acc.y, pos.y)));
        BoundsInt bounds = new BoundsInt(new Vector3Int(min.x, min.y, 0), new Vector3Int(max.x - min.x + 1, max.y - min.y + 1, 1));
        
        Debug.Log($"Room Bounds: Min: {min}, Max: {max}, Size: {bounds.size}");
        return bounds;
}

    // private BoundsInt CalculateBounds(HashSet<Vector2Int> positions)
    // {
    //     Vector2Int min = positions.Aggregate((acc, pos) => new Vector2Int(Mathf.Min(acc.x, pos.x), Mathf.Min(acc.y, pos.y)));
    //     Vector2Int max = positions.Aggregate((acc, pos) => new Vector2Int(Mathf.Max(acc.x, pos.x), Mathf.Max(acc.y, pos.y)));
    //     return new BoundsInt(new Vector3Int(min.x, min.y, 0), new Vector3Int(max.x - min.x + 1, max.y - min.y + 1, 1));
    // }

    private bool BoundsOverlap(BoundsInt boundsA, BoundsInt boundsB)
    {
        return boundsA.min.x < boundsB.max.x &&
               boundsA.max.x > boundsB.min.x &&
               boundsA.min.y < boundsB.max.y &&
               boundsA.max.y > boundsB.min.y;
    }
}