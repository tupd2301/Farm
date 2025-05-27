using System;
using System.Collections.Generic;

namespace RoomDecoration
{
    [Serializable]
    public class RoomDecorationSaveData
    {
        public int currentRoomIndex;
        public string currentRoomName;
        public List<FurnitureConfig> rooms;
    }
}