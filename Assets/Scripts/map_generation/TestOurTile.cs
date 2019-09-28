using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestOurTile : MonoBehaviour {
	
	// Update is called once per frame
	private void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			/*Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
			var worldPoint = new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y), 0);

			var tiles = WorldGenerator.Instance.groundTiles; // This is our Dictionary of tiles
            WorldTile _tile;
			if (tiles.TryGetValue(worldPoint, out _tile)) 
			{
				print("Tile " + _tile.Name + " costs: " + _tile.Cost + " Location: " + _tile.WorldLocation);
                WorldGenerator.Instance.treeTilemap.SetTile(Vector3Int.CeilToInt(_tile.WorldLocation), LakeGenerator.Instance._waterTile);
				//_tile.TilemapMember.SetTileFlags(_tile.LocalPlace, TileFlags.None);
				//_tile.TilemapMember.SetColor(_tile.LocalPlace, Color.green);
                //Debug.Log(LakeGenerator.Instance.lakeTilemap.GetTile(_tile.LocalPlace) == null);
			}*/
		}
	}
}