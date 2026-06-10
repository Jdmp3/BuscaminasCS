using Microsoft.Xna.Framework;

namespace BuscaminasCS;

public class Tile()
{
    public Rectangle Bounds;
    public bool Flagged;
    public bool IsMine;
    public bool IsRevealed;
    public bool IsRevealing;
    public float RevealTime;
    public bool IsFlagAnimating;
    public float FlagAnimTime;
    public bool FlagAnimForward;
}
