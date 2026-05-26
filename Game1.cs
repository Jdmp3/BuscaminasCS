using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BuscaminasCS;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    Texture2D pixel;

    int row = 10;
    int colm = 10;
    int tileSize = 40;

    int offsetX;
    int offsetY;


    Tile [,] grid;
    Random random = new();

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    public class Tile()
    {
        public Rectangle Bounds;
        public bool IsMine;
        public bool IsRevealed;
    }

    protected override void Initialize()
    {
        int gridWidth = colm * tileSize;
        int gridHeight = row * tileSize;
        offsetX = (GraphicsDevice.Viewport.Width - gridWidth) / 2;
        offsetY = (GraphicsDevice.Viewport.Height - gridHeight) / 2;

        grid = new Tile [row, colm];

        for (int y = 0; y < row; y++)
            for (int x = 0; x < colm; x++)
                grid[y, x] = new Tile { Bounds = new Rectangle(offsetX + x * tileSize, offsetY + y * tileSize, tileSize, tileSize) };

              int minesPlaced = 0;
                  while (minesPlaced < 10)
        {
            int rx = random.Next(colm);
            int ry = random.Next(row);
            if (!grid[ry, rx].IsMine)
            {
                grid[ry, rx].IsMine = true;
                minesPlaced++;
            }
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] {Color.White});
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        MouseState mouse = Mouse.GetState();

        if (mouse.LeftButton == ButtonState.Pressed)
        {
           int x = (mouse.X - offsetX) / tileSize;
           int y = (mouse.Y - offsetY) / tileSize;

           if(x >= 0 && x < row && y >= 0 && y < colm)
            {
                Tile tile = grid[y, x];
                if (!tile.IsRevealed)
                {
                    tile.IsRevealed = true;
                }
            }
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Beige);

        _spriteBatch.Begin();

        foreach (Tile tile in grid)
        {
            if (tile.IsRevealed && tile.IsMine)
            {
                _spriteBatch.Draw(pixel, tile.Bounds, Color.Red);

                float DetectorDiagonal = MathF.Sqrt(2) * (tile.Bounds.Width - 4 * 2);

                _spriteBatch.Draw(pixel, new Vector2(tile.Bounds.X + tile.Bounds.Width/ 2f, tile.Bounds.Y + tile.Bounds.Height/ 2f), null, Color.Black, MathHelper.ToRadians(45), new Vector2(0.5f, 0.5f), new Vector2(DetectorDiagonal, 4), SpriteEffects.None, 0f);
                _spriteBatch.Draw(pixel, new Vector2(tile.Bounds.X + tile.Bounds.Width / 2f, tile.Bounds.Y + tile.Bounds.Height/ 2f), null, Color.Black, MathHelper.ToRadians(-45), new Vector2(0.5f, 0.5f), new Vector2(DetectorDiagonal, 4), SpriteEffects.None, 0f);
            }
            else if (tile.IsRevealed)
            {
                _spriteBatch.Draw(pixel, tile.Bounds, Color.LightGray);
            }
            else
            {
                _spriteBatch.Draw(pixel, tile.Bounds, Color.DarkGray);
            }

            _spriteBatch.Draw(
                pixel,
                new Rectangle(tile.Bounds.X, tile.Bounds.Y, tile.Bounds.Width, 2),
                Color.Magenta
            );
        }

        for (int x = 0; x <= colm; x++)
        {
            _spriteBatch.Draw(
                pixel,
                new Rectangle(offsetX + x * tileSize, offsetY, 2, row * tileSize),
                Color.Magenta
            );
        }

        for (int y = 0; y <= row; y++)
        {
            _spriteBatch.Draw(
                pixel,
                new Rectangle(offsetX, offsetY + y * tileSize, colm * tileSize, 2),
                Color.Magenta
            );
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
