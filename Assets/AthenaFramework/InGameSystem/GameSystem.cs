using System.Collections.Generic;

namespace AthenaFramework.InGameSystem
{
    public abstract class GameSystem : IGameSystem
    {
        public abstract void Init();
        public abstract void Destroy();
        public virtual List<string> GetConfigPaths()
        {
            return new();
        }
    }
}
