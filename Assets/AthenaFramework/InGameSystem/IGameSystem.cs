using System.Collections.Generic;

namespace AthenaFramework.InGameSystem
{
    public interface IGameSystem
    {
        void Init();
        void Destroy();
        List<string> GetConfigPaths();
    }
}
