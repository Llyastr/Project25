using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project25
{
    class TextBox
    {
        BaseGame BaseGame;

        string FontName = "Font16";
        SpriteFont Font
        {
            get { return BaseGame.GetFont(FontName); }
        }
        string BoxTextureName = "WhiteSquare";
        Texture2D WhiteSquareTexture
        {
            get { return BaseGame.GetTexture(BoxTextureName); }
        }

        int LocationX;
        int LocationY;
        int Width;
        int Height;

        int Gap = 5;

        int WrapWidth
        {
            get { return Width - 2 * Gap; }
        }

        Vector2 TextLocation
        {
            get { return new Vector2(LocationX + Gap, LocationY + Gap); }
        }
        Rectangle Rectangle
        {
            get { return new Rectangle(LocationX, LocationY, Width, Height); }
        }

        Color TextColour = Color.LawnGreen;

        public TextBox(BaseGame baseGame, int locationX, int locationY, int width, int height)
        {
            BaseGame = baseGame;

            LocationX = locationX;
            LocationY = locationY;
            Width = width;
            Height = height;
        }

        public void Update(GameTime gameTime,
                           KeyboardState keyboardState,
                           KeyboardState previousKeyboardState,
                           MouseState mouseState,
                           MouseState previousMouseState)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch, string text)
        {
            spriteBatch.Draw(WhiteSquareTexture, Rectangle, Color.Black);
            spriteBatch.DrawString(Font, WrapText(text), TextLocation, TextColour);
        }

        private string WrapText(string text)
        {
            int MaxLineLength = WrapWidth;
            string WrappedText = "";
            string CurrentWord = "";
            void DoStuff()
            {
                if (Font.MeasureString(WrappedText + CurrentWord).X > MaxLineLength)
                {
                    WrappedText = WrappedText + "\n" + CurrentWord;
                }
                else
                {
                    WrappedText += CurrentWord;
                }
            }
            foreach (char letter in text)
            {
                CurrentWord += letter;
                if (letter == ' ')
                {
                    DoStuff();
                    CurrentWord = "";
                }
            }
            DoStuff();
            return WrappedText;
        }
    }
}
