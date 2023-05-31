using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project25
{
    class ClickButton : Button
    {
        public bool IsClicked;

        public ClickButton(BaseGame baseGame, string text, int locationX, int locationY, int width, int height)
            : base(baseGame, text, "WhiteButton", "Font16",
                   locationX, locationY, width, height)
        {
            IsClicked = false;
        }

        public override void Update(GameTime gameTime, 
                                    KeyboardState keyboardState, 
                                    KeyboardState previousKeyboardState, 
                                    MouseState mouseState, 
                                    MouseState previousMouseState)
        {
            IsClicked = false;

            base.Update(gameTime, keyboardState, previousKeyboardState, mouseState, previousMouseState);
        }

        public override void OnClick()
        {
            IsClicked = true;
        }
    }
}
