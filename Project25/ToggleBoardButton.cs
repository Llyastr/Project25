using System;
using System.Collections.Generic;
using System.Text;

namespace Project25
{
    class ToggleBoardButton : Button
    {
        public bool IsToggled;

        public override string Text
        {
            get
            {
                if (IsToggled)
                {
                    return Text2;
                }
                else
                {
                    return Text1;
                }
            }
        }

        string Text2 = "To Ships Board";

        public ToggleBoardButton(BaseGame baseGame, int locationX, int locationY, int width, int height)
            : base(baseGame, " To Shooting Board", "WhiteButton", "Font16",
                   locationX, locationY, width, height)
        {
            IsToggled = false;
        }

        public override void OnClick()
        {
            IsToggled = !IsToggled;
        }
    }
}
