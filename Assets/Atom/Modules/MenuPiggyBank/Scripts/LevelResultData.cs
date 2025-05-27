namespace Atom
{
    public class LevelResultData
    {
        public int LevelId;
        public bool Result; // true === win, false === lose
        public int Star;
        public int Score;
        public int CurrentLevel;
        public bool IsRoyalLeague;
        public int RoundIndex;
    }
}