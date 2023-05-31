using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project25
{
    class Ship
    {
        BaseGame BaseGame;

        string WhiteSquareName = "WhiteSquare";
        Texture2D WhiteSquareTexture
        {
            get { return BaseGame.GetTexture(WhiteSquareName); }
        }

        public bool IsHovered;

        bool IsHorizontal
        {
            get
            {
                if (Squares == null || Squares.Length <= 0)
                {
                    return true;
                }
                if (Math.Abs(Squares[0] - Squares[1]) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        int LocationX;
        int LocationY;

        int SquareSize;

        int BoardWidth;
        int BoardHeight;

        public int[] Squares;

        public int Size
        {
            get { return Squares.Length; }
        }

        public Ship(BaseGame baseGame, int locationX, int locationY, int squareSize,
                    int boardWidth, int boardHeight, int shipSize)
        {
            BaseGame = baseGame;

            LocationX = locationX;
            LocationY = locationY;

            SquareSize = squareSize;

            BoardWidth = boardWidth;
            BoardHeight = boardHeight;

            List<int> TempSquares = new List<int>();
            for (int i = 0; i < shipSize; i++)
            {
                TempSquares.Add(i * 10);
            }
            Squares = TempSquares.ToArray();
        }

        public void Update(GameTime gameTime,
                           KeyboardState keyboardState,
                           KeyboardState previousKeyboardState,
                           MouseState mouseState,
                           MouseState previousMouseState)
        {
            Point MousePoint = new Point(mouseState.X, mouseState.Y);
            foreach (int i in Squares)
            {
                Tuple<int, int> RowColumn = ToRowColumn(i);
                int Row = RowColumn.Item1;
                int Column = RowColumn.Item2;
                int X = LocationX + (Column * SquareSize);
                int Y = LocationY + (Row * SquareSize);
                Rectangle Rectangle = new Rectangle(X, Y, SquareSize, SquareSize);

                if (Rectangle.Contains(MousePoint))
                {
                    IsHovered = true;
                    return;
                }
            }
            IsHovered = false;
        }

        public void Draw(SpriteBatch spriteBatch, Color shipColour)
        {
            foreach (int i in Squares)
            {
                Tuple<int, int> RowColumn = ToRowColumn(i);
                int Row = RowColumn.Item1;
                int Column = RowColumn.Item2;
                int X = LocationX + (Column * SquareSize);
                int Y = LocationY + (Row * SquareSize);
                Rectangle Rectangle = new Rectangle(X, Y, SquareSize, SquareSize);

                spriteBatch.Draw(WhiteSquareTexture, Rectangle, shipColour);
            }
        }

        public void SetSquares(params int[] squares)
        {
            Squares = squares;
        }

        public void SetLocation(int locationX, int locationY)
        {
            LocationX = locationX;
            LocationY = locationY;
        }

        private Tuple<int, int> ToRowColumn(int square)
        {
            int Column = square % BoardWidth;
            int Row = (square - Column) / BoardWidth;

            return new Tuple<int, int>(Row, Column);
        }

        public bool IsOnTop()
        {
            foreach (int i in Squares)
            {
                Tuple<int, int> RC = ToRowColumn(i);
                if (RC.Item1 == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsOnBottom()
        {
            foreach (int i in Squares)
            {
                Tuple<int, int> RC = ToRowColumn(i);
                if (RC.Item1 == BoardHeight - 1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsOnLeft()
        {
            foreach (int i in Squares)
            {
                Tuple<int, int> RC = ToRowColumn(i);
                if (RC.Item2 == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsOnRight()
        {
            foreach (int i in Squares)
            {
                Tuple<int, int> RC = ToRowColumn(i);
                if (RC.Item2 == BoardWidth - 1)
                {
                    return true;
                }
            }
            return false;
        }

        public void DoRotate()
        {
            if (IsHorizontal)
            {
                for (int i = 0; i < Squares.Length; i++)
                {
                    Squares[i] = i * BoardWidth;
                }
            }
            else
            {
                for (int i = 0; i < Squares.Length; i++)
                {
                    Squares[i] = i;
                }
            }
        }

        public void DoMove(int offset)
        {
            bool CheckSide(Func<bool> borderCheck, int offsetCheck)
            {
                if (offsetCheck == offset && borderCheck())
                {
                    return true;
                }
                return false;
            }

            if (CheckSide(IsOnTop, -BoardWidth) ||
                CheckSide(IsOnBottom, BoardWidth) ||
                CheckSide(IsOnLeft, -1) ||
                CheckSide(IsOnRight, 1))
            {
                return;
            }

            for (int i = 0; i < Squares.Length; i++)
            {
                Squares[i] += offset;
            }
        }

        public bool IsValidPosition(List<Ship> ships)
        {
            int[] OccupiedSquares = GetOccupiedSquares(ships);

            foreach (int i in Squares)
            {
                foreach (int n in OccupiedSquares)
                {
                    if (i == n)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private int[] GetOccupiedSquares(List<Ship> ships)
        {
            List<int> TempSquares = new List<int>();
            foreach (Ship ship in ships)
            {
                foreach (int square in ship.Squares)
                {
                    TempSquares.Add(square);
                }
            }
            return TempSquares.ToArray();
        }
    }
}
