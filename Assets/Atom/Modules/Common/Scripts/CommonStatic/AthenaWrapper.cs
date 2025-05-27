using Athena.GameOps;
using UnityEngine;
#if USE_APPSFLYER
using AppsFlyerSDK;
#endif

namespace Atom
{
    public partial class AthenaWrapper
    {
        static AthenaWrapper _instance;
        public static AthenaWrapper Instance
        {
            get
            {
                if( _instance == null) _instance =  new AthenaWrapper();
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }
        AthenaApp _athenaApp;
        AthenaWrapper()
        {
            GameObject go = new GameObject("_AthenaApp");
            _athenaApp = go.AddComponent<AthenaApp>();
            go.AddComponent<TrackingManager>();
            _athenaApp.Initialize();
            GameObject.DontDestroyOnLoad(go);
#if USE_APPSFLYER
            AppsFlyerAdRevenue.start();
#endif

        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_instance != null) return;
            _instance = new AthenaWrapper();
        }
    }
}