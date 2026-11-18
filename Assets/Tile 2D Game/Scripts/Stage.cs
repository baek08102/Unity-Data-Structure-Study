using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject[] tileObjs;

    public int mapWidth = 20;
    public int mapHeight = 20;

    public Vector2 tileSize = new Vector2(16, 16);

    public Sprite[] islandSprites;
    public Sprite[] fowSprites;

    private Map map;
    public Map Map => map;

    public List<int> decoCounts = new List<int>();

    public List<Tile> path = new List<Tile>();

    [Header("스테이지 컴포넌트 설정")]

    [Range(0f, 1f)]
    public float erodePercent = 0.3f;
    public float erodeIterations = 2;

    [Range(0f, 1f)]
    public float lakePercent = 0.1f;

    public const int castleCount = 1;
    public int woodsCount = 11;
    public int hillCount = 6;
    public int mountainCount = 5;
    public int townCount = 3;
    public int dungeonCount = 3;

    private void Awake()
    {
        ResetStage();
    }

    [ContextMenu("Reset Stage")]
    public void ResetStage()
    {
        bool succeed = false;
        int maxAttemp = 10;

        for (int i = 0; i < maxAttemp; i++)
        {
            map = new Map();
            map.Init(mapHeight, mapWidth);
            succeed = map.CreateIsland(
                erodePercent, erodeIterations,
                lakePercent,
                castleCount,
                woodsCount,
                hillCount,
                mountainCount,
                townCount,
                dungeonCount
                );
        }

        if (!succeed)
        {
            Debug.LogError("맵 생성 실패!");
            map = null;
            return;
        }

        CreateGrid();
        CreatePlayer();
    }

    public Vector3 firstTilePos;
    public GameObject playerPrefab;
    public Player player;

    public void OnTileVisited(int tileId)
    {
        // Debug.Log($"OnTileVisited({tileId}): ({tileId / mapWidth}, {(tileId % mapWidth)} / {map.tiles}");
    }

    private void CreatePlayer()
    {
        if (player != null)
        {
            Destroy(player.gameObject);
            player = null;
        }

        // Instantiate(playerPrefab, GetTilePos(map.startTile.id), Quaternion.identity);
        var newGo = Instantiate(playerPrefab);
        player = newGo.GetComponent<Player>();
    }

    private void CreateGrid()
    {
        if (tileObjs != null)
        {
            foreach (var tile in tileObjs)
            {
                Destroy(tile.gameObject);
            }
        }
        tileObjs = new GameObject[mapWidth * mapHeight];

        Vector3 pos = Vector3.zero;
        float startX = pos.x;

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                var tileId = i * mapWidth + j;
                var tile = map.tiles[tileId];

                var newGo = Instantiate(tilePrefab, transform);
                newGo.transform.position = pos;
                pos.x += tileSize.x;

                newGo.name = $"Tile {tileId} ({i}, {j})";
                tileObjs[tileId] = newGo;

                DecorateTile(tileId);
            }
            pos.x = startX;
            pos.y -= tileSize.y;
        }
    }

    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];
        var ren = tileGo.GetComponent<SpriteRenderer>();

        if (!tile.isVisited)
        {
            ren.sprite = fowSprites[tile.fowTileId];
            return;
        }

        if (tile.autoTileId != (int)TileTypes.Empty)
        {
            ren.sprite = islandSprites[tile.autoTileId];
        }
        else
        {
            ren.sprite = null;
        }
    }

    public int ScreenPosToTileId(Vector3 screenPos, bool clamp = true)
    {
        Camera cam = Camera.main;
        screenPos.z = Mathf.Abs(transform.position.z - cam.transform.position.z);
        return WorldPosToTileId(Camera.main.ScreenToWorldPoint(screenPos), clamp = true);
    }

    public int WorldPosToTileId(Vector3 worldPos, bool clamp = true)
    {
        int x = Mathf.FloorToInt((worldPos.x - firstTilePos.x) / tileSize.x + 0.5f);
        int y = Mathf.FloorToInt((firstTilePos.y - worldPos.y) / tileSize.y + 0.5f);

        if (clamp)
        {
            x = Mathf.Clamp(x, 0, mapWidth - 1);
            y = Mathf.Clamp(y, 0, mapHeight - 1);
        }

        return y * mapWidth + x;
    }

    public Vector3 GetTilePos(int y, int x)
    {
        return firstTilePos + new Vector3(x * tileSize.x, -y * tileSize.y);
    }

    public Vector3 GetTilePos(int tileId)
    {
        return GetTilePos(tileId / mapWidth, tileId % mapWidth);
    }
}
