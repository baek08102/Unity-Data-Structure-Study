using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject[] tileObjs;

    public int mapWidth = 20;
    public int mapHeight = 20;

    public Vector2 tileSize = new Vector2(16, 16);

    public Sprite[] islandSprites;

    private Map map;
    public Map Map => map;

    public List<int> decoCounts = new List<int>();

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

    [ContextMenu("Reset Stage")]
    public void ResetStage()
    {
        map = new Map();
        map.Init(mapHeight, mapWidth);
        map.CreateIsland(erodePercent, erodeIterations, lakePercent,
       castleCount, woodsCount, hillCount, mountainCount, townCount, dungeonCount);

        CreateGrid();
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

        IsCastleReachable();
    }

    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];
        var ren = tileGo.GetComponent<SpriteRenderer>();

        if (tile.autoTileId != (int)TileTypes.Empty)
        {
            ren.sprite = islandSprites[tile.autoTileId];
        }
        else
        {
            ren.sprite = null;
        }
    }

    public bool IsCastleReachable()
    {
        int startTileId = Random.Range(0, map.tIdx + 1);
        SpriteRenderer tSprite = tileObjs[startTileId].GetComponentInChildren<SpriteRenderer>();
        tSprite.color = Color.red;



        return false;
    }

    // private void SetDecoCounts()
    // {
    //     decoCounts.Add()
    // }
}
