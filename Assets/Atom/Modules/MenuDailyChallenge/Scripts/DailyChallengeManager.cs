using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtils;

namespace Atom
{
    public class DailyChallengeManager : SingletonMono<DailyChallengeManager>
    {
        private DailyChallengeUI _dailyChallengeUI;
        public DailyChallengeUI DailyChallengeUI
        {
            get
            {
                return _dailyChallengeUI;
            }
        }
        public void Setup(System.Action<int, bool> action)
        {
            List<QuestCalendarData> questCalendarDatas = LoadQuestCalendarData();
            UnityEngine.Debug.Log("ok : " + questCalendarDatas.Count);
            _dailyChallengeUI = AppManager.Instance.ShowSafeTopUI<DailyChallengeUI>("Atom/DailyChallengeUI", false);
            _dailyChallengeUI.ActionPlayDate = () =>
            {
                string date = _dailyChallengeUI.CurrentSelectedDateUI.ToString("yyyy/MM/dd");
                for (int i = 0; i < questCalendarDatas.Count; i++)
                {
                    if (questCalendarDatas[i].date == date)
                    {
                        action?.Invoke(questCalendarDatas[i].level, _dailyChallengeUI.ShowAdsToPlay);
                        break;
                    }
                }
            };
            _dailyChallengeUI.DailyStartDate = System.DateTime.Parse(questCalendarDatas[0].date);
        }

        private List<QuestCalendarData> LoadQuestCalendarData()
        {
            QuestCalendarContainer detectiveStoryLevelContainer = JsonUtility.FromJson<QuestCalendarContainer>("{\"questCalendarDatas\":{0}}".Replace("{0}", Resources.Load<TextAsset>(string.Format("Atom/{0}", "QuestCalendar")).text));
            return detectiveStoryLevelContainer.questCalendarDatas;
        }
    }

    public class QuestCalendarContainer
    {
        public List<QuestCalendarData> questCalendarDatas;
    }

    [System.Serializable]
    public class QuestCalendarData
    {
        public string date;
        public int level;
    }
}