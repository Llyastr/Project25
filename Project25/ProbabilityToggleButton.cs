using System;
using System.Collections.Generic;
using System.Text;

namespace Project25
{
    class ProbabilityToggleButton : Button
    {
        BoardType2 BoardType2;

        public bool IsShow;

        public override string Text
        {
            get
            {
                if (IsShow)
                {
                    return Text2;
                }
                else
                {
                    return Text1;
                }
            }
        }

        string Text2 = "Hide";

        public ProbabilityToggleButton(BaseGame baseGame, BoardType2 boardType2, 
                                       int locationX, int locationY, int width, int height)
            : base(baseGame, "Show", "WhiteButton", "Font16",
                   locationX, locationY, width, height)
        {
            BoardType2 = boardType2;
            IsShow = false;
        }

        public override void OnClick()
        {
            if (!IsShow)
            {
                BoardType2.UpdateProbabilityDensity();
            }
            IsShow = !IsShow;
        }
    }
}
