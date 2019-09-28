using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LakeGenerator : MonoBehaviour {
    
    public Tilemap lakeTilemap;
    public Tilemap backgroundLakeObjects;
    public Tilemap lakeObjectsTilemap;

    [SerializeField]
    public TileBase _waterTile;

    [SerializeField]
    private TileBase _leftConnector;
    [SerializeField]
    private TileBase _rightConnector;
    [SerializeField]
    private TileBase _topConnector;

    [SerializeField]
    private TileBase _waterLeft;
    [SerializeField]
    private TileBase _waterRight;
    [SerializeField]
    private TileBase _waterBottom;
    [SerializeField]
    private TileBase _waterConnectorLeft;
    [SerializeField]
    private TileBase _waterConnectorRight;

    [SerializeField]
    private TileBase _reeds;
    [SerializeField]
    private TileBase[] _waterRocks;
    [SerializeField]
    private TileBase[] _waterPlants;
    
    private const int MIN_AMOUNT_OF_LAKES = 30;

    private List<Vector3> lakeSpawnPositions = new List<Vector3>();

    private int _tileCounter = 0;

    public static LakeGenerator Instance;

    private void Awake() {
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy(gameObject);
		}
		GenerateLakes();
	}

    private void GenerateLakes() {
        int size = WorldGenerator.TILES_PER_SIDE - 15;
        if (size < 0) {
            Debug.LogError("WHAT YOU DOING?");
            return;
        }
        while(lakeSpawnPositions.Count < MIN_AMOUNT_OF_LAKES) {
            Vector3Int spawnPos = new Vector3Int(Random.Range((-size) / 2, (size) / 2), Random.Range((-size ) / 2, (size) / 2), 0);
            if (NearbyLakeExists(spawnPos) || Vector3.Distance(spawnPos, new Vector3(50, 50, 0)) < 50 || Vector3.Distance(spawnPos, new Vector3(0, 0, 0)) < 50 
                || lakeTilemap.GetTile(spawnPos) != null)
                continue;
          
            Vector3Int startPos = new Vector3Int(spawnPos.x - 4, spawnPos.y + 2, spawnPos.z);

            lakeTilemap.SetTile(startPos, _leftConnector);
            
            int currentTopX = startPos.x;
            int width = 0, height = 0;

            // TOP TILES
            while(!SwapDirection(30)) {
                if (lakeTilemap.GetTile(new Vector3Int(currentTopX + 7, startPos.y, startPos.z)) != null && _tileCounter > 2) {
                    break;
                }

                lakeTilemap.SetTile(new Vector3Int(currentTopX, startPos.y, startPos.z), _topConnector);
                _tileCounter++;
                currentTopX++;
            }
            width = _tileCounter;
            _tileCounter = 0;

            // CONNECTION FROM TOP TO RIGHT
            lakeTilemap.SetTile(new Vector3Int(currentTopX, startPos.y, startPos.z), _rightConnector);

            // RIGHT TILES
            int currentTopY = startPos.y - 1;
            while(!SwapDirection(20)) {
                if (lakeTilemap.GetTile(new Vector3Int(currentTopX, currentTopY - 7, startPos.z)) != null && _tileCounter > 2) {
                    
                    break;
                }
                lakeTilemap.SetTile(new Vector3Int(currentTopX, currentTopY, startPos.z), _waterRight);
                _tileCounter++;
                currentTopY--;
            }
            height = _tileCounter;
            _tileCounter = 0;

            // CONNECTION FROM RIGHT TO BOTTOM
            lakeTilemap.SetTile(new Vector3Int(currentTopX, currentTopY, startPos.z), _waterConnectorRight);

            // BOTTOM TILES
            currentTopX--;
            for(int i = 0; i < width; i++) {
                if (lakeTilemap.GetTile(new Vector3Int(currentTopX - 5, currentTopY, startPos.z)) != null) {
                    break;
                }
                lakeTilemap.SetTile(new Vector3Int(currentTopX, currentTopY, startPos.z), _waterBottom);
                currentTopX--;
            }

            // CONNECTION FROM BOTTOM TO LEFT
            lakeTilemap.SetTile(new Vector3Int(currentTopX, currentTopY, startPos.z), _waterConnectorLeft);

            // LEFT TILES
            currentTopY++;
            for(int i = 0; i < height; i++) {
                lakeTilemap.SetTile(new Vector3Int(currentTopX, currentTopY, startPos.z), _waterLeft);
                currentTopY++;
            }

            // CONNECTION FROM LEFT TO ORIGIN
            lakeTilemap.SetTile(new Vector3Int(currentTopX, currentTopY, startPos.z), _leftConnector);
            
            // Get actual center position of lake and add to list
            Vector3Int centerPos = new Vector3Int(startPos.x + (width / 2), startPos.y - (height / 2), 0);
            lakeSpawnPositions.Add(centerPos);
        
            // Fill lake with water
            lakeTilemap.FloodFill(new Vector3Int(startPos.x, startPos.y - 1, 0), _waterTile);
            
            // Generate reeds for the lake
            GenerateReeds(new Vector3Int(startPos.x, startPos.y - 1, 0), width, height);

            // Generate the rest of the objects for the lake
            GenerateLakeObjects(centerPos, width, height);
        }
        WorldGenerator.Instance.GenerateWorld();
    }

    private void GenerateLakeObjects(Vector3Int center, int width, int height) {
        Vector3Int start = new Vector3Int(center.x - (width / 2), center.y - (height / 2), 0);

        for(int y = start.y; y < start.y + (height - 3); y++) {
            for(int x = start.x; x < start.x + width; x++) {
                if (lakeObjectsTilemap.GetTile(new Vector3Int(x, y, 0)) != null || lakeObjectsTilemap.GetTile(new Vector3Int(x, y + 1, 0)) != null)
                    continue;
                if (Random.Range(0, 20) == 0) {
                    if (y - start.y > 3 && x - start.x > 3 && start.x + width - x > 3) {
                        backgroundLakeObjects.SetTile(new Vector3Int(x, y, 0), _waterPlants[Random.Range(0, _waterPlants.Length)]);
                    } else {
                        backgroundLakeObjects.SetTile(new Vector3Int(x, y, 0), _waterRocks[Random.Range(0, _waterRocks.Length)]);
                    }
                }
            }
        }
    }

    private void GenerateReeds(Vector3Int pos, int xLength, int yLength) {
        // 1st X row
        for(int i = pos.x; i < pos.x + xLength; i++) {
            if (Random.Range(0, 10) >= 2) { 
                lakeObjectsTilemap.SetTile(new Vector3Int(i, pos.y, pos.z), _reeds);
            }
        }
        // 2nd X row
        for(int i = pos.x; i < pos.x + xLength; i++) {
            if (Random.Range(0, 10) >= 2) { 
                lakeObjectsTilemap.SetTile(new Vector3Int(i, pos.y - 1, pos.z), _reeds);
            }
        }
        // 1st Y row
        for(int i = pos.y; i > pos.y - (yLength / 1.5f); i--) {
            if (lakeObjectsTilemap.GetTile(new Vector3Int(pos.x, i, pos.z)) != null)
                continue;

            if (Random.Range(0, 2) == 0) {
                lakeObjectsTilemap.SetTile(new Vector3Int(pos.x, i, pos.z), _reeds);
            }
        }
        // 2nd Y row
        for(int i = pos.y; i > pos.y - (yLength / 1.5f); i--) {
            if (lakeObjectsTilemap.GetTile(new Vector3Int(pos.x + 1, i, pos.z)) != null)
                continue;

            if (Random.Range(0, 2) == 0) {
                lakeObjectsTilemap.SetTile(new Vector3Int(pos.x + 1, i, pos.z), _reeds);
            }
        }
        // 3rd Y row
        for(int i = pos.y; i > pos.y - (yLength / 1.5f); i--) {
            if (lakeObjectsTilemap.GetTile(new Vector3Int(pos.x + (xLength - 2), i, pos.z)) != null)
                continue;

            if (Random.Range(0, 2) == 0) {
                lakeObjectsTilemap.SetTile(new Vector3Int(pos.x + (xLength - 2), i, pos.z), _reeds);
            }
        }
        // 4th Y row
        for(int i = pos.y; i > pos.y - (yLength / 1.5f); i--) {
            if (lakeObjectsTilemap.GetTile(new Vector3Int(pos.x + (xLength - 1), i, pos.z)) != null)
                continue;

            if (Random.Range(0, 2) == 0) {
                lakeObjectsTilemap.SetTile(new Vector3Int(pos.x + (xLength - 1), i, pos.z), _reeds);
            }
        }
    }

    private bool SwapDirection(int cutoff) {
        if (_tileCounter > 35)
            return true;
        if (_tileCounter < 7)
            return false;
        return Random.Range(0, cutoff) == 0;
    }
    
    private bool NearbyLakeExists(Vector3 newSpawn) {
        foreach(Vector3 lakeSpawn in lakeSpawnPositions) {
            if (Vector3.Distance(lakeSpawn, newSpawn) < 60) {
                return true;
            }
        }
        return false;
    }
}