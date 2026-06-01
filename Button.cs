using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


public class Button
{
    public Rectangle Bounds;
    public string text;
    public Action OnClick;
    bool isHovered;

    public void Update(MouseState mouse, MouseState previousMouse)
    {
       isHovered = Bounds.Contains(mouse.X, mouse.Y);

       if(isHovered && mouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed)
        {
            OnClick?.Invoke();
        }
    }


    public void Draw(SpriteBatch _spriteBatch, Texture2D pixel, SpriteFont fontBase)
    {
        Color color = isHovered ? Color.LightYellow : Color.Yellow;

        _spriteBatch.Draw(pixel, Bounds, color);

        Vector2 textSize = fontBase.MeasureString(text);
        Vector2 textPos = new Vector2(
            Bounds.X + (Bounds.Width - textSize.X) / 2f,
            Bounds.Y + (Bounds.Height - textSize.Y) / 2f
        );
        _spriteBatch.DrawString(fontBase, text, textPos, Color.Black);
    }
}