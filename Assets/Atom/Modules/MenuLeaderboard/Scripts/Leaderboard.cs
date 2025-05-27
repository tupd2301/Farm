using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Atom
{
    public class Leaderboard : IDisposable
    {
        private event Action OnLeaderboardUpdated;

        private List<LeaderboardEntry> boardData;

        public bool IsEmpty
        {
            get
            {
                return boardData.Count == 0;
            }
        }

        public bool IsExpired
        {
            get
            {
                return DateTime.Now >= timeExpire;
            }
        }

        public int promoteEntries;
        public int remainEntries;
        public int demoteEntries
        {
            get
            {
                return boardData.Count - promoteEntries - remainEntries;
            }
        }

        public List<LeaderboardEntry> BoardData { get => boardData; }
        public DateTime timeCreated;
        public DateTime timeExpire;

        public Leaderboard(int hours)
        {
            boardData = new List<LeaderboardEntry>();
            timeCreated = DateTime.Now;
            timeExpire = timeCreated.AddHours(hours);

            OnLeaderboardUpdated += Sort;
            OnLeaderboardUpdated += AssignRanks;
        }

        public void AddEntry(int rank, int score, string name, LeaderBoardEntryStatus status, bool sort = false)
        {
            var newEntry = new LeaderboardEntry
            {
                Rank = rank,
                StatValue = score,
                Name = name,
                Status = status,
                Type = LeaderboardEntryType.Player
            };
            boardData.Add(newEntry);

            if (sort)
            {
                OnLeaderboardUpdated?.Invoke();
            }
        }

        public void AddBotEntry(string name, int score, float botLevel)
        {
            var newEntry = new LeaderboardEntry
            {
                Rank = 0,
                StatValue = score,
                Name = name,
                BotLevel = botLevel,
                Type = LeaderboardEntryType.Bot
            };

            boardData.Add(newEntry);
        }

        public void RemoveEntry(string name, int score)
        {
            var entryToRemove = boardData.Find(x => x.Name == name && x.StatValue == score);
            if (entryToRemove != null)
            {
                boardData.Remove(entryToRemove);
            }
        }

        public void Sort()
        {
            boardData.Sort(SortLeaderBoardEntry);
        }

        public void AssignRanks()
        {
            for (int i = 0; i < boardData.Count; i++)
            {
                boardData[i].Rank = i + 1; // Rank starts from 1
            }
        }

        public LeaderboardEntry GetPlayerEntry()
        {
            return boardData.Find(x => x.Type == LeaderboardEntryType.Player);
        }

        private int SortLeaderBoardEntry(LeaderboardEntry entry1, LeaderboardEntry entry2)
        {
            if (entry1.StatValue > entry2.StatValue)
            {
                return -1;
            }
            else if (entry1.StatValue < entry2.StatValue)
            {
                return 1;
            }
            else
            {
                if (entry1.Type == LeaderboardEntryType.Player)
                {
                    return -1;
                }
                else if (entry2.Type == LeaderboardEntryType.Player)
                {
                    return 1;
                }
                return 0;
            }
        }

        private void GenerateUUID()
        {
            var uuid = System.Guid.NewGuid().ToString();
            var newEntry = new LeaderboardEntry
            {
                Rank = boardData.Count + 1,
                StatValue = 0,
                Name = uuid
            };
            boardData.Add(newEntry);
        }

        public void Dispose()
        {
            OnLeaderboardUpdated = null;
        }
    }
    
    public enum LeaderBoardLeague
    {
        None,
        Bronze,
        Silver,
        Gold,
        Platinum,
        Diamond
    }
}
