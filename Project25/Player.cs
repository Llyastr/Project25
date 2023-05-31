using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project25
{
    class Player
    {
        BaseGame BaseGame;

        BoardType1 BoardType1;
        BoardType2 BoardType2;

        ToggleBoardButton ToggleButton;
        bool IsToggled
        {
            get { return ToggleButton.IsToggled; }
        }

        public bool IsDonePlacing
        {
            get { return BoardType1.IsDonePlacing; }
        }

        bool IsPlacingPhase
        {
            get { return BaseGame.IsPlacingPhase; }
        }

        int TotalSize
        {
            get { return BoardType1.TotalSize; }
        }

        public bool IsWin
        {
            get
            {
                int Hits = BoardType2.GetHits().Length;
                return Hits >= TotalSize;
            }
        }

        int LocationX;
        int LocationY;

        int BoardOffsetX = 20;
        int BoardOffsetY = 70;

        int ToggleButtonOffsetX = 72;
        int ToggleButtonOffsetY = 0;
        int ToggleButtonWidth = 200;
        int ToggleButtonHeight = 40;

        public Player(BaseGame baseGame, int locationX, int locationY)
        {
            BaseGame = baseGame;

            LocationX = locationX;
            LocationY = locationY;

            int BoardLocationX = locationX + BoardOffsetX;
            int BoardLocationY = locationY + BoardOffsetY;

            BoardType1 = new BoardType1(baseGame, BoardLocationX, BoardLocationY);
            BoardType2 = new BoardType2(baseGame, BoardLocationX, BoardLocationY);

            int ToggleButtonLocationX = locationX + ToggleButtonOffsetX;
            int ToggleButtonLocationY = locationY + ToggleButtonOffsetY;

            ToggleButton = new ToggleBoardButton(baseGame, ToggleButtonLocationX, ToggleButtonLocationY,
                                                 ToggleButtonWidth, ToggleButtonHeight);
        }

        public void Update(GameTime gameTime,
                           KeyboardState keyboardState,
                           KeyboardState previousKeyboardState,
                           MouseState mouseState,
                           MouseState previousMouseState)
        {
            if (!IsToggled)
            {
                BoardType1.Update(gameTime, keyboardState, previousKeyboardState, mouseState, previousMouseState);
            }
            else
            {
                BoardType2.Update(gameTime, keyboardState, previousKeyboardState, mouseState, previousMouseState);
            }

            if (!IsPlacingPhase)
            {
                if (mouseState.LeftButton == ButtonState.Pressed &&
                    previousMouseState.LeftButton == ButtonState.Released &&
                    BoardType2.HoveredSquare != -1 &&
                    !BoardType2.IsAlreadyShot())
                {
                    BoardType2.ShootAt(BoardType2.HoveredSquare, BaseGame.NotActivePlayer.BoardType1);
                }
                else if (keyboardState.IsKeyDown(Keys.B) && previousKeyboardState.IsKeyUp(Keys.B))
                {
                    BoardType2.ShootAt(BoardType2.ProbabilityBoard.NextShot, BaseGame.NotActivePlayer.BoardType1);
                }
            }

            ToggleButton.Update(gameTime, keyboardState, previousKeyboardState, mouseState, previousMouseState);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsToggled)
            {
                BoardType1.Draw(spriteBatch);
            }
            else
            {
                BoardType2.Draw(spriteBatch);
            }

            ToggleButton.Draw(spriteBatch);
        }

        public void Reset()
        {
            BoardType1.Reset();
            BoardType2.Reset();

            ToggleButton.IsToggled = false;
        }

        public void HideBoard()
        {
            BoardType1.HideBoard();
        }

        public void ToggleBoard()
        {
            ToggleButton.OnClick();
        }
    }
}
