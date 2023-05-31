using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project25
{
    class BaseGame
    {
        Game1 OperatingGame;

        BackgroundRender BackgroundRender;

        Player Player1;
        Player Player2;

        bool IsActivePlayer;
        Player ActivePlayer
        {
            get
            {
                if (IsActivePlayer)
                {
                    return Player1;
                }
                else
                {
                    return Player2;
                }
            }
        }
        public Player NotActivePlayer
        {
            get
            {
                if (!IsActivePlayer)
                {
                    return Player1;
                }
                else
                {
                    return Player2;
                }
            }
        }
        int ActivePlayerInt
        {
            get
            {
                if (IsActivePlayer)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
        }

        public bool IsPlacingPhase;

        Dictionary<string, ClickButton> ClickButtons;

        TextBox TextBox;

        public int TextIndex;
        Dictionary<Tuple<int, int>, string> Texts;

        string TextBoxText
        {
            get { return Texts[new Tuple<int, int>(TextIndex, ActivePlayerInt)]; }
        }

        bool IsGameOver
        {
            get { return TextIndex == 5; }
        }

        public Dictionary<string, Texture2D> Textures
        {
            get { return OperatingGame.Textures; }
        }

        public Dictionary<string, SpriteFont> Fonts
        {
            get { return OperatingGame.Fonts; }
        }

        public BaseGame(Game1 operatingGame)
        {
            OperatingGame = operatingGame;

            Tuple<int, int> ScreenDimensions = operatingGame.GetScreenSize();
            BackgroundRender = new BackgroundRender(this, ScreenDimensions.Item1, ScreenDimensions.Item2);

            IsActivePlayer = true;

            Player1 = new Player(this, 100, 100);
            Player2 = new Player(this, 770, 100);

            ClickButtons = new Dictionary<string, ClickButton>()
            {
                { "Reset", new ClickButton(this, "Reset Game", 540, 300, 200, 30) },
            };

            TextBox = new TextBox(this, 540, 400, 200, 100);

            TextIndex = 0;

            Texts = new Dictionary<Tuple<int, int>, string>()
            {
                { new Tuple<int, int>(0, 1), "Player1 is now placing ships" },
                { new Tuple<int, int>(1, 1), "It is Player1's turn to shoot" },
                { new Tuple<int, int>(2, 1), "Player2 missed. It is now Player1's turn" },
                { new Tuple<int, int>(3, 1), "Player1 hit. Player1 can shoot again" },
                { new Tuple<int, int>(4, 1), "Player2 sunk a ship. It is now Player1's turn" },
                { new Tuple<int, int>(5, 1), "Player2 won. Press reset to play again" },
                { new Tuple<int, int>(0, 2), "Player2 is now placing ships" },
                { new Tuple<int, int>(1, 2), "It is Player2's turn to shoot" },
                { new Tuple<int, int>(2, 2), "Player1 missed. It is now Player2's turn" },
                { new Tuple<int, int>(3, 2), "Player2 hit. Player2 can shoot again" },
                { new Tuple<int, int>(4, 2), "Player1 sunk a ship. It is now Player2's turn" },
                { new Tuple<int, int>(5, 2), "Player1 won. Press reset to play again" },
            };

            IsPlacingPhase = true;
        }

        public void Update(GameTime gameTime,
                           KeyboardState keyboardState,
                           KeyboardState previousKeyboardState,
                           MouseState mouseState,
                           MouseState previousMouseState)
        {
            BackgroundRender.Update(gameTime, keyboardState, previousKeyboardState, mouseState, previousMouseState);
            foreach (KeyValuePair<string, ClickButton> kvp in ClickButtons)
            {
                ClickButton ClickButton = kvp.Value;
                ClickButton.Update(gameTime, keyboardState, previousKeyboardState, mouseState, previousMouseState);
            }
            if ((keyboardState.IsKeyDown(Keys.LeftShift) || previousKeyboardState.IsKeyDown(Keys.RightShift)) &&
                keyboardState.IsKeyDown(Keys.E) && previousKeyboardState.IsKeyUp(Keys.E))
            {
                ClickButtons["Reset"].IsClicked = true;
            }
            if (IsButtonClicked("Reset"))
            {
                Reset();
            }
            

            if (!IsGameOver)
            {
                if (IsPlacingPhase)
                {
                    if (ActivePlayer.IsDonePlacing)
                    {
                        ActivePlayer.HideBoard();
                        ToggleActivePlayer();
                    }
                    if (Player1.IsDonePlacing && Player2.IsDonePlacing)
                    {
                        Player1.HideBoard();
                        Player2.HideBoard();
                        Player1.ToggleBoard();
                        Player2.ToggleBoard();
                        IsPlacingPhase = false;
                        TextIndex = 1;
                    }
                }

                ActivePlayer.Update(gameTime, keyboardState, previousKeyboardState, mouseState, previousMouseState);

                if (NotActivePlayer.IsWin)
                {
                    TextIndex = 5;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            BackgroundRender.Draw(spriteBatch);
            foreach (KeyValuePair<string, ClickButton> kvp in ClickButtons)
            {
                ClickButton ClickButton = kvp.Value;
                ClickButton.Draw(spriteBatch);
            }

            Player1.Draw(spriteBatch);
            Player2.Draw(spriteBatch);

            TextBox.Draw(spriteBatch, TextBoxText);
        }

        public Texture2D GetTexture(string textureName)
        {
            return Textures[textureName];
        }

        public SpriteFont GetFont(string fontName)
        {
            return Fonts[fontName];
        }

        private bool IsButtonClicked(string buttonName)
        {
            return ClickButtons[buttonName].IsClicked;
        }

        public void ToggleActivePlayer()
        {
            IsActivePlayer = !IsActivePlayer;
        }

        public void Reset()
        {
            Player1.Reset();
            Player2.Reset();
            IsPlacingPhase = true;

            TextIndex = 0;
        }
    }
}
