using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C16_Ex03_Michael_305597478_Shai_300518495
{
    public class PostScoreCalculatorByComments : IPostScoreCalculator
    {
        public int GetScore(int i_Likes, int i_Comments)
        {
            return i_Comments;
        }
    }
}
