using System;
using System.Collections.Generic;

namespace Atom
{
    
    public class LeaderboardSaveData
    {
        public string Name;
        public DateTime lastUpdateTime;
        public List<LeaderboardEntry> entries;
    }
    
}