using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Athena.Common.UI;
using UnityEngine.UI;
using TMPro;

namespace Atom
{
    public class ProgressBarUI : MonoBehaviour
    {
        [SerializeField] private Sprite _spriteBadge;
        [SerializeField] private Sprite _spriteBadgeLocked;
        private float _progressWidth;

        [SerializeField]
        private int[] BADGE_UNLOCK_REQUIRE;

        private float SIZE = 900;

        [SerializeField]
        private UnityEngine.UI.Image[] _badgeIcon;
        [SerializeField]
        private TextMeshProUGUI[] _badgeText;
        [SerializeField]
        private RectTransform _progressBar;
        [SerializeField]
        private TextMeshProUGUI _winCountText;

        private void Setup()
        {
            float final = BADGE_UNLOCK_REQUIRE[BADGE_UNLOCK_REQUIRE.Length - 1];
            for (int i = 0; i < BADGE_UNLOCK_REQUIRE.Length - 1; i++)
            {
                _badgeIcon[i].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(SIZE * 0.5f - BADGE_UNLOCK_REQUIRE[i] / final * SIZE, 0);
            }
        }

        public void UpdateBadgeIcon(int winCount, System.DateTime currentMonth)
        {
            Setup();
            int winCountShow = winCount;
            //if (_willShowCompletePopup)
            //{
            //    winCountShow = _winCount - 1;
            //}
            _winCountText.text = winCountShow.ToString();

            float progressWidth = 0;
            for (int i = 0; i < BADGE_UNLOCK_REQUIRE.Length; i++)
            {
                bool isUnlocked = false;
                if (winCountShow > BADGE_UNLOCK_REQUIRE[i])
                {
                    isUnlocked = true;
                }
                progressWidth = winCountShow * SIZE / BADGE_UNLOCK_REQUIRE[BADGE_UNLOCK_REQUIRE.Length - 1];
                if (isUnlocked)
                {
                    _badgeIcon[i].sprite = _spriteBadge;
                    _badgeIcon[i].SetNativeSize();
                    _badgeText[i].gameObject.SetActive(false);
                }
                else
                {
                    _badgeIcon[i].sprite = _spriteBadgeLocked;
                    _badgeIcon[i].SetNativeSize();
                    _badgeText[i].gameObject.SetActive(true);
                    _badgeText[i].text = string.Format("{0}", BADGE_UNLOCK_REQUIRE[i]);
                }
            }
            /*
            if (_willShowCompletePopup)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (_winCount <= BADGE_UNLOCK_REQUIRE[i])
                    {
                        if (i == 0)
                        {
                            float d = (float)(PROGRESS_MILSTONE_SIZE_END[i] - PROGRESS_MILSTONE_SIZE_START[i]);
                            _progressWidth = PROGRESS_MILSTONE_SIZE_START[i] + (float)_winCount / (BADGE_UNLOCK_REQUIRE[i] - 1) * d;
                        }
                        else if (i >= 0)
                        {
                            float currentD = (float)(_winCount - BADGE_UNLOCK_REQUIRE[i - 1]);
                            float totalD = (float)(BADGE_UNLOCK_REQUIRE[i] - BADGE_UNLOCK_REQUIRE[i - 1] - 1);
                            float d = (float)(PROGRESS_MILSTONE_SIZE_END[i] - PROGRESS_MILSTONE_SIZE_START[i]);
                            _progressWidth = PROGRESS_MILSTONE_SIZE_START[i] + ((currentD / totalD) * d);
                        }
                        else if (i == 2)
                        {
                            _progressWidth = PROGRESS_MILSTONE_SIZE[i];
                        }
                        break;
                    }
                }
            }
            else
            {
                _progressWidth = progressWidth;
            }
            */
            _progressWidth = progressWidth;
            _progressBar.sizeDelta = new Vector2(progressWidth, _progressBar.sizeDelta.y);
        }

        public void ActionProgress()
        {
            StartCoroutine(ProgressUpdateProcess());
        }

        private IEnumerator ProgressUpdateProcess()
        {
            _progressBar.sizeDelta = new Vector2(0, _progressBar.sizeDelta.y);

            float delta = (_progressWidth - _progressBar.sizeDelta.x) / 10;
            while (_progressBar.sizeDelta.x < _progressWidth)
            {
                _progressBar.sizeDelta = new Vector2(_progressBar.sizeDelta.x + delta, _progressBar.sizeDelta.y);
                yield return new WaitForSeconds(0.05f); //Yielders.Get(0.05f);
            }
            /*
            int unlockIndex = -1;
            for (int i = 0; i < 3; i++)
            {
                if (_winCount == BADGE_UNLOCK_REQUIRE[i])
                {
                    unlockIndex = i;
                    break;
                }
            }
            if (unlockIndex >= 0)
            {
                DailyConfig dailyConfig = ResourceManager.Instance.GetDailyConfig();
                int month = _currentMonth.Month;
                DailyBadgeImageSet badgeSet = dailyConfig.GetBadgeConfigByMonth(month).Badges[unlockIndex];
                bool isWaiting = true;
                var unlockedSprite = dailyConfig.GetSpriteFromAtlases(badgeSet.IconImage);
                var rewardUI = UIManager.Instance.ShowUIOnTop<DailyUnlockRewardUI>("DailyUnlockRewardUI");
                rewardUI.onHideFinished = () => {
                    onDailyFlowFinished?.Invoke();
                    isWaiting = false;
                    _badgeIcon[unlockIndex].sprite = unlockedSprite;
                    _badgeText[unlockIndex].gameObject.SetActive(false);
                    UIManager.Instance.ReleaseUI(rewardUI, true);
                };
                rewardUI.onOKPressed = () => {
                    AudioManager.Instance.PlaySFX(AudioId.ItemClaim);
                    rewardUI.RewardEndPos = _badgeIcon[unlockIndex].gameObject.transform.position;
                    rewardUI.RewardEndSize = unlockedSprite.bounds.size;
                    rewardUI.HideAndShowRewardAnim();
                };

                rewardUI.Setup(month, unlockIndex);
                rewardUI.Show();
                
                while (isWaiting)
                {
                    yield return null;
                }
            }
            else
            {
                onDailyFlowFinished?.Invoke();
            }
            */
        }
    }
}