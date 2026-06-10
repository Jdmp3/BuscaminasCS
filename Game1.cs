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

    bool _firstClick = true;

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

        PlaceMines();
        _firstClick = true;

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

        PlaceMines();
        _firstClick = true;
        _pressedBounds = Rectangle.Empty;
        _previousMouse = default;
        _previousKeyboard = default;
    }

    private void PlaceMines()
    {
        int[,] tempCount = new int[row, colm];
        int minesPlaced = 0;

        while (minesPlaced < 10)
        {
            int rx = random.Next(colm);
            int ry = random.Next(row);
            if (!grid[ry, rx].IsMine)
            {
                bool canPlace = true;
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dy == 0 && dx == 0) continue;
                        int ny = ry + dy, nx = rx + dx;
                        if (ny >= 0 && ny < row && nx >= 0 && nx < colm)
                        {
                            if (tempCount[ny, nx] >= 6)
                            {
                                canPlace = false;
                                break;
                            }
                        }
                    }
                    if (!canPlace) break;
                }

                if (canPlace)
                {
                    grid[ry, rx].IsMine = true;
                    for (int dy = -1; dy <= 1; dy++)
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            if (dy == 0 && dx == 0) continue;
                            int ny = ry + dy, nx = rx + dx;
                            if (ny >= 0 && ny < row && nx >= 0 && nx < colm)
                                tempCount[ny, nx]++;
                        }
                    minesPlaced++;
                }
            }
        }

        for (int y = 0; y < row; y++)
            for (int x = 0; x < colm; x++)
                grid[y, x].NeighborMineCount = tempCount[y, x];
    }

    private void RecalculateNeighborCounts()
    {
        for (int y = 0; y < row; y++)
        {
            for (int x = 0; x < colm; x++)
            {
                int count = 0;
                for (int dy = -1; dy <= 1; dy++)
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dy == 0 && dx == 0) continue;
                        int ny = y + dy, nx = x + dx;
                        if (ny >= 0 && ny < row && nx >= 0 && nx < colm && grid[ny, nx].IsMine)
                            count++;
                    }
                grid[y, x].NeighborMineCount = count;
            }
        }
    }

    private void RevealTile(int y, int x)
    {
        if (y < 0 || y >= row || x < 0 || x >= colm) return;
        var tile = grid[y, x];

        if (tile.State != TileState.Hidden || tile.IsMine)
            return;

        tile.State = TileState.Revealing;
        tile.RevealTime = 0f;

        if (tile.NeighborMineCount == 0)
        {
            for (int dy = -1; dy <= 1; dy++)
                for (int dx = -1; dx <= 1; dx++)
                    if (dy != 0 || dx != 0)
                        RevealTile(y + dy, x + dx);
        }
    }


    private void OnWindowSizeChanged(Object sender, EventArgs e)
    {
        offsetX = (GraphicsDevice.Viewport.Width - colm * tileSize) / 2;
        offsetY = (GraphicsDevice.Viewport.Height - row * tileSize) / 2;
        for(int y = 0; y < row; y++)
        {
            for(int x = 0; x < colm; x++){
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
                if (grid[y, x].State == TileState.Hidden)
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
                var tile = grid[y, x];

                if (tile.State == TileState.Hidden)
                {
                    if (_firstClick && tile.IsMine)
                    {
                        tile.IsMine = false;
                        int ny, nx;
                        do {
                            ny = random.Next(row);
                            nx = random.Next(colm);
                        } while (grid[ny, nx].IsMine || (ny == y && nx == x));
                        grid[ny, nx].IsMine = true;
                        RecalculateNeighborCounts();
                        _firstClick = false;
                    }
                    else
                    {
                        _firstClick = false;
                    }

                    if (!tile.IsMine)
                        RevealTile(y, x);
                    else
                    {
                        tile.State = TileState.Revealing;
                        tile.RevealTime = 0f;
                    }
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
                if (tile.State == TileState.Hidden)
                {
                    tile.State = TileState.Flagging;
                    tile.FlagAnimTime = 0f;
                }
                else if (tile.State == TileState.Flagged)
                {
                    tile.State = TileState.Unflagging;
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
            if (tile.State == TileState.Revealing)
            {
                tile.RevealTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                tileSheet.UpdateReveal(tile);
            }

            if (tile.State == TileState.Flagging || tile.State == TileState.Unflagging)
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
            if (tile.State == TileState.Flagged || tile.State == TileState.Flagging || tile.State == TileState.Unflagging)
            {
                if (tile.State == TileState.Flagged)
                    tileSheet.DrawFlagTile(_spriteBatch, tile.Bounds, float.MaxValue, true);
                else
                    tileSheet.DrawFlagTile(_spriteBatch, tile.Bounds, tile.FlagAnimTime, tile.State == TileState.Flagging);
                continue;
            }

            if (tile.State == TileState.Revealed || tile.State == TileState.Revealing)
            {
                if (tile.IsMine)
                    tileSheet.DrawMineTile(_spriteBatch, tile.Bounds);
                else if (tile.NeighborMineCount > 0)
                    tileSheet.DrawNumberTile(_spriteBatch, tile.Bounds, tile.NeighborMineCount);
                else
                    tileSheet.DrawEmptyTile(_spriteBatch, tile.Bounds);
            }

            if (tile.State == TileState.Revealing)
                tileSheet.DrawRevealTile(_spriteBatch, tile.Bounds, tile.RevealTime, pixel);

            if (tile.State == TileState.Hidden)
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
