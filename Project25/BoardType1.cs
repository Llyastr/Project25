using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project25
{
    class BoardType1
    {
        BaseGame BaseGame;

        EmptyBoard EmptyBoard;

        HideButton HideButton;

        bool IsHidden
        {
            get { return HideButton.IsHidden; }
        }

        public bool IsDonePlacing
        {
            get { return ShipsToBePlaced.Count == 0 && SelectedShip == null; }
        }

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

        public int[] ShipSizes = { 2, 3, 3, 4, 5 };
        public int TotalSize
        {
            get { return Enumerable.Sum(ShipSizes); }
        }

        int BoardWidth = 10;
        int BoardHeight = 10;
        int BoardSize
        {
            get { return BoardWidth * BoardHeight; }
        }

        int SquareSize = 30;
        int LocationX;
        int LocationY;

        int ButtonWidth = 70;
        int ButtonHeight = 30;
        int ButtonGap = 5;

        public List<Ship> ActiveShips;

        List<Ship> ShipsToBePlaced;

        Ship SelectedShip;

        public BoardType1(BaseGame baseGame, int locationX, int locationY)
        {
            BaseGame = baseGame;

            EmptyBoard = new EmptyBoard(baseGame, locationX, locationY, BoardWidth, BoardHeight, SquareSize);

            LocationX = locationX;
            LocationY = locationY;

            int ButtonY = LocationY + (BoardHeight * SquareSize) - ((BoardHeight * SquareSize) + ButtonHeight) / 2;
            int ButtonX = LocationX + (BoardWidth * SquareSize) + ButtonGap;
            HideButton = new HideButton(baseGame, ButtonX, ButtonY, ButtonWidth, ButtonHeight);

            Reset();
        }



        public void Update(GameTime gameTime,
                           KeyboardState keyboardState,
                           KeyboardState previousKeyboardState,
                           MouseState mouseState,
                           MouseState previousMouseState)
        {
            HideButton.Update(gameTime, keyboardState, previousKeyboardState, mouseState, previousMouseState);

            if (SelectedShip != null)
            {
                void CheckMoveKey(Keys key, int offset)
                {
                    if (keyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key))
                    {
                        SelectedShip.DoMove(offset);
                    }
                }
                CheckMoveKey(Keys.W, -BoardWidth);
                CheckMoveKey(Keys.S, BoardWidth);
                CheckMoveKey(Keys.A, -1);
                CheckMoveKey(Keys.D, 1);

                if (keyboardState.IsKeyDown(Keys.R) && previousKeyboardState.IsKeyUp(Keys.R))
                {
                    SelectedShip.DoRotate();
                }

                if (keyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter) &&
                    SelectedShip.IsValidPosition(ActiveShips))
                {
                    ActiveShips.Add(SelectedShip);
                    SelectedShip = null;
                }
            }
            else
            {
                for (int i = 0; i < ShipsToBePlaced.Count; i++)
                {
                    Ship Ship = ShipsToBePlaced[i];

                    Ship.Update(gameTime, keyboardState, previousKeyboardState, mouseState, previousMouseState);
                    if ((Ship.IsHovered && mouseState.LeftButton == ButtonState.Pressed &&
                        previousMouseState.LeftButton == ButtonState.Released && SelectedShip == null) ||
                        (keyboardState.IsKeyDown(Keys.Q) && previousKeyboardState.IsKeyUp(Keys.Q)))
                    {
                        Ship.SetLocation(LocationX, LocationY);
                        int SquareCount = 0;
                        List<int> TempSquares = new List<int>();
                        for (int n = 0; n < Ship.Size; n++)
                        {
                            TempSquares.Add(SquareCount);
                            SquareCount++;
                        }
                        Ship.SetSquares(TempSquares.ToArray());

                        SelectedShip = Ship;
                        ShipsToBePlaced.RemoveAt(i);
                        break;
                    }

                    if (keyboardState.IsKeyDown(Keys.P) && previousKeyboardState.IsKeyUp(Keys.P))
                    {
                        HideBoard();
                        RandomlyPlaceRemainingShips();
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            EmptyBoard.Draw(spriteBatch);
            HideButton.Draw(spriteBatch);

            if (!IsHidden)
            {
                int ColourIndex = 0;
                foreach (Ship ship in ActiveShips)
                {
                    DrawShip(spriteBatch, ship, IntToColour(ColourIndex));
                    ColourIndex++;
                }
            }

            foreach (Ship ship in ShipsToBePlaced)
            {
                if (ship.IsHovered && SelectedShip == null)
                {
                    ship.Draw(spriteBatch, Color.DarkSlateBlue);
                }
                else
                {
                    ship.Draw(spriteBatch, Color.Aquamarine);
                }
            }

            if (SelectedShip != null)
            {
                if (SelectedShip.IsValidPosition(ActiveShips))
                {
                    SelectedShip.Draw(spriteBatch, Color.DarkSlateBlue);
                }
                else
                {
                    SelectedShip.Draw(spriteBatch, Color.Red * 0.7f);
                }
            }
        }

        private void DrawShip(SpriteBatch spriteBatch, Ship ship, Color shipColour)
        {
            int[] Squares = ship.Squares;
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

        private Tuple<int, int> ToRowColumn(int square)
        {
            int Column = square % BoardWidth;
            int Row = (square - Column) / BoardWidth;

            return new Tuple<int, int>(Row, Column);
        }

        public int[] GetOccupiedSquares()
        {
            List<int> TempSquares = new List<int>();
            foreach (Ship ship in ActiveShips)
            {
                foreach (int square in ship.Squares)
                {
                    TempSquares.Add(square);
                }
            }
            return TempSquares.ToArray();
        }

        public void Reset()
        {
            ActiveShips = new List<Ship>();

            int ShipLocationX = LocationX;
            int ShipLocationY = LocationY + ((BoardHeight + 2) * SquareSize);
            ShipsToBePlaced = new List<Ship>();
            foreach (int shipSize in ShipSizes)
            {
                ShipsToBePlaced.Add(new Ship(BaseGame, ShipLocationX, ShipLocationY,
                                             SquareSize, BoardWidth, BoardHeight, shipSize));
                ShipLocationX += (SquareSize * 2);
            }

            HideButton.IsHidden = false;
        }

        public void HideBoard()
        {
            HideButton.IsHidden = true;
        }

        private Color IntToColour(int i)
        {
            Dictionary<int, Color> TempColours = new Dictionary<int, Color>()
            {
                { 0, Color.CadetBlue},
                { 1, Color.MediumBlue},
                { 2, Color.DodgerBlue},
                { 3, Color.Blue},
                { 4, Color.DodgerBlue},
            };
            return TempColours[i % TempColours.Count];
        }

        private void RandomlyPlaceRemainingShips()
        {
            foreach (Ship ship in ShipsToBePlaced)
            {
                RandomlyPlaceShip(ship);
            }
            ShipsToBePlaced.Clear();
        }

        private void RandomlyPlaceShip(Ship ship)
        {
            int ShipSize = ship.Size;
            int[] OccupiedSquares = GetOccupiedSquares();
            bool IsOccupied(int square)
            {
                foreach (int i in OccupiedSquares)
                {
                    if (square == i)
                    {
                        return true;
                    }
                }
                return false;
            }
            bool IsValidPosition(List<int> ship)
            {
                foreach (int i in ship)
                {
                    if (IsOccupied(i))
                    {
                        return false;
                    }
                }
                return true;
            }

            int PossiblePositionsCount = 0;
            void IncrementCount(int[] ignore)
            {
                PossiblePositionsCount++;
            }

            void LoopThroughPositions(Action<int[]> action)
            {
                int MaxColumn = BoardWidth - ShipSize;
                int OffsetHorizontal = ShipSize - 1;
                for (int i = 0; i < BoardSize; i++)
                {
                    List<int> TempInt = new List<int>();
                    for (int n = 0; n < ShipSize; n++)
                    {
                        TempInt.Add(i + n);
                    }
                    if (IsValidPosition(TempInt))
                    {
                        action(TempInt.ToArray());
                    }

                    if (i % BoardWidth == MaxColumn)
                    {
                        i += OffsetHorizontal;
                    }
                }

                int IterationsVertical = BoardHeight - ShipSize + 1;
                int MaxVertical = ((BoardHeight - ShipSize + 1) * BoardWidth) - 1;

                for (int i = 0; i <= MaxVertical; i++)
                {
                    List<int> TempInt = new List<int>();
                    for (int n = 0; n < ShipSize; n++)
                    {
                        TempInt.Add(i + (n * BoardWidth));
                    }
                    if (IsValidPosition(TempInt))
                    {
                        action(TempInt.ToArray());
                    }
                }
            }

            LoopThroughPositions(IncrementCount);

            Random Random = new Random();
            int PositionIndex = Random.Next(PossiblePositionsCount);
            int CurrentIndex = 0;

            void PlaceShip(int[] squares)
            {
                if (CurrentIndex == PositionIndex)
                {
                    ship.SetSquares(squares);
                    ship.SetLocation(LocationX, LocationY);
                    ActiveShips.Add(ship);
                }
                CurrentIndex = Math.Min(CurrentIndex + 1, PossiblePositionsCount - 1);
            }

            LoopThroughPositions(PlaceShip);
        }
    }
}
