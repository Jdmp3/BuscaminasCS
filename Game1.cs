using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Security.Principal;

namespace BuscaminasCS;

public class Game1 : Game
{

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    Texture2D pixel;
    Texture2D bgSprite;
    int totalFrames = 5;
    int currentFrame = 0;
    float frameTimer = 0f;
    float frameDuration = 0.6f;

    int row = 10;
    int colm = 10;
    int tileSize = 40;

    int offsetX;
    int offsetY;

    MouseState _previousMouse;

    KeyboardState _previousKeyboard;
    Rectangle _pressedBounds = Rectangle.Empty;

    Tile [,] grid;
    Random random = new();

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.ClientSizeChanged += OnWindowSizeChanged;
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

        bgSprite = Content.Load<Texture2D>("SpaceWallpaperSheet");
    }


    private void OnWindowSizeChanged(Object sender, EventArgs e)
    {
        offsetX = (GraphicsDevice.Viewport.Width - colm * tileSize) / 2;
        offsetY = (GraphicsDevice.Viewport.Height - row * tileSize) / 2;
        for(int y = 0; y < row; y++)
        {
            for(int x = 0; x < row; x++){
            grid[y, x].Bounds = new Rectangle(offsetX + x * tileSize, offsetY + y * tileSize, tileSize, tileSize);
            }
        }
    }


    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (keyboard.IsKeyDown(Keys.F11) && _previousKeyboard.IsKeyUp(Keys.F11))
        {
            _graphics.ToggleFullScreen();
            
        }



        MouseState mouse = Mouse.GetState();




        if (mouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
        {
            int x = (mouse.X - offsetX) / tileSize;
            int y = (mouse.Y - offsetY) / tileSize;
            if (x >= 0 && x < colm && y >= 0 && y < row)
            {
                _pressedBounds = grid[y, x].Bounds;
            }
        }

        if (_pressedBounds != Rectangle.Empty && mouse.LeftButton == ButtonState.Pressed)
        {
            if (!_pressedBounds.Contains(mouse.X, mouse.Y))
            {
                _pressedBounds = Rectangle.Empty;
            }
        }

        if (mouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
        {
            if (_pressedBounds != Rectangle.Empty && _pressedBounds.Contains(mouse.X, mouse.Y))
            {
                int x = (_pressedBounds.X - offsetX) / tileSize;
                int y = (_pressedBounds.Y - offsetY) / tileSize;
                grid[y, x].IsRevealed = true;
            }
            _pressedBounds = Rectangle.Empty;
        }

        _previousMouse = mouse;

        frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (frameTimer >= frameDuration)
        {
            currentFrame = (currentFrame + 1) % totalFrames;
            frameTimer -= frameDuration;
        }

        _previousKeyboard = keyboard;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        Rectangle sourceRect = new Rectangle(currentFrame * 1280, 0, 1280, 720);
        _spriteBatch.Draw(bgSprite, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), sourceRect, Color.White);

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
                if (tile.Bounds == _pressedBounds)
                    _spriteBatch.Draw(pixel, tile.Bounds, Color.Gray);
                else
                    _spriteBatch.Draw(pixel, tile.Bounds, Color.DarkGray);
            }
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
