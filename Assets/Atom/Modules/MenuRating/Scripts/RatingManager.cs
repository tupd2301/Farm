using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtils;
using Athena.Common.UI;

namespace Atom
{
    public class RatingManager : SingletonMono<RatingManager>
    {
        protected RatingUI _ratingUI;

        public RatingUI RatingUI
        {
            get
            {
                return _ratingUI;
            }
        }

        public void ShowUI()
        {
            int numberOfStar = 0;
            _ratingUI = AppManager.Instance.ShowSafeTopUI<RatingUI>("Atom/RatingUI", false);
            //ratingUI.onHideFinished = () => {
            //_isWaiting = false;
            //UIManager.Instance.ReleaseUI(ratingUI, true);
            //LocalDatabase.SetValue(Global.LocalDatabaseKey.LAST_LEVEL_SHOWED_RATING_UI, levelToShow + Global.GameConfig.RATING_UI_LEVEL_INTERVAL);
            //};
            _ratingUI.onCancelPressed = () => {
                UIManager.Instance.ReleaseUI(_ratingUI, true);
                //ratingUI.Hide();
            };
            _ratingUI.onOKPressed = () => {
                switch (numberOfStar)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        //AppManager.Instance.SendEmail();
                        UIManager.Instance.ReleaseUI(_ratingUI, true);
                        ShowFeedbackThankUI("Rating", "Thank you", "OK");
                        break;
                    case 5:
                        OpenStoreURL();
                        //ShowRatingThankUI();
                        break;
                }
                _ratingUI.Hide();
                //AppManager.Instance.TrackingButtonTap("Send", "Rating");
                TrackingManager.Instance.TrackRate(string.Format("rated{0}", numberOfStar), "", 1);
            };
            _ratingUI.onStarPressed = (index) => {
                UnityEngine.Debug.Log("ok : star");
                _ratingUI.SetActiveSendButton(true);
                numberOfStar = index + 1;
                _ratingUI.Rate(index);
                //AppManager.Instance.TrackingButtonTap(string.Format("Star_{0}", numberOfStar), "Rating");
            };
            _ratingUI.Setup();
            _ratingUI.Show();
        }

        public void ShowFeedbackThankUI(string title, string text, string buttonText)
        {
            ConfirmUI feedbackThankUI = AppManager.Instance.ShowSafeTopUI<ConfirmUI>("Atom/FeedbackThankUI", false);
            feedbackThankUI.onOKPressed = () => {
                UIManager.Instance.ReleaseUI(feedbackThankUI, true);
            };
            feedbackThankUI.Setup(title, text, buttonText);
        }

        public void OpenStoreURL()
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=game.puzzle.woody.blockbuster");
        }
    }
}