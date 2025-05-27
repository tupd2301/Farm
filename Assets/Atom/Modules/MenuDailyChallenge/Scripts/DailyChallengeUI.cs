using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Athena.Common.UI;
using UnityEngine.UI;
using TMPro;

namespace Atom
{
    public class DailyChallengeUI : UIController
    {
        public System.Action onDailyFlowFinished, onNoInternetExceptionTriggered;
        private const int DAILY_START_YEAR = 2023;

        [SerializeField]
        private RectTransform _calendarDatesContent, _calendarDateTextsContent;
        [SerializeField]
        private CalendarDateUI _calendarDatePrefab;
        [SerializeField]
        private TextMeshProUGUI _bannerMonthText, _btnDateText, _statusDateText;
        [SerializeField]
        private GameObject _btnPlay, _btnComplete, _btnUnavailable, _adIcon, _playIcon;

        private System.DateTime _currentMonth;
        private System.DateTime _todayDate;

        private CalendarDateUI _currentSelectedDateUI = null;
        private CalendarDateUI[] _dateUIs;

        private int _winCount;
        
        private bool _rewardedPlayDisabled = false;

        private System.DateTime _dailyStartDate;

        public System.DateTime DailyStartDate
        {
            set
            {
                _dailyStartDate = value;
            }
        }

        public bool ShowAdsToPlay
        {
            get
            {
                return _adIcon.activeSelf;
            }
        }

        public ProgressBarUI ProgressBarUI;

        public System.Action ActionPlayDate;

        public System.DateTime CurrentSelectedDateUI
        {
            get
            {
                return _currentSelectedDateUI.GetDate();
            }
        }

        void Awake()
        {
            var now = System.DateTime.Today;
            _todayDate = new System.DateTime(now.Year, now.Month, now.Day);
            //_rewardedPlayDisabled = Global.RemoteConfigGetter.DAILY_REWARDED_PLAY_DISABLED;
            //_dailyStartDate = new System.DateTime(DAILY_START_YEAR, 1, 1);
            //_cheatBtn.SetActive(Global.RemoteConfigGetter.IS_CHEAT_ENABLED);
            //_isCheatIdEnable = false;
            //UpdateCheatUI();
        }

        private void OnEnable()
        {
            //ShowDailyLevelCompletedUI();
        }

        public void Setup(System.DateTime month, System.DateTime? completeDate = null)
        {
            //UpdateLockedUI();
            _currentMonth = new System.DateTime(month.Year, month.Month, 1);
            //_willShowCompletePopup = completeDate.HasValue;
            //if (_willShowCompletePopup)
            //{
            //    _currentMonth = new(completeDate.Value.Year, completeDate.Value.Month, 1);
            //}

            System.DateTime firstDate = new System.DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            int deltaDay = (firstDate.DayOfWeek == System.DayOfWeek.Sunday) ? -6 : 1 - (int)firstDate.DayOfWeek;
            System.DateTime firstDateCalendar = firstDate.AddDays(deltaDay);
            System.DateTime lastDate = new System.DateTime(_currentMonth.Year, _currentMonth.Month, 1).AddMonths(1).AddDays(-1);
            deltaDay = (lastDate.DayOfWeek == System.DayOfWeek.Sunday) ? 0 : 7 - (int)lastDate.DayOfWeek;
            System.DateTime lastDateCalendar = lastDate.AddDays(deltaDay);
            System.TimeSpan calendarDays = lastDateCalendar.AddDays(1) - firstDateCalendar;
            int rowCount = (int)calendarDays.TotalDays / 7;
            Vector2 startPos = new Vector2(-333, 0);
            if (_dateUIs == null)
            {
                _dateUIs = new CalendarDateUI[42];
            }
            if (rowCount <= 5)
            {
                _calendarDateTextsContent.anchoredPosition = new Vector2(0, -100);
                _calendarDatesContent.anchoredPosition = new Vector2(0, -230);
            }
            else
            {
                _calendarDateTextsContent.anchoredPosition = new Vector2(0, -35);
                _calendarDatesContent.anchoredPosition = new Vector2(0, -155);
            }
            bool hasSelected = false;
            _winCount = 0;
            for (int i = 0; i < calendarDays.TotalDays; i++)
            {
                System.DateTime date = firstDateCalendar.AddDays(i);
                if (date.Month != _currentMonth.Month)
                {
                    continue; //not use full calendar
                }
                CalendarDateUI dateUI = _dateUIs[i];
                if (dateUI == null)
                {
                    dateUI = Instantiate(_calendarDatePrefab, _calendarDatesContent.transform);
                    _dateUIs[i] = dateUI;
                }
                dateUI.gameObject.SetActive(true);
                RectTransform rt = dateUI.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 1);
                rt.anchorMax = new Vector2(0.5f, 1);
                rt.anchoredPosition = new Vector3(startPos.x + (i % 7) * (rt.sizeDelta.x + 5), startPos.y - (i / 7) * (rt.sizeDelta.y + 5));
                CalendarDateItemData data = new CalendarDateItemData();
                data.Date = date;
                data.IsCompleted = IsDailyComplete(date) && (date - lastDate).TotalDays <= 0 && (date - firstDate).TotalDays >= 0;
                if (data.IsCompleted)
                {
                    _winCount++;
                }
                if (hasSelected)
                {
                    data.IsSelected = false;
                }
                else if (completeDate.HasValue)
                {
                    if ((date - completeDate.Value).TotalDays == 0)
                    {
                        data.IsSelected = true;
                        _currentSelectedDateUI = dateUI;
                        hasSelected = true;
                        data.IsCompleted = false;
                    }
                    else
                    {
                        data.IsSelected = false;
                    }
                }
                else if ((date - _todayDate).TotalDays == 0)
                {
                    data.IsSelected = true;
                    _currentSelectedDateUI = dateUI;
                    hasSelected = true;
                }
                else if ((date - lastDate).TotalDays == 0 && !hasSelected)
                {
                    _currentSelectedDateUI = dateUI;
                    data.IsSelected = true;
                }
                else
                {
                    data.IsSelected = false;
                }
                if ((date - lastDate).TotalDays > 0 || (date - _todayDate).TotalDays > 0 || (date - firstDate).TotalDays < 0)
                {
                    data.IsAvailable = false;
                }
                else
                {
                    data.IsAvailable = true;
                }
                dateUI.OnClickedFunc = OnSelectDate;

                dateUI.Setup(data);
            }
            UpdateBtnDateText();
            UpdateBannerMonthText();
            UpdatePlayButtonStatus();
            ProgressBarUI.UpdateBadgeIcon(_winCount + 15, _currentMonth);

