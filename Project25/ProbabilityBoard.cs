using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project25
{
    class ProbabilityBoard
    {
        BaseGame BaseGame;

        string WhiteSquareName = "WhiteSquare";
        Texture2D WhiteSquareTexture
        {
            get { return BaseGame.GetTexture(WhiteSquareName); }
        }

        string FontName = "Font16";
        SpriteFont Font
        {
            get { return BaseGame.GetFont(FontName); }
        }

        public int[] ProbabilityDensity;

        public int NextShot;

        int BoardWidth;
        int BoardHeight;
        int BoardSize
        {
            get { return BoardWidth * BoardHeight; }
        }

        int SquareSize;
        int LocationX;
        int LocationY;

        int LetterGap = 5;

        public ProbabilityBoard(BaseGame baseGame, int[] probabilityDensity, int locationX, int locationY,
                                int boardWidth, int boardHeight, int squareSize)
        {
            BaseGame = baseGame;

            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            SquareSize = squareSize;

            if (probabilityDensity != null)
            {
                ProbabilityDensity = probabilityDensity;
            }
            else
            {
                ProbabilityDensity = new int[BoardSize];
            }

            LocationX = locationX;
            LocationY = locationY;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                Tuple<int, int> RowColumn = ToRowColumn(i);
                int Row = RowColumn.Item1;
                int Column = RowColumn.Item2;
                int X = LocationX + (Column * SquareSize);
                int Y = LocationY + (Row * SquareSize);
                Rectangle Rectangle = new Rectangle(X, Y, SquareSize, SquareSize);

                spriteBatch.Draw(WhiteSquareTexture, Rectangle, GetColour(i));
            }

            int LetterLocationY = LocationY + (BoardHeight * SquareSize) + LetterGap;
            int LetterLocationX = LocationX;
            char[] Letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N' };

            for (int i = 0; i < BoardWidth; i++)
            {
                char Letter = Letters[i];
                int X = LetterLocationX + (SquareSize - (int)Font.MeasureString(Letter.ToString()).X) / 2;

                Vector2 LetterLocation = new Vector2(X, LetterLocationY);
                spriteBatch.DrawString(Font, Letter.ToString(), LetterLocation, Color.White);

                LetterLocationX += SquareSize;
            }

            for (int i = 1; i <= BoardHeight; i++)
            {
                Vector2 Measure = Font.MeasureString(i.ToString());
                int X = LocationX - LetterGap - (int)Measure.X;
                int Y = LocationY + (SquareSize * (i - 1)) + (SquareSize - (int)Measure.Y) / 2;
                Vector2 NumberLocation = new Vector2(X, Y);

                spriteBatch.DrawString(Font, i.ToString(), NumberLocation, Color.White);
            }
        }

        private Tuple<int, int> ToRowColumn(int square)
        {
            int Column = square % BoardWidth;
            int Row = (square - Column) / BoardWidth;

            return new Tuple<int, int>(Row, Column);
        }

        private Color GetColour(int square)
        {
            if (ProbabilityDensity[square] == 0)
            {
                return Color.Gray * 0f;
            }
            if (square == NextShot)
            {
                return Color.Yellow;
            }
            float Weight = ProbabilityDensity[square] / 35f;
            int Blue = (int)Math.Clamp(255 * (1 - Weight), 0, 255);
            int Red = (int)Math.Clamp(255 * Weight, 0, 255);
            return (new Color(Red, 0, Blue)) * 0.87f;
        }

        public void UpdateNextShot()
        {
            NextShot = ProbabilityCalculater.PickMax(ProbabilityDensity);
        }
    }
}
