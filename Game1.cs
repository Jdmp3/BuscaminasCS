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
    Texture2D bgSprite;
    TileSheet tileSheet;
    SpriteFont FontBase;
    int totalFrames = 5;
    int currentFrame = 0;
    float frameTimer = 0f;
    float frameDuration = 0.6f;
    Button ResetButton;

    int row = 10;
    int colm = 10;
    int tileSize = 45;

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

        FontBase = Content.Load<SpriteFont>("FontBase");

        ResetButton = new Button
        {
            Bounds = new Rectangle(
                GraphicsDevice.Viewport.Width - 160,
                GraphicsDevice.Viewport.Height - 65,
                140,
                45
            ),
            text = "Reiniciar",
            OnClick = ResetGame
        };

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] {Color.White});

        bgSprite = Content.Load<Texture2D>("SpaceWallpaperSheet");
        tileSheet = new TileSheet(Content);
    }



    private void ResetGame()
    {
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
        _pressedBounds = Rectangle.Empty;
        _previousMouse = default;
        _previousKeyboard = default;
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

        ResetButton.Bounds = new Rectangle(
            GraphicsDevice.Viewport.Width - 160,
            GraphicsDevice.Viewport.Height - 65,
            140,
            45
        );

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


        ResetButton.Update(mouse, _previousMouse);



        //  Deteccion de Click Izquierdo Para Revelar


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
                if(!grid[y, x].Flagged && !grid[y, x].IsFlagAnimating && !grid[y, x].IsRevealed && !grid[y, x].IsRevealing)
                { 
                     grid[y, x].IsRevealing = true;
                     grid[y, x].RevealTime = 0f;
                }
            }
            _pressedBounds = Rectangle.Empty;
        }


        //Flaging si hay Click Derecho



        if (mouse.RightButton == ButtonState.Pressed && _previousMouse.RightButton == ButtonState.Released)
        {
            int x = (mouse.X - offsetX) / tileSize;
            int y = (mouse.Y - offsetY) / tileSize;
            if (x >= 0 && x < colm && y >= 0 && y < row)
            {
                _pressedBounds = grid[y, x].Bounds;
            }
        }

        if (_pressedBounds != Rectangle.Empty && mouse.RightButton == ButtonState.Pressed)
        {
            if (!_pressedBounds.Contains(mouse.X, mouse.Y))
            {
                _pressedBounds = Rectangle.Empty;
            }
        }


        if (mouse.RightButton == ButtonState.Released && _previousMouse.RightButton == ButtonState.Pressed)
        {
            if(_pressedBounds != Rectangle.Empty && _pressedBounds.Contains(mouse.X, mouse.Y))
            {
                int x = (_pressedBounds.X - offsetX) / tileSize;
                int y = (_pressedBounds.Y - offsetY) / tileSize;
                var tile = grid[y, x];
                if(!tile.IsRevealed && !tile.IsRevealing && !tile.IsFlagAnimating)
                { 
                     tile.IsFlagAnimating = true;
                     tile.FlagAnimForward = !tile.Flagged;
                     tile.FlagAnimTime = 0f;
                }
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

        foreach (var tile in grid)
        {
            if (tile.IsRevealing)
            {
                tile.RevealTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                tileSheet.UpdateReveal(tile);
            }

            if (tile.IsFlagAnimating)
            {
                tile.FlagAnimTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                tileSheet.UpdateFlagAnimation(tile);
            }
        }

        tileSheet.Update(gameTime);

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
            if (tile.Flagged || tile.IsFlagAnimating)
            {
                if (tile.IsFlagAnimating)
                    tileSheet.DrawFlagTile(_spriteBatch, tile.Bounds, tile.FlagAnimTime, tile.FlagAnimForward);
                else
                    tileSheet.DrawFlagTile(_spriteBatch, tile.Bounds, float.MaxValue, true);
                continue;
            }

            if (tile.IsRevealed || tile.IsRevealing)
            {
                if (tile.IsMine)
                    tileSheet.DrawMineTile(_spriteBatch, tile.Bounds);
                else if (!tile.IsMine)
                    tileSheet.DrawEmptyTile(_spriteBatch, tile.Bounds);
            }

            if (tile.IsRevealing)
                tileSheet.DrawRevealTile(_spriteBatch, tile.Bounds, tile.RevealTime, pixel);

            if (!tile.IsRevealed && !tile.IsRevealing)
                tileSheet.DrawIdleTile(_spriteBatch, tile.Bounds, tile.Bounds == _pressedBounds);
        }

        for (int x = 0; x <= colm; x++)
        {
            _spriteBatch.Draw(
                pixel,
                new Rectangle(offsetX + x * tileSize, offsetY, 2, row * tileSize),
                Color.DimGray
            );
        }

        for (int y = 0; y <= row; y++)
        {
            _spriteBatch.Draw(
                pixel,
                new Rectangle(offsetX, offsetY + y * tileSize, colm * tileSize, 2),
                Color.DimGray
            );
        }

        ResetButton.Draw(_spriteBatch, pixel, FontBase);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