            /*
            if (_willShowCompletePopup)
            {
                ShowDailyLevelCompletedUI();
            }
            */
        }

        private DailyLevelCompleteUI _completeUI;
        private void ShowDailyLevelCompletedUI()
        {
            //AppManager.Instance.TrackingSceneName(Global.TrackingScreenName.DC_WIN);

            _completeUI = UIManager.Instance.ShowUIOnTop<DailyLevelCompleteUI>("Detective/DailyLevelCompletedUI");
            //_completeUI.Setup(completeDate.Value);
            _completeUI.onCollectPressed = () =>
            {
                //AudioManager.Instance.PlaySFX(AudioId.ItemClaim);
                //_completeUI.HideAndShowStarAnim();
                UIManager.Instance.ReleaseUI(_completeUI, true);
                ProgressBarUI.ActionProgress();
            };
        }

        private void ResetCalendar()
        {
            if (_dateUIs == null)
            {
                return;
            }
            for (int i = 0; i < _dateUIs.Length; i++)
            {
                if (_dateUIs[i] != null)
                {
                    _dateUIs[i].gameObject.SetActive(false);
                }
            }
        }

        private void UpdateBtnDateText()
        {
            if (_currentSelectedDateUI == null)
            {
                return;
            }
            System.DateTime date = _currentSelectedDateUI.GetDate();
            _btnDateText.text = ToDateString(date, 10, 18);
            _statusDateText.text = _btnDateText.text;
        }

        private void UpdateBannerMonthText()
        {
            _bannerMonthText.text = _currentMonth.ToString("MMM yyy");
        }

        private void UpdatePlayButtonStatus()
        {
            var currentDate = _currentSelectedDateUI.GetDate();
            if (IsDailyComplete(_currentSelectedDateUI.GetDate()))
            {
                _btnComplete.SetActive(true);
                _btnPlay.SetActive(false);
                _btnUnavailable.SetActive(false);
            }
            else if ((currentDate - _todayDate).TotalDays > 0)
            {
                _btnComplete.SetActive(false);
                _btnPlay.SetActive(false);
                _btnUnavailable.SetActive(true);
            }
            else
            {
                if ((currentDate - _todayDate).TotalDays != 0 && !_rewardedPlayDisabled)
                {
                    _adIcon.SetActive(true);
                    _playIcon.SetActive(false);
                }
                else
                {
                    _adIcon.SetActive(false);
                    _playIcon.SetActive(true);
                }
                _btnComplete.SetActive(false);
                _btnPlay.SetActive(true);
                _btnUnavailable.SetActive(false);
            }
        }

        

        public void OnSelectDate(CalendarDateUI dateUI)
        {
            _currentSelectedDateUI.SetSelect(false);
            _currentSelectedDateUI = dateUI;
            _currentSelectedDateUI.SetSelect(true);
            UpdateBtnDateText();
            UpdatePlayButtonStatus();
        }

        private void ChangeMonth(int monthChange)
        {
            System.DateTime newMonth = _currentMonth.AddMonths(monthChange);
            if ((newMonth - _todayDate).TotalDays > 0)
            {
                return;
            }
            if ((newMonth - _dailyStartDate).TotalSeconds < 0)
            {
                return;
            }
            ResetCalendar();
            _currentMonth = newMonth;
            Setup(_currentMonth);
        }

        public void OnNextMonthClicked()
        {
            //AudioManager.Instance.PlaySFX(Common.AudioId.ButtonTap, usePlayIndex: true);
            ChangeMonth(1);
        }

