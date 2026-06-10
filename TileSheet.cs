using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BuscaminasCS;

public class TileSheet
{
    private Texture2D texture;
    private int frameSize = 40;

    private int idleFrames = 3;
    private int idleCurrentFrame = 0;
    private int idleDirection = 1;
    private float idleTimer = 0f;
    private float idleDuration = 0.6f;

    private int revealFrames = 5;
    private float revealFrameDuration = 0.06f;
    private float revealFadeDuration = 0.2f;

    private float RevealPlayDuration => revealFrames * revealFrameDuration;
    private float RevealTotalDuration => RevealPlayDuration + revealFadeDuration;

    private int flagFrames = 8;
    private float flagFrameDuration = 0.06f;
    private float FlagPlayDuration => flagFrames * flagFrameDuration;

    private int mineFrames = 3;
    private int mineCurrentFrame = 0;
    private int mineDirection = 1;
    private float mineTimer = 0f;
    private float mineDuration = 0.6f;

    public TileSheet(ContentManager content)
    {
        texture = content.Load<Texture2D>("SheetTile");
    }

    public void Update(GameTime gameTime)
    {
        idleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (idleTimer >= idleDuration)
        {
            idleCurrentFrame += idleDirection;
            if (idleCurrentFrame >= idleFrames - 1 || idleCurrentFrame <= 0)
                idleDirection *= -1;
            idleTimer -= idleDuration;
        }

        mineTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (mineTimer >= mineDuration)
        {
            mineCurrentFrame += mineDirection;
            if (mineCurrentFrame >= mineFrames - 1 || mineCurrentFrame <= 0)
                mineDirection *= -1;
            mineTimer -= mineDuration;
        }
    }

    public void UpdateReveal(Tile tile)
    {
        if (tile.RevealTime >= RevealTotalDuration)
        {
            tile.State = TileState.Revealed;
        }
    }

    public void UpdateFlagAnimation(Tile tile)
    {
        if (tile.FlagAnimTime >= FlagPlayDuration)
        {
            if (tile.State == TileState.Flagging)
                tile.State = TileState.Flagged;
            else if (tile.State == TileState.Unflagging)
                tile.State = TileState.Hidden;
        }
    }

    public void DrawIdleTile(SpriteBatch spriteBatch, Rectangle bounds, bool isPressed)
    {
        var source = new Rectangle(idleCurrentFrame * frameSize, 0, frameSize, frameSize);
        var tint = isPressed ? Color.Gray : Color.White;
        spriteBatch.Draw(texture, bounds, source, tint);
    }

    public void DrawFlagTile(SpriteBatch spriteBatch, Rectangle bounds, float animTime, bool forward)
    {
        float t = Math.Min(animTime, FlagPlayDuration);
        int frame;
        if (forward)
            frame = Math.Min((int)(t / flagFrameDuration), flagFrames - 1);
        else
            frame = Math.Max(0, flagFrames - 1 - (int)(t / flagFrameDuration));
        var source = new Rectangle(frame * frameSize, 3 * frameSize, frameSize, frameSize);
        spriteBatch.Draw(texture, bounds, source, Color.White);
    }

    public void DrawEmptyTile(SpriteBatch spriteBatch, Rectangle bounds)
    {
        var source = new Rectangle(0, 4 * frameSize, frameSize, frameSize);
        spriteBatch.Draw(texture, bounds, source, Color.White);
    }

    public void DrawNumberTile(SpriteBatch spriteBatch, Rectangle bounds, int number)
    {
        var source = new Rectangle(number * frameSize, 4 * frameSize, frameSize, frameSize);
        spriteBatch.Draw(texture, bounds, source, Color.White);
    }

    public void DrawMineTile(SpriteBatch spriteBatch, Rectangle bounds)
    {
        var source = new Rectangle(mineCurrentFrame * frameSize, 2 * frameSize, frameSize, frameSize);
        spriteBatch.Draw(texture, bounds, source, Color.White);
    }

    public void DrawRevealTile(SpriteBatch spriteBatch, Rectangle bounds, float revealTime, Texture2D pixel)
    {
        float t = Math.Min(revealTime, RevealTotalDuration);

        if (t < RevealPlayDuration)
        {
            int frame = (int)(t / revealFrameDuration);
            var source = new Rectangle(frame * frameSize, frameSize, frameSize, frameSize);
            spriteBatch.Draw(texture, bounds, source, Color.White);
        }
        else
        {
            float fadeT = (t - RevealPlayDuration) / revealFadeDuration;
            float alpha = MathHelper.Lerp(1f, 0f, fadeT);
            spriteBatch.Draw(pixel, bounds, Color.Black * alpha);
        }
    }
}
