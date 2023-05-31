using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project25
{
    class BackgroundRender
    {
        BaseGame BaseGame;

        string TextureName = "WhitePixel";
        Texture2D Texture
        {
            get { return BaseGame.GetTexture(TextureName); }
        }

        public bool Enabled;

        int ScreenWidth;
        int ScreenHeight;
        int PixelSize = 5;
        int Width
        {
            get { return ScreenWidth / PixelSize; }
        }
        int Height
        {
            get { return ScreenHeight / PixelSize; }
        }
        int ScreenSize
        {
            get { return Width * Height; }
        }


        List<Tuple<int, int, float, float>> WavePoints;

        int[] BaseImage;

        int BaseValue = 100;

        public BackgroundRender(BaseGame baseGame, int width, int height)
        {
            BaseGame = baseGame;
            ScreenWidth = width;
            ScreenHeight = height;
            Enabled = true;

            WavePoints = new List<Tuple<int, int, float, float>>()
            {
                new Tuple<int, int, float, float>(600, 200, 400f, 10f)
            };

            BaseImage = new int[ScreenSize];
            for (int i = 0; i < BaseImage.Length; i++)
            {
                BaseImage[i] = BaseValue;
            }
        }



        public virtual void Update(GameTime gameTime,
                                   KeyboardState keyboardState,
                                   KeyboardState previousKeyboardState,
                                   MouseState mouseState,
                                   MouseState previousMouseState)
        {

        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            int[] Image = GetImage();
            for (int i = 0; i < Image.Length; i++)
            {
                int Column = i % Width;
                int Row = (i - Column) / Width;
                int Value = Image[i];
                Rectangle Rectangle = new Rectangle(Column * PixelSize, Row * PixelSize, PixelSize, PixelSize);
                spriteBatch.Draw(Texture, Rectangle, GetColour(i));
            }
        }

        private int[] GetImage()
        {
            int[] Image = BaseImage;
            for (int i = 0; i < Image.Length; i++)
            {
                int Column = i % Width;
                int Row = (i - Column) / Width;
                double Distance = Math.Sqrt((Column * Column) + (Row * Row));
                int Offset = 40;
                if (Distance < 400)
                {
                    Image[i] += Offset;
                }
            }

            return Image;
        }

        public Color GetColour(int i)
        {
            return new Color(0, 0, i % 256);
        }

        public void ToggleEnabled()
        {
            Enabled = !Enabled;
        }
    }
}
