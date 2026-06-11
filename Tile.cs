using Microsoft.Xna.Framework;

namespace BuscaminasCS;

public enum TileState
{
    Hidden,
    Revealing,
    Revealed,
    Flagging,
    Flagged,
    Unflagging,
}

public class Tile()
{
    public Rectangle Bounds;
    public bool IsMine;
    public int NeighborMineCount;
    public TileState State;
    public float RevealTime;
    public float FlagAnimTime;
}
