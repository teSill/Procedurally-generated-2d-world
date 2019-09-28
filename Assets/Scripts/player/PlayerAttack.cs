using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class PlayerAttack : MonoBehaviour {

    public TextMeshProUGUI text;
    private IEnumerator RespawnTree(Vector3Int pos, WorldTile tile) {
        float timer = 3f;
        while(timer >= 0) {
        text.gameObject.SetActive(true);
            timer -= Time.deltaTime;
            text.text = timer.ToString("F2");
            yield return null;
        }
        text.gameObject.SetActive(false);
        WorldGenerator.Instance.treeTilemap.SetTile(pos, WorldGenerator.Instance.grownTreeTile);
        tile.IsInteractable = true;
    }
    
    private void OnCollisionEnter2D(Collision2D collision) {
        switch(collision.gameObject.tag) {
            case "Tree":
                Tilemap tilemap = WorldGenerator.Instance.treeTilemap;

                Vector3 hitPosition = Vector3.zero;
                foreach (ContactPoint2D hit in collision.contacts)
                {
                    hitPosition.x = (hit.point.x - 0.01f * hit.normal.x);
                    hitPosition.y = (hit.point.y - 0.01f * hit.normal.y) + 1;
                }

                CheckForTrees(hitPosition);
                break;
            default:
                break;
        }
    }

    private void CheckForTrees(Vector3 hitPosition) {
        var tiles = WorldGenerator.Instance.treeTiles;
        WorldTile tile;
        
        if (!tiles.TryGetValue(new Vector3(Mathf.Round(hitPosition.x), Mathf.Round(hitPosition.y), hitPosition.z), out tile) &&
            !tiles.TryGetValue(new Vector3(Mathf.Round(hitPosition.x - 1), Mathf.Round(hitPosition.y), hitPosition.z), out tile) &&
            !tiles.TryGetValue(new Vector3(Mathf.Round(hitPosition.x + 1), Mathf.Round(hitPosition.y), hitPosition.z), out tile)) {
            Debug.Log("No nearby tree located.");
            return;
        }

        if (!tile.IsInteractable) {
            return;
        }

        ChopTree(tile);
    }

    private void ChopTree(WorldTile tile) {
        tile.IsInteractable = false;
        WorldGenerator.Instance.treeTilemap.SetTile(Vector3Int.CeilToInt(tile.WorldLocation), WorldGenerator.Instance.deadTreeTile);
        StartCoroutine(RespawnTree(Vector3Int.CeilToInt(tile.WorldLocation), tile));
    }
}
