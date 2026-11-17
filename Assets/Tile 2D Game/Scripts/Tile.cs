public enum Sides
{
    Top,
    Left,
    Right,
    Bottom,
}

public class Tile
{
    public int id;
    public Tile[] adjacents = new Tile[4];

    public int autoTileId;

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
