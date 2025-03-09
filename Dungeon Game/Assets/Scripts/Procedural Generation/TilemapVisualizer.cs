using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap, wallTimeMap, enemySpawnLocations, itemSpawnLocations;
    [SerializeField] private TileBase floorTile, wallTile, enemySpawnPointTile, itemSpawnPointTile;

    public void PaintFloorTiles(IEnumerable<Vector2Int> _floorPositions)
    {
        PaintTiles(_floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> _positions, Tilemap _tilemap, TileBase _tile)
    {
        foreach (var _position in _positions)
        {
            PaintSingleTile(_tilemap, _tile, _position);
        }
    }

    internal void PaintSingleBasicWall(Vector2Int _position)
    {
        PaintSingleTile(wallTimeMap, wallTile, _position);
    }

    private void PaintSingleTile(Tilemap _tilemap, TileBase _tile, Vector2Int _position)
    {
        var tilePosition = _tilemap.WorldToCell((Vector3Int)_position);
        _tilemap.SetTile(tilePosition, _tile);
    }

    public void PaintEnemySpawnPoints(IEnumerable<Vector2Int> _spawnPointPositions)
    {
        PaintTiles(_spawnPointPositions, enemySpawnLocations, enemySpawnPointTile);
    }

    public void PaintItemSpawnPoints(IEnumerable<Vector2Int> _spawnPointPositions) 
    {
        PaintTiles(_spawnPointPositions, itemSpawnLocations, itemSpawnPointTile);
    }

    public void Clear() 
    {
        floorTilemap.ClearAllTiles();
        wallTimeMap.ClearAllTiles();
        enemySpawnLocations.ClearAllTiles();
    }
}
