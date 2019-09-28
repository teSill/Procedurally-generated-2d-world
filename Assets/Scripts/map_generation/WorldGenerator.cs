using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class WorldGenerator : MonoBehaviour {

    [SerializeField]
    private AstarPath _pathfinding;

    [SerializeField]
    private GameObject _tilemapParent;

    public TileBase tile;
    public TileBase grownTreeTile;
    public TileBase deadTreeTile;

	public Tilemap tilemap;
    public Tilemap treeTilemap;

	public Dictionary<Vector3, WorldTile> groundTiles;
    public Dictionary<Vector3, WorldTile> treeTiles;

    public const int TILES_PER_SIDE = 400; // 800x800

    public delegate void SpawnAction(Vector3Int pos);
    public static event SpawnAction OnSpawnEvent;

    public static WorldGenerator Instance;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		} else if (Instance != this) {
			Destroy(gameObject);
		}
        OnSpawnEvent += SpawnGrass;
        OnSpawnEvent += SpawnTree;
	}

    public void GenerateWorld() {
        for(int y = 0; y < TILES_PER_SIDE; y++) {
            for(int x = 0; x < TILES_PER_SIDE; x++) {
                var adjustedTile = -Vector2.one * (TILES_PER_SIDE - 1) * 0.5f + new Vector2(x, y);
                OnSpawnEvent(new Vector3Int((int) adjustedTile.x, (int) adjustedTile.y, 0));
            }
        }
        SetWorldTiles();
        SetWorldTreeTiles();
        StartCoroutine(AnimalControl.Instance.SpawnAnimals());
    }

    private void SpawnGrass(Vector3Int pos) {
        tilemap.SetTile(pos, tile);
    }

    private void SpawnTree(Vector3Int pos) {
        if (LakeGenerator.Instance.lakeTilemap.GetTile(pos) == null && LakeGenerator.Instance.lakeTilemap.GetTile(new Vector3Int(pos.x, pos.y - 1, pos.z)) == null && Vector3.Distance(pos, new Vector3(0, 0, 0)) > 10) {
            int modifier = 20;
            if (treeTilemap.GetTile(new Vector3Int(pos.x + 1, pos.y, pos.z)) != null ||
                treeTilemap.GetTile(new Vector3Int(pos.x - 1, pos.y, pos.z)) != null ||
                treeTilemap.GetTile(new Vector3Int(pos.x, pos.y + 1, pos.z)) != null ||
                treeTilemap.GetTile(new Vector3Int(pos.x, pos.y - 1, pos.z)) != null ||
                treeTilemap.GetTile(new Vector3Int(pos.x + 1, pos.y + 1, pos.z)) != null ||
                treeTilemap.GetTile(new Vector3Int(pos.x - 1, pos.y - 1, pos.z)) != null ||
                treeTilemap.GetTile(new Vector3Int(pos.x + 1, pos.y + 1, pos.z)) != null ||
                treeTilemap.GetTile(new Vector3Int(pos.x - 1, pos.y - 1, pos.z)) != null) {
                modifier = 4;
            }
            if (Random.Range(0, modifier) == 0) {
                treeTilemap.SetTile(pos, grownTreeTile);
            }
        }
    }
    
	private void SetWorldTiles() {
        groundTiles = new Dictionary<Vector3, WorldTile>();
		foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin) {
			var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

			if (!tilemap.HasTile(localPlace)) continue;
			var tile = GetWorldTile(localPlace, tilemap);
			
			groundTiles.Add(tile.WorldLocation, tile);
		}
        Debug.Log(groundTiles.Count);
	}

    private void SetWorldTreeTiles() {
        treeTiles = new Dictionary<Vector3, WorldTile>();
        foreach (Vector3Int pos in treeTilemap.cellBounds.allPositionsWithin) {
            var localPlace = new Vector3Int(pos.x, pos.y, pos.z);

			if (!treeTilemap.HasTile(localPlace)) continue;
			var tile = GetWorldTile(localPlace, treeTilemap);
			
			treeTiles.Add(tile.WorldLocation, tile);
        }
        Debug.Log(treeTiles.Count);
    }

    public WorldTile GetWorldTile(Vector3Int localPos, Tilemap tilemap) {
        var tile = new WorldTile {
			LocalPlace = localPos,
			WorldLocation = tilemap.CellToWorld(localPos),
			TileBase = tilemap.GetTile(localPos),
			TilemapMember = tilemap,
			Name = localPos.x + "," + localPos.y,
			Cost = 1 // TODO: Change this with the proper cost from ruletile
		};
        
        return tile;
    }

    public bool IsWalkableTile(Vector3Int pos) {
        return !treeTilemap.HasTile(pos) && !LakeGenerator.Instance.lakeTilemap.HasTile(pos);
    }
}
