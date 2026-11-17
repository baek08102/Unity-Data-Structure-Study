using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public enum TileTypes
{
    Empty = -1,
    //0, 14
    Grass = 15,
    Woods,
    Hills,
    Mountains,
    Towns,
    Castle = 20,
    Monster,
}

public class Map
{
    public int rows = 0;
    public int cols = 0;

    public Tile[] tiles;

    // public List<int> TownIndex => TownIndex;
    public int tIdx;

    public Tile[] CoastTiles => tiles.Where(
        t => t.autoTileId >= 0 && t.autoTileId < (int)TileTypes.Grass).ToArray();
    public Tile[] LandTiles => tiles.Where(
    t => t.autoTileId >= (int)TileTypes.Grass).ToArray();

    public void Init(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;

        tiles = new Tile[rows * cols];
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i] = new Tile
            {
                id = i
            };
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < rows; c++)
            {
                int index = r * cols + c;
                var adjacents = tiles[index].adjacents;
                if ((r - 1) >= 0)
                {
                    adjacents[(int)Sides.Top] = tiles[index - cols];
                }
                if ((c + 1) < cols)
                {
                    adjacents[(int)Sides.Right] = tiles[index + 1];
                }
                if ((c - 1) >= 0)
                {
                    adjacents[(int)Sides.Left] = tiles[index - 1];
                }
                if ((r + 1) < rows)
                {
                    adjacents[(int)Sides.Bottom] = tiles[index + cols];
                }
            }
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].UpdateAutoTileId();
        }
    }

    public bool CreateIsland(
        float erodePercent,
        float erodeIterations,
        float lakePercent,
        int castleCount,
        int woodsCount,
        int hillCount,
        int mountainCount,
        int townCount,
        int dungeonCount
    )
    {
        DecorateTiles(LandTiles, lakePercent, TileTypes.Empty);
        for (int i = 0; i < erodeIterations; i++)
        {
            DecorateTiles(CoastTiles, erodePercent, TileTypes.Empty);
        }

        DecorateTiles(LandTiles, castleCount, TileTypes.Castle);
        DecorateTiles(LandTiles, woodsCount, TileTypes.Woods);
        DecorateTiles(LandTiles, hillCount, TileTypes.Hills);
        DecorateTiles(LandTiles, mountainCount, TileTypes.Mountains);
        DecorateTiles(LandTiles, townCount, TileTypes.Towns);
        DecorateTiles(LandTiles, dungeonCount, TileTypes.Monster);

        return true;
    }

    public void DecorateTiles(Tile[] tiles, float percent, TileTypes tileType)
    {
        int total = Mathf.FloorToInt(tiles.Length * percent);

        DecorateTiles(tiles, total, tileType);
    }
    public void DecorateTiles(Tile[] tiles, int count, TileTypes tileType)
    {
        ShuffleTiles(tiles);

        for (int i = 0; i < count; i++)
        {
            if (tileType == TileTypes.Empty)
            {
                tiles[i].ClearAdjacents();
            }
            tiles[i].autoTileId = (int)tileType;

            if (tiles[i].autoTileId == (int)TileTypes.Towns)
            {
                tIdx = i;
                // TownIndex.Add(tiles[i].autoTileId);
            }
        }
    }

    public void ShuffleTiles(Tile[] tiles)
    {
        // 피셔 예이츠 셔플 알고리즘

        for (int i = tiles.Length - 1; i > 0; i--)
        {
            int index = Random.Range(0, i + 1);
            Tile temp = tiles[i];
            tiles[i] = tiles[index];
            tiles[index] = temp;
        }
    }
}
