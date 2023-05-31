using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project25
{
    class BoardType2
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

        int LetterGap = 5;

        int[] State;

        public int HoveredSquare;

        public List<int[]> SunkenShips;

        public ProbabilityBoard ProbabilityBoard;

        ProbabilityToggleButton ProbabilityToggleButton;
        bool ShowProbablity
        {
            get { return ProbabilityToggleButton.IsShow; }
        }

        int[] ShipSizes = { 2, 3, 3, 4, 5 };

        public BoardType2(BaseGame baseGame, int locationX, int locationY)
        {
            BaseGame = baseGame;

            LocationX = locationX;
            LocationY = locationY;

            SunkenShips = new List<int[]>();

            ProbabilityBoard = new ProbabilityBoard(baseGame, null, locationX, locationY, 
                                                    BoardWidth, BoardHeight, SquareSize);

            int ButtonY = LocationY + (BoardHeight * SquareSize) - ((BoardHeight * SquareSize) + ButtonHeight) / 2;
            int ButtonX = LocationX + (BoardWidth * SquareSize) + ButtonGap;
            ProbabilityToggleButton = new ProbabilityToggleButton(baseGame, this, ButtonX, ButtonY, 
                                                                  ButtonWidth, ButtonHeight);
            

            Reset();
        }

        public void Update(GameTime gameTime,
                           KeyboardState keyboardState,
                           KeyboardState previousKeyboardState,
                           MouseState mouseState,
                           MouseState previousMouseState)
        {
            Point MousePoint = new Point(mouseState.X, mouseState.Y);
            int[] Indexes = new int[BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                Indexes[i] = i;
            }

            foreach (int i in Indexes)
            {
                Tuple<int, int> RowColumn = ToRowColumn(i);
                int Row = RowColumn.Item1;
                int Column = RowColumn.Item2;
                int X = LocationX + (Column * SquareSize);
                int Y = LocationY + (Row * SquareSize);
                Rectangle Rectangle = new Rectangle(X, Y, SquareSize, SquareSize);

                if (Rectangle.Contains(MousePoint))
                {
                    HoveredSquare = i;
                    break;
                }
                HoveredSquare = -1;
            }

            ProbabilityToggleButton.Update(gameTime, keyboardState, previousKeyboardState, mouseState, previousMouseState);
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

                Color Colour = GetColour(State[i]);

                spriteBatch.Draw(WhiteSquareTexture, Rectangle, Colour);
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

            if (ShowProbablity)
            {
                ProbabilityBoard.Draw(spriteBatch);
            }

            ProbabilityToggleButton.Draw(spriteBatch); 
            
            DrawHoveredSquare(spriteBatch);
        }

        private void DrawHoveredSquare(SpriteBatch spriteBatch)
        {
            if (HoveredSquare == -1 || IsAlreadyShot())
            {
                return;
            }

            Tuple<int, int> RowColumn = ToRowColumn(HoveredSquare);
            int Row = RowColumn.Item1;
            int Column = RowColumn.Item2;
            int X = LocationX + (Column * SquareSize);
            int Y = LocationY + (Row * SquareSize);
            Rectangle Rectangle = new Rectangle(X, Y, SquareSize, SquareSize);

            spriteBatch.Draw(WhiteSquareTexture, Rectangle, Color.Orange);
        }

        public void ShootAt(int square, BoardType1 boardType1)
        {
            int[] OccupiedSquares = boardType1.GetOccupiedSquares();
            foreach (int i in OccupiedSquares)
            {
                if (i == square)
                {
                    if (WillSink(square, boardType1))
                    {
                        State[square] = 2;
                        BaseGame.TextIndex = 4;
                        BaseGame.ToggleActivePlayer();
                        UpdateProbabilityDensity();
                        return;
                    }
                    else
                    {
                        State[square] = 2;
                        BaseGame.TextIndex = 3;
                        UpdateProbabilityDensity();
                        return;
                    }
                }
            }
            State[square] = 1;
            BaseGame.TextIndex = 2;
            UpdateProbabilityDensity();
            BaseGame.ToggleActivePlayer();
        }

        private bool WillSink(int square, BoardType1 boardType1)
        {
            int[] Hits = GetHits();
            foreach (Ship ship in boardType1.ActiveShips)
            {
                List<int> TempInts = new List<int>();
                foreach (int i in ship.Squares)
                {
                    TempInts.Add(i);
                }
                int[] ShipSize = TempInts.ToArray();
                foreach (int hit in Hits)
                {
                    TempInts.Remove(hit);
                }
                if (TempInts.Count == 1 && TempInts[0] == square)
                {
                    SunkenShips.Add(ShipSize);
                    return true;
                }
            }
            return false;
        }

        public bool IsAlreadyShot()
        {
            return State[HoveredSquare] != 0;
        }

        private Tuple<int, int> ToRowColumn(int square)
        {
            int Column = square % BoardWidth;
            int Row = (square - Column) / BoardWidth;

            return new Tuple<int, int>(Row, Column);
        }

        private Color GetColour(int i)
        {
            switch (i)
            {
                case 1:
                    return Color.Gray;
                case 2:
                    return Color.Red;
            }
            return Color.White;
        }
        
        public void Reset()
        {
            State = new int[BoardSize];

            SunkenShips = new List<int[]>();

            ProbabilityBoard = new ProbabilityBoard(BaseGame, null, LocationX, LocationY,
                                                    BoardWidth, BoardHeight, SquareSize);
            UpdateProbabilityDensity();
            ProbabilityToggleButton.IsShow = true;

            HoveredSquare = -1;
        }

        public int[] GetHits()
        {
            List<int> TempHits = new List<int>();
            for (int i = 0; i < BoardSize; i++)
            {
                if (State[i] == 2)
                {
                    TempHits.Add(i);
                }
            }
            return TempHits.ToArray();
        }

        public void UpdateProbabilityDensity()
        {
            ProbabilityCalculater ProbabilityCalculater = new ProbabilityCalculater();
            int[] NewDensity = ProbabilityCalculater.CalculateProbabilityDensity(ShipSizes, SunkenShips, State);
            ProbabilityBoard.ProbabilityDensity = NewDensity;
            ProbabilityBoard.UpdateNextShot();
        }
    }
}
