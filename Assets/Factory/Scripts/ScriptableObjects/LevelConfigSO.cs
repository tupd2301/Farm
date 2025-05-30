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
        public FishConfigSO fishConfigSO;

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
                    List<FishConfigDay> fishes = levelConfig.dayConfigurations[i].fishConfigs;
                    int totalWeight = 0;
                    int totalFishes = 0;
                    foreach (var fish in fishes)
                    {
                        var fishConfig = new FishConfig();
                        var config = fishConfigSO.fishConfigs.Find(
                            (fishConfig) => fishConfig.fishType == fish.fishType
                        );
                        if (config == null)
                        {
                            Debug.LogError("FishConfig not found: " + fish.fishType);
                            fishes.Remove(fish);
                            continue;
                        }
                        fishConfig.Copy(
                            fishConfigSO.fishConfigs.Find(
                                (fishConfig) => fishConfig.fishType == fish.fishType
                            )
                        );
                        if (fishConfig == null)
                        {
                            Debug.LogError("FishConfig not found: " + fish.fishType);
                            continue;
                        }
                        fish.fishConfig.Copy(fishConfig);
                        totalWeight += fishConfig.weight;
                        totalFishes += fish.amount;
                    }
                    if (totalFishes == 0)
                    {
                        Debug.LogError("Total fishes is 0");
                    }
                    if (totalWeight == 0)
                    {
                        Debug.LogError("Total weight is 0");
                    }
                    levelConfig.dayConfigurations[i].numberOfFishes = totalFishes;
                    float moneyOfDay =
                        levelConfig.dayConfigurations[i].percentOutputCash
                        * levelConfig.maxTotalFishHP
                        / 100;
                    float coinOfDay =
                        levelConfig.dayConfigurations[i].percentOutputCash
                        * levelConfig.maxCoinDrop
                        / 100;

                    foreach (var fish in fishes)
                    {
                        fish.fishConfig.fishCurrencyValue = Mathf.Max(
                            Mathf.RoundToInt(
                                (fish.fishConfig.weight / (float)totalWeight)
                                    * moneyOfDay
                                    / (float)fish.amount
                            ),
                            1
                        );
                        fish.fishConfig.dropCoinValue = Mathf.Max(
                            Mathf.RoundToInt(
                                (fish.fishConfig.weight / (float)totalWeight)
                                    * coinOfDay
                                    / (float)fish.amount
                            ),
                            1
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
        public int maxTotalFishHP = 1000;
        public int maxCoinDrop = 1000;
        public Vector2Int gridSize = new Vector2Int(7, 4);
        public int maxHeadGearSlots = 1;
        public int initialLevelCurrency = 50;
        public List<DayConfiguration> dayConfigurations;
    }

    [System.Serializable]
    public class FishConfigDay
    {
        public FishType fishType;
        public int amount;

        [ReadOnly]
        public FishConfig fishConfig;
    }

    [System.Serializable]
    public class DayConfiguration
    {
        public int dayNumber;
        public int numberOfFishes = 5;

        public List<FishConfigDay> fishConfigs;
        public int initialDayCurrency = 50;
        public int percentOutputCash;
        public int maxInPool = 10;
    }
}
