using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    // Dungeon 
    [Header("Dungeon Stuff")]
    [SerializeField] protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField] protected List<BoundsInt> roomsList;
    [SerializeField] protected float brushSizeX = 6f, brushSizeY = 6f;
    [SerializeField] protected int corridorLength = 14, corridorCount = 5;
    [SerializeField] protected int roomCount = 0;
    [SerializeField] [Range(0f, 1f)] protected float changeDirectionProbability = 0.3f;


    // SpawnPoint
    [Header("Spawn Point Stuff")]
    [SerializeField] protected List<GameObject> spawnPoints = new(); // To store the spawn points
    [SerializeField] protected GameObject spawnPointPref;
    [SerializeField] protected Transform spawnPointsHolder;
    [SerializeField] protected int maxSpawnPoints = 20;
    [SerializeField] protected float minDistanceToFirstRoom = 5f; 

    void Start() 
    {
        GenerateDungeon();
    }

    public void GenerateDungeon() 
    {
        tilemapVisualizer.Clear();
        roomsList.Clear();
        roomCount = 0;
        GameManager.instance.enemyCounter = 0;

        DestroyAtNewGeneration(spawnPoints);
        DestroyAtNewGeneration(GameManager.instance.enemiesInGame);

        // Clear dictionary
        GameManager.instance.heroes.Clear();

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
