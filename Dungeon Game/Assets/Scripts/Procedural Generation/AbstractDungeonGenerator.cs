using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    // Dungeon 
    [Header("Dungeon Stuff")]
    [SerializeField] protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField] protected float brushSizeX = 6f, brushSizeY = 6f;
    [SerializeField] protected List<BoundsInt> roomsList;
    [SerializeField] protected int corridorLength = 14, corridorCount = 5;
    [SerializeField] protected int roomCount = 0;
    [SerializeField] [Range(0f, 1f)] protected float changeDirectionProbability = 0.3f;



    // SpawnPoint
    [Header("SpawnPoint Stuff")]
    [SerializeField] protected int maxSpawnPoints = 10;
    [SerializeField] protected GameObject spawnPointPref;
    [SerializeField] protected List<GameObject> spawnPoints = new(); // To store the spawn points
    [SerializeField] protected float minDistanceToFirstRoom = 5f; 
    [SerializeField] protected Transform spawnPointsHolder;


    // Player
    [Header("Player Stuff")]
    [SerializeField] protected GameObject heroPref;
    [SerializeField] protected List<GameObject> heroes = new();

    // Enemy
    [Header("Enemy Stuff")]
    [SerializeField] protected List<GameObject> enemyTypes = new();
    [SerializeField] protected List<GameObject> enemies = new();
    [SerializeField] protected int enemiesAmount;
    [SerializeField] protected int enemyCounter;
    [SerializeField] protected float checkRadius = 5f; 
    [SerializeField] protected Transform enemiesHolder;


    public void GenerateDungeon() 
    {
        tilemapVisualizer.Clear();
        roomsList.Clear();
        roomCount = 0;
        enemyCounter = 0;

        DestroyAtNewGeneration(heroes);
        DestroyAtNewGeneration(spawnPoints);
        DestroyAtNewGeneration(enemies);

        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();

    private void DestroyAtNewGeneration(List<GameObject> gameObjects) 
    {
        foreach (var gameObject in gameObjects)
        {
            DestroyImmediate(gameObject);
        }
        gameObjects.Clear();
    }
}



 // private void CorridorGenerator()
    // {
    //     HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
    //     List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();

    //     Vector2Int currentPosition = startPosition;

    //     for (int i = 0; i < corridorCount; i++)
    //     {
    //         // Generate corridor in a random direction, starting from current position
    //         var direction = Direction2D.GetRandomCardinalDirection();
    //         var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, direction, corridorLength);

    //         corridors.Add(corridor);
    //         currentPosition = corridor[corridor.Count - 1]; // Update current position to end of corridor

    //         floorPositions.UnionWith(corridor);
    //     }

    //     // Generate rooms and handle dead ends as before
    //     HashSet<Vector2Int> roomPositions = CreateRooms(corridors);

    //     List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);
    //     CreateRoomsAtDeadEnd(deadEnds, roomPositions);

    //     floorPositions.UnionWith(roomPositions);

    //     // Increase corridor width (if needed)
    //     for (int i = 0; i < corridors.Count; i++)
    //     {
    //         corridors[i] = IncreaseCorridorBrush3By3(corridors[i]);
    //         floorPositions.UnionWith(corridors[i]);
    //     }

    //     // Paint tiles and create walls
    //     tilemapVisualizer.PaintFloorTiles(floorPositions);
    //     WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    // }


  // private void CorridorGenerator()
    // {
    //     HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
    //     List<List<Vector2Int>> corridors = CreateCorridors(floorPositions);

    //     HashSet<Vector2Int> roomPositions = CreateRooms(corridors);

    //     List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);
    //     CreateRoomsAtDeadEnd(deadEnds, roomPositions);

    //     floorPositions.UnionWith(roomPositions);

    //     for (int i = 0; i < corridors.Count; i++)
    //     {
    //         corridors[i] = IncreaseCorridorBrush3By3(corridors[i]);
    //         floorPositions.UnionWith(corridors[i]);
    //     }

    //     tilemapVisualizer.PaintFloorTiles(floorPositions);
    //     WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    // }


        // private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions)
    // {
    //     var currentPosition = startPosition; // Start from the specified first room position

    //     List<List<Vector2Int>> corridors = new();

    //     for (int i = 0; i < corridorCount; i++)
    //     {
    //         var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
    //         corridors.Add(corridor);

    //         currentPosition = corridor[corridor.Count - 1];
    //         floorPositions.UnionWith(corridor);
    //     }

    //     return corridors;
    // }
