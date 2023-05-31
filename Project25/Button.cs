using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project25
{
    class Button
    {
        public BaseGame BaseGame;

        Texture2D ButtonTexture;

        string FontName;
        SpriteFont Font
        {
            get { return BaseGame.GetFont(FontName); }
        }

        protected bool IsHovered;

        int LocationX;
        int LocationY;
        int Width;
        int Height;

        float TextX
        {
            get { return (LocationX + Width / 2) - (Font.MeasureString(Text.ToString()).X / 2); }
        }
        float TextY
        {
            get { return (LocationY + Height / 2) - (Font.MeasureString(Text.ToString()).Y / 2); }
        }
        Vector2 TextPosition
        {
            get { return new Vector2(TextX, TextY); }
        }

        protected Color HoverColour;
        protected Color UnhoverColour;
        protected virtual Color CurrentColour
        {
            get
            {
                if (IsHovered)
                {
                    return HoverColour;
                }
                else
                {
                    return UnhoverColour;
                }
            }
        }

        protected Color TextHoverColour;
        protected Color TextUnhoverColour;
        protected virtual Color TextColour
        {
            get
            {
                if (IsHovered)
                {
                    return TextHoverColour;
                }
                else
                {
                    return TextUnhoverColour;
                }
            }
        }

        public virtual string Text
        {
            get { return Text1; }
        }
        protected string Text1;

        public Rectangle Rectangle
        {
            get { return new Rectangle(LocationX, LocationY, Width, Height); }
        }

        public Button(BaseGame baseGame, string text, string buttonTextureName, string fontName,
                      int locationX, int locationY, int width, int height)
        {
            BaseGame = baseGame;

            Text1 = text;

            ButtonTexture = BaseGame.GetTexture(buttonTextureName);
            FontName = fontName;

            LocationX = locationX;
            LocationY = locationY;
            Width = width;
            Height = height;

            HoverColour = Color.White;
            UnhoverColour = Color.Black;

            TextHoverColour = Color.PaleGreen;
            TextUnhoverColour = Color.PaleGreen;
        }

        public Button(BaseGame baseGame, string text, string buttonTextureName, string fontName,
                      int locationX, int locationY, int width, int height,
                      Color hoverColour, Color unhoverColour, Color textHoverColour, Color textUnhoverColour)
        {
            BaseGame = baseGame;

            Text1 = text;

            ButtonTexture = BaseGame.GetTexture(buttonTextureName);
            FontName = fontName;

            LocationX = locationX;
            LocationY = locationY;
            Width = width;
            Height = height;

            HoverColour = hoverColour;
            UnhoverColour = unhoverColour;

            TextHoverColour = textHoverColour;
            TextUnhoverColour = textUnhoverColour;
        }

        public virtual void Update(GameTime gameTime,
                                   KeyboardState keyboardState,
                                   KeyboardState previousKeyboardState,
                                   MouseState mouseState,
                                   MouseState previousMouseState)
        {
            Point MousePoint = new Point(mouseState.X, mouseState.Y);

            if (this.Rectangle.Contains(MousePoint))
            {
                IsHovered = true;
                if (mouseState.LeftButton == ButtonState.Released &&
                    previousMouseState.LeftButton == ButtonState.Pressed)
                {
                    OnClick();
                }
            }
            else
            {
                IsHovered = false;
            }
        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ButtonTexture, Rectangle, CurrentColour);

            if (Text != null)
            {
                spriteBatch.DrawString(Font, Text, TextPosition, TextColour);
            }
        }

        public virtual void OnClick()
        {
            throw new Exception("OnClick for button not yet set");
        }
    }
}
