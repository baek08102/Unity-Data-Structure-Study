using UnityEngine;

public enum Sides
{
    Top,
    Left,
    Right,
    Bottom,
}

public class Tile
{

    public static readonly int[] tableWeight =
    {
        int.MaxValue,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
        2,4,int.MaxValue,1,1,1
    };

    public bool CanMove => autoTileId != (int)TileTypes.Empty && Weight < int.MaxValue;

    public int Weight
    {
        get
        {
            if (autoTileId < 0 || autoTileId >= tableWeight.Length)
            {
                return int.MaxValue;
            }
            return tableWeight[autoTileId];
        }
    }

    public int id;
    public Tile[] adjacents = new Tile[4];
    public Tile previous = null;

    public int autoTileId;
    public bool CanVisit => adjacents.Length > 0;

    public bool isVisited;
    public int fowId;

    public void UpdateAutoFowId()
    {
        fowId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null || !adjacents[i].isVisited)
            {
                fowId |= 1 << i;
            }
        }
    }

    public void UpdateAutoTileId()
    {
        autoTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null)
            {
                autoTileId |= 1 << i;
            }
        }
    }

    public void RemoveAdjacents(Tile tile)
    {
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null)
                continue;
            if (adjacents[i].id == tile.id)
            {
                adjacents[i] = null;
                break;
            }
        }

        UpdateAutoTileId();
        UpdateAutoFowId();
    }

    public void ClearAdjacents()
    {
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null)
                continue;
            adjacents[i].RemoveAdjacents(this);
            adjacents[i] = null;
        }

        UpdateAutoTileId();
    }
}
