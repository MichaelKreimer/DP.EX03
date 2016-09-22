using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C16_Ex03_Michael_305597478_Shai_300518495
{
    public class PostScoreCalculatorCreator
    {
        public static IPostScoreCalculator CreatePostScoreCalculator(BestPostFinder.eScoreCalculationStyle i_CalculationStyle)
        {
            if (i_CalculationStyle.Equals(BestPostFinder.eScoreCalculationStyle.Comments))
            {
                return new PostScoreCalculatorByComments();
            }
            else if (i_CalculationStyle.Equals(BestPostFinder.eScoreCalculationStyle.Formula))
            {
                return new PostScoreCalculatorByFormula();
            }
            else
            {
                return new PostScoreCalculatorByLikes();
            }
        }
    }
}
