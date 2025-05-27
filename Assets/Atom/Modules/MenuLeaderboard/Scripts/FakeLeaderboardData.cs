using System;
using System.Collections.Generic;
using UnityEngine;

namespace Atom
{
    [CreateAssetMenu(fileName = "FakeLeaderboardData", menuName = "FakeLeaderboardData", order = 0)]
    public class FakeLeaderboardData : ScriptableObject 
    {
        public int entriesToShow;
        public int promoteEntries;
        public int remainEntries;
        public int highestScore;
        public int duration;
        public float difficulty;
        
        [Header("Bot Config")]
        public int baseScore;
        public int botMaxScore;
        public List<Vector2> botLevelRanges;
        public List<int> botLevelRates;
        [Tooltip("Update interval in hours")]
        public int botUpdateInterval;
        [TextArea(10, 20)]
        public string fakeNames;
        

        
    }
    
}