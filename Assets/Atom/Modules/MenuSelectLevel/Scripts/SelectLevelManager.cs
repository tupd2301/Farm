using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomUtils;

namespace Atom
{
    public class SelectLevelManager : SingletonMono<SelectLevelManager>
    {
        protected SelectLevelUI _storyLevelUI;

        public SelectLevelUI StoryLevelUI
        {
            get
            {
                return _storyLevelUI;
            }
        }

        protected GameObject[] inventoryButtons;
        protected GameObject[] equipButtons;


        public void Setup(string namePrefabUI, string storyData, System.Action<List<int>> action)
        {
            List<QuestStoryData> questStoryDatas = LoadQuestStoryData(storyData);
            _storyLevelUI = AppManager.Instance.ShowSafeTopUI<SelectLevelUI>(namePrefabUI, false);
            for (int i = 0; i < questStoryDatas.Count; i++)
            {
                if (!string.IsNullOrEmpty(questStoryDatas[i].name))
                {
                    Button button = GameObject.Instantiate(_storyLevelUI.ItemPrefab, _storyLevelUI.Content).GetComponent<Button>();
                    SelectLevelItem storyLevelItem = button.GetComponent<SelectLevelItem>();
                    //equipmentItem.Icon.sprite = Resources.Load<Sprite>(itemID.image);
                    //equipmentItem.Icon.SetNativeSize();
                    storyLevelItem.TextName.text = questStoryDatas[i].name;
                    List<int> levels = new List<int>();
                    levels.Add(questStoryDatas[i].level);
                    int j = i + 1;
                    while (j < questStoryDatas.Count && string.IsNullOrEmpty(questStoryDatas[j].name))
                    {
                        levels.Add(questStoryDatas[j].level);
                        j++;
                    }
                    storyLevelItem.ButtonPlay.onClick.AddListener(() => {
                        action?.Invoke(levels);
                    });
                }
            }
        }

        //---------------------------

        public int ParseInt(string stringNumber)
        {
            int number = 0;
            int.TryParse(stringNumber, out number);
            return number;
        }

        private string ChangeStringNumber(string stringNumber, int number)
        {
            int changeNumber = 0;
            if (int.TryParse(stringNumber, out changeNumber))
            {
                changeNumber = changeNumber + number;
                return changeNumber.ToString();
            }
            return stringNumber;
        }

        private List<QuestStoryData> LoadQuestStoryData(string storyData)
        {
            QuestStoryContainer questStoryContainer = JsonUtility.FromJson<QuestStoryContainer>("{\"questStoryDatas\":{0}}".Replace("{0}", Resources.Load<TextAsset>(string.Format("Atom/{0}", storyData)).text));
            return questStoryContainer.questStoryDatas;
        }
    }

    public class QuestStoryContainer
    {
        public List<QuestStoryData> questStoryDatas;
    }

    [System.Serializable]
    public class QuestStoryData
    {
        public int id;
        public string name;
        public int level;
    }
}