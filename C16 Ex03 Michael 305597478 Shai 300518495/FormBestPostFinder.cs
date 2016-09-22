﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;

namespace C16_Ex03_Michael_305597478_Shai_300518495
{
    public partial class BestPostFinder : Form
    {
        private readonly object m_LockPostsSearch = new object();
        private readonly object m_LockPostsGenerate = new object();
        private PostScoreCalculatorCreator m_PostScoreCalculatorCreator = new PostScoreCalculatorCreator();

        public BestPostFinder()
        {
            InitializeComponent();
            initPostTypes();
        }

        private static bool isLegalDate(Post post, eDate eDate)
        {
            return eDate.Equals(eDate.AllTime) || post.CreatedTime >= DateTime.Now.AddDays((int)eDate * -1);
        }

        private void initPostTypes()
        {
            foreach (Post.eType etype in Enum.GetValues(typeof(Post.eType)))
            {
                checkedListBoxPostType.Items.Add(etype);
            }
        }

        public enum eScoreCalculationStyle
        {
            Likes,
            Comments,
            Formula,
        }

        private int getPostScore(Post i_PostToCheck)
        {
            IPostScoreCalculator postCalculator;
            BestPostFinder.eScoreCalculationStyle scoreCalculationStyle = getScoreCalcStyle();
            postCalculator = PostScoreCalculatorCreator.CreatePostScoreCalculator(scoreCalculationStyle);
            int likes = i_PostToCheck.LikedBy.Count;
            int comments = i_PostToCheck.Comments.Count;
            int likeValue = 1;
            int commentValue = 1;
            if (numericUpDownLikesValue.Enabled)
            {
                likeValue = (int)numericUpDownLikesValue.Value;
                commentValue = (int)numericUpDownCommentsValue.Value;
            }

            return postCalculator.GetScore(likes * likeValue, comments * commentValue);
        }

        private bool isPostTypeSelected(Post.eType i_typeToCheck)
        {
            bool res = false;
            foreach (var eType in checkedListBoxPostType.CheckedItems)
            {
                if (eType.Equals(i_typeToCheck))
                {
                    res = true;
                    break;
                }
            }

            return res;
        }

        private void findBestPosts(string i_Expression)
        {
            lock (m_LockPostsSearch)
            {
                List<PostWrapper> postsWrappers = new List<PostWrapper>();
                foreach (Post post in LoginForm.s_LoggedInUser.Posts)
                {
                    if (isPostTypeSelected(post.Type.Value))
                    {
                        eDate eDate = getDateRange();
                        if (post != null &&
                            isLegalDate(post, eDate))
                        {
                            if (i_Expression != string.Empty)
                            {
                                if (post.Message != null && post.Message.Contains(i_Expression))
                                {
                                    postsWrappers.Add(new PostWrapper(post));
                                }
                            }
                            else
                            {
                                postsWrappers.Add(new PostWrapper(post));
                            }
                        }
                    }
                }

                postsWrappers.Sort((x, y) => comparePostWrappers(x.Post, y.Post));
                postsWrappers.Reverse();
                int maxPossibleRange = Math.Min(postsWrappers.Count, (int)numericUpDownDisplayResults.Value);
                List<PostWrapper> subPostWrappers = postsWrappers.GetRange(0, maxPossibleRange);
                listBoxResult.Invoke(new Action(
                    () =>
                {
                    listBoxResult.Items.Clear();
                    foreach (PostWrapper pw in subPostWrappers)
                    {
                        listBoxResult.Items.Add(pw);
                    }
                }));
            }
        }

        private int comparePostWrappers(Post post1, Post post2)
        {
            IPostScoreCalculator scoreCalculator = PostScoreCalculatorCreator.CreatePostScoreCalculator(getScoreCalcStyle());
            return getPostScore(post1) - getPostScore(post2);
        }

        private eDate getDateRange()
        {
            if (radioButtonAllTime.Checked)
            {
                return eDate.AllTime;
            }
            else if (radioButtonLastYear.Checked)
            {
                return eDate.Year;
            }
            else if (radioButtonLastWeek.Checked)
            {
                return eDate.Week;
            }
            else
            {
                return eDate.Day;
            }
        }