        public void OnPrevMonthClicked()
        {
            //AudioManager.Instance.PlaySFX(Common.AudioId.ButtonTap, usePlayIndex: true);
            ChangeMonth(-1);
        }

        public void OnPlayClicked()
        {
            ActionPlayDate?.Invoke();
            //AudioManager.Instance.PlaySFX(Common.AudioId.ButtonTap, usePlayIndex: true);
            /*
            var currentDate = _currentSelectedDateUI.GetDate();
            if ((currentDate - _todayDate).Days != 0 && !_rewardedPlayDisabled)
            {
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    onNoInternetExceptionTriggered?.Invoke();
                    return;
                }

                AppManager.Instance.ShowRewardedAds("FreeDaily", (success) => {
                    if (success)
                    {
                        PlayCurrentDaily();
                    }
                });
            }
            else
            {
                PlayCurrentDaily();
            }
            */
        }

        

        public bool IsDailyComplete(System.DateTime date)
        {
            return false;
            /*
            string month = string.Format("{0}_{1}", date.Year, date.Month);
            if (!_dailyCountsByMonth.ContainsKey(month))
            {
                return false;
            }

            int day = date.Day;
            if (!_dailyCountsByMonth[month].ContainsKey(day))
            {
                return false;
            }
            return _dailyCountsByMonth[month][day] == 1;
            */
        }

        public string ToDateString(System.DateTime date, int offset, int subStrSize, bool useMonthFull = false, int style = 0)
        {
            string monthStr = useMonthFull ? date.ToString("MMMM") : date.ToString("MMM");
            string format = style switch
            {
                1 => "{0}<voffset={2}><size={3}>{1}</size></voffset> {4}",
                _ => "{4} {0}<voffset={2}><size={3}>{1}</size></voffset>"
            };
            return string.Format(format, date.Day, ToDateSubString(date), offset, subStrSize, monthStr);
        }

        public string ToDateSubString(System.DateTime date)
        {
            string dateSub = date.Day switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                21 => "st",
                22 => "nd",
                23 => "rd",
                31 => "st",
                _ => "th"
            };
            return dateSub;
        }


        /*
        private DailyLevelCompleteUI _completeUI;
        private bool _willShowCompletePopup = false;
        private bool _isCheatIdEnable = false;
        private int _cheatId = 1;

        [SerializeField]
        private GameObject _lockedLayer;
        [SerializeField]
        private TextMeshProUGUI _lockedTimerText;

        [SerializeField]
        private GameObject _cheatBtn;
        [SerializeField]
        private TextMeshProUGUI _cheatBtnText;
        [SerializeField]
        private TMP_InputField _cheatInputField;

        private bool _isLocked = false;

        private void UpdateCheatUI()
        {
            if (!_cheatBtn.activeSelf)
            {
                return;
            }
            _cheatBtnText.text = "Cheat Id Enable:\n" + (_isCheatIdEnable ? "YES" : "NO");
            _cheatInputField.text = _cheatId.ToString();
        }

        private void FixedUpdate()
        {
            if (_isLocked)
            {
                UpdateLockedUI();
            }
        }

        public void UpdateLockedUI()
        {
            System.DateTime FirstOpenDate = System.DateTime.UtcNow;
            System.DateTime dateOne = FirstOpenDate.ToLocalTime().AddDays(1);
            dateOne = new System.DateTime(dateOne.Year, dateOne.Month, dateOne.Day);
            System.TimeSpan firstDateTimeRemaining = dateOne - System.DateTime.Now;
            _isLocked = firstDateTimeRemaining.TotalSeconds > 0;
            if (_isLocked)
            {
                _lockedLayer.SetActive(true);
                _lockedTimerText.text = firstDateTimeRemaining.ToString(@"hh\:mm\:ss");
            }
            else
            {
                _lockedLayer.SetActive(false);
            }
        }

        public void OnCheatIdInputChange()
        {
            int.TryParse(_cheatInputField.text, out _cheatId);
            _cheatId = Mathf.Clamp(_cheatId, 1, AppManager.Instance.DailyLevelSet.Levels.Count);
            _cheatInputField.text = _cheatId.ToString();
        }

        public void OnCheatBtnPressed()
        {
            //AudioManager.Instance.PlaySFX(Common.AudioId.ButtonTap, usePlayIndex: true);
            _isCheatIdEnable = !_isCheatIdEnable;
            _cheatBtnText.text = "Cheat Id Enable:\n" + (_isCheatIdEnable ? "YES" : "NO");
        }

        public void PlayCurrentDaily()
        {
            var daily = AppManager.Instance.DailyLevelSet;
            var date = _currentSelectedDateUI.GetDate();
            int index = AppManager.Instance.GetDailyLevelId(_currentSelectedDateUI.GetDate());
            if (_isCheatIdEnable)
            {
                index = _cheatId - 1;
            }
            AppManager.Instance.Switch(new AppStateDailyLevel()
            {
                Date = date,
                MatchData = daily.Levels[index].Clone(),
                LevelId = index + 1,
                Root = "Menu_DC",
            });
        }
        */
    }
}