using System;
using System.Collections.Generic;
using System.Text;

namespace Project25
{
    class HideButton : Button
    {
        public bool IsHidden;

        public override string Text
        {
            get
            {
                if (IsHidden)
                {
                    return Text2;
                }
                else
                {
                    return Text1;
                }
            }
        }

        string Text2 = "Unhide";

        public HideButton(BaseGame baseGame, int locationX, int locationY, int width, int height)
            : base(baseGame, "Hide", "WhiteButton", "Font16",
                   locationX, locationY, width, height)
        {
            IsHidden = false;
        }

        public override void OnClick()
        {
            IsHidden = !IsHidden;
        }
    }
}