        private eScoreCalculationStyle getScoreCalcStyle()
        {
            if (radioButtonCommentsStyle.Checked)
            {
                return eScoreCalculationStyle.Comments;
            }
            else if (radioButtonLikesStyle.Checked)
            {
                return eScoreCalculationStyle.Likes;
            }
            else
            {
                return eScoreCalculationStyle.Formula;
            }
        }

        private void radioButtonFormulaStyle_CheckedChanged(object sender, EventArgs e)
        {
            setFormulaGroupStatus(true, eScoreCalculationStyle.Formula);
        }

        private void radioButtonCommentsStyle_CheckedChanged(object sender, EventArgs e)
        {
            setFormulaGroupStatus(false, eScoreCalculationStyle.Comments);
        }

        private void radioButtonLikesStyle_CheckedChanged(object sender, EventArgs e)
        {
            setFormulaGroupStatus(false, eScoreCalculationStyle.Likes);
        }

        private void setFormulaGroupStatus(bool i_ShowFormula, eScoreCalculationStyle i_CalcStyle)
        {
            groupBoxFormulaInit.Enabled = numericUpDownCommentsValue.Enabled = numericUpDownLikesValue.Enabled = labelFormula.Visible = i_ShowFormula;
        }

        private void buttonGenerateResults_Click(object sender, EventArgs e)
        {
            new Thread(generateResults).Start();
        }

        private void generateResults()
        {
            if (LoginForm.s_LoggedInUser != null)
            {
                textBoxResultInfo.Invoke(new Action(() =>
              {
                  textBoxResultInfo.Text = string.Empty;
                  textBoxResultInfo.Enabled = false;
              }));
            }

            findBestPosts(textBoxToSearch.Text);
        }

        private void listBoxResult_SelectedValueChanged(object sender, EventArgs e)
        {
            displayPostInformation();
        }

        private void displayPostInformation()
        {
            PostWrapper postWrapperSelected = listBoxResult.SelectedItem as PostWrapper;
            displayPostInfo(postWrapperSelected);
            displayPostPicture(postWrapperSelected.Post);
        }

        private void displayPostPicture(Post i_PostToDisplay)
        {
            if (i_PostToDisplay.PictureURL != null)
            {
                pictureBoxResultPhoto.LoadAsync(i_PostToDisplay.PictureURL);
                pictureBoxResultPhoto.Visible = true;
            }
            else
            {
                pictureBoxResultPhoto.Visible = false;
            }
        }

        private void displayPostInfo(PostWrapper i_Pw)
        {
            Post postToDisplay = i_Pw.Post;
            textBoxResultInfo.Invoke(new Action(() =>
            {
                textBoxResultInfo.Enabled = true;
            }));
            string message = postToDisplay.Message == null ? string.Empty : postToDisplay.Message;
            string resultInfo = string.Format(
@"{0}
{1}
Likes: {2}
Comments: {3}
Score: {4}",
message,
postToDisplay.CreatedTime.GetValueOrDefault(),
postToDisplay.LikedBy.Count,
postToDisplay.Comments.Count,
getPostScore(postToDisplay));
            textBoxResultInfo.Text = resultInfo;
        }

        private void checkedListBoxPostType_SelectedIndexChanged(object sender, EventArgs e)
        {
            setItemsAsChecked();
        }

        private void setItemsAsChecked()
        {
            if (checkedListBoxPostType.SelectedItem.ToString().Equals("All"))
            {
                for (int i = 0; i < checkedListBoxPostType.Items.Count; i++)
                {
                    if (!checkedListBoxPostType.GetItemChecked(i))
                    {
                        checkedListBoxPostType.SetItemChecked(i, true);
                    }
                }
            }
            else
            {
                for (int i = 0; i < checkedListBoxPostType.Items.Count; i++)
                {
                    if (!checkedListBoxPostType.SelectedItem.Equals(checkedListBoxPostType.Items[i]))
                    {
                        checkedListBoxPostType.SetItemChecked(i, false);
                    }
                    else
                    {
                        checkedListBoxPostType.SetItemChecked(i, true);
                    }
                }
            }
        }
    }
}
