﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookWrapper.ObjectModel;

namespace C16_Ex03_Michael_305597478_Shai_300518495
{
    public interface ITaskFactory
    {
        LikerByNums CreateLikerByNums(bool i_IsUnlike, Post i_HandledPost, int i_NumOfRequiredLikes, int i_NumOfRequiredComments, bool i_isAndOperation);

        LikerByTime CreateLikerByTime(bool i_IsUnlike, Post i_HandledPost, DateTime i_ChosenDateTime);

        CommenterByNums CreateCommenterByNums(string i_CommentText, Post i_HandledPost, int i_NumOfRequiredLikes, int i_NumOfRequiredComments, bool i_IsAndOperation);

        CommenterByTime CreateCommenterByTime(string i_CommentText, Post i_HandledPost, DateTime i_ChosenDateTime);

        Poster CreatePoster(Action<string> i_PosterInvoker, DateTime i_ChosenDateTime, string i_TextToPost);
    }
}
