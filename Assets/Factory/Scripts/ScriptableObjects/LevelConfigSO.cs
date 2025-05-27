using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Factory
{
    [CreateAssetMenu(fileName = "LevelConfigSO", menuName = "Factory/LevelConfigSO")]
    public class LevelConfigSO : ScriptableObject
    {
        public List<LevelConfiguration> levelConfigs;

        [ReadOnly]
        public BoxConfigSO boxConfigSO;

        void OnValidate()
        {
            foreach (var levelConfig in levelConfigs)
            {
                levelConfig.gridSize = new Vector2Int(
                    Mathf.Clamp(levelConfig.gridSize.x, 1, 7),
                    Mathf.Clamp(levelConfig.gridSize.y, 1, 4)
                );
                levelConfig.levelNumber = levelConfigs.IndexOf(levelConfig);
                levelConfig.dayConfigurations.Sort((a, b) => a.dayNumber.CompareTo(b.dayNumber));
                int percentOutputCash = 0;
                for (int i = 0; i < levelConfig.dayConfigurations.Count; i++)
                {
                    levelConfig.dayConfigurations[i].dayNumber = i + 1;
                    List<BoxConfigDay> boxs = levelConfig.dayConfigurations[i].boxConfigs;
                    int totalWeight = 0;
                    int totalBoxes = 0;
                    foreach (var box in boxs)
                    {
                        var boxConfig = new BoxConfig();
                        var config = boxConfigSO.boxConfigs.Find(
                            (boxConfig) => boxConfig.boxType == box.boxType
                        );
                        if (config == null)
                        {
                            Debug.LogError("BoxConfig not found: " + box.boxType);
                            boxs.Remove(box);
                            continue;
                        }
                        boxConfig.Copy(
                            boxConfigSO.boxConfigs.Find(
                                (boxConfig) => boxConfig.boxType == box.boxType
                            )
                        );
                        if (boxConfig == null)
                        {
                            Debug.LogError("BoxConfig not found: " + box.boxType);
                            continue;
                        }
                        box.boxConfig.Copy(boxConfig);
                        totalWeight += boxConfig.weight;
                        totalBoxes += box.amount;
                    }
                    if (totalBoxes == 0)
                    {
                        Debug.LogError("Total boxes is 0");
                    }
                    if (totalWeight == 0)
                    {
                        Debug.LogError("Total weight is 0");
                    }
                    levelConfig.dayConfigurations[i].numberOfBoxes = totalBoxes;
                    float moneyOfDay =
                        levelConfig.dayConfigurations[i].percentOutputCash
                        * levelConfig.maxOutputCash
                        / 100;

                    foreach (var box in boxs)
                    {
                        box.boxConfig.boxCurrencyValue = (int)(
                            (box.boxConfig.weight / (float)totalWeight)
                            * moneyOfDay
                            / (float)box.amount
                        );
                    }
                    if (i == levelConfig.dayConfigurations.Count - 1)
                    {
                        levelConfig.dayConfigurations[i].percentOutputCash =
                            100 - percentOutputCash;
                    }
                    percentOutputCash += levelConfig.dayConfigurations[i].percentOutputCash;
                }
            }
            levelConfigs.Sort((a, b) => a.levelNumber.CompareTo(b.levelNumber));
        }
    }

    [System.Serializable]
    public class LevelConfiguration
    {
        public int levelNumber;
        public int maxOutputCash = 1000;
        public Vector2Int gridSize = new Vector2Int(7, 4);
        public int maxHeadGearSlots = 1;
        public int initialLevelCurrency = 50;
        public List<DayConfiguration> dayConfigurations;
    }

    [System.Serializable]
    public class BoxConfigDay
    {
        public BoxType boxType;
        public int amount;

        [ReadOnly]
        public BoxConfig boxConfig;
    }

    [System.Serializable]
    public class DayConfiguration
    {
        public int dayNumber;
        public int numberOfBoxes = 5;

        public List<BoxConfigDay> boxConfigs;
        public int initialDayCurrency = 50;
        public int percentOutputCash;
    }
}
