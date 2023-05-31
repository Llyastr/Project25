using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Project25
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Dictionary<string, Texture2D> Textures;

        public Dictionary<string, SpriteFont> Fonts;

        KeyboardState KeyboardState;
        KeyboardState PreviousKeyboardState;
        MouseState MouseState;
        MouseState PreviousMouseState;

        BaseGame BaseGame;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Textures = new Dictionary<string, Texture2D>()
            {
                { "WhiteSquare", Content.Load<Texture2D>("WhiteSquare") },
                { "WhiteButton", Content.Load<Texture2D>("WhiteButton") },
                { "WhitePixel", Content.Load<Texture2D>("WhitePixel10") },
            };

            Fonts = new Dictionary<string, SpriteFont>()
            {
                { "Font16", Content.Load<SpriteFont>("galleryFont") },
            };

            BaseGame = new BaseGame(this);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            HandleInput();

            BaseGame.Update(gameTime, KeyboardState, PreviousKeyboardState, MouseState, PreviousMouseState);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            BaseGame.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void HandleInput()
        {
            PreviousKeyboardState = KeyboardState;
            PreviousMouseState = MouseState;
            KeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
        }

        public Tuple<int, int> GetScreenSize()
        {
            int Width = _graphics.PreferredBackBufferWidth;
            int Height = _graphics.PreferredBackBufferHeight;
            return new Tuple<int, int>(Width, Height);
        }
    }
}
