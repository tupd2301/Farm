# Athena Game Ops

## CHANGELOG

### [Version 2.0.1]
Release notes:
+ Add video ad type(softlaunch, adbreak, inter,coldstart) callback
+ Fix bug load config ad unknow 
## [Version 2.0.0]
Release notes:
+ Add Adjust LogAdRevenue tracking
+ Add Adjust purchase SDK
+ Add more Ads type: ColdStart, SoftLaunch, AdBreak
+ Load Id Rewarded Ads from app_configs
## [Version 1.0.4 - Beta]

### Features

- Implement App Open Ad for Max SDK
- New Api for IAdManager:

```
- void RequestAppOpenAd(string adUnitId, bool autoRetry = true)
- void ShowAppOpenAd(System.Action<bool> cb = null, bool autoPreload = true)
```

- New Event for AthenApp:

```
public event Action<string, string, string> evtAppOpenAdLoaded;
public event Action<string, string, string> evtAppOpenAdFailedToLoad;
public event Action<string, string, string, float> evtAppOpenAdDisplayed;
public event Action<string, string, string> evtAppOpenAdDisplayFailed;
public event Action<string, string, string> evtAppOpenAdClicked;
public event Action<string, string> evtAppOpenAdHidden;
public event Action<string, string, string, float> evtAppOpenAdRevenuePaid;
```

### Usage

- Fill Ad Unit ID at `Athena/Resources/app_configs.bytes`

```
MAX_ios_app_open_ad_id = unknown
MAX_android_app_open_ad_id = unknown
```

- **Cold start** — Creating new session of the app on device. Just call `ShowAppOpenAd` after loading scene

- **Cold start** - Bringing the app from background to foreground, turning the phone on when the app is in foreground mode
  - Register Pause App event by call Athena App API:
  - `SubscribeAppPause(AppPaused listener)`
  - AppPaused is `Action<bool>`
  - Check App foreground anh call `ShowAppOpenAd`

```
AthenaApp.Instance.SubscribeAppPause((pauseStatus)=> {
    if(!pauseStatus)
        AthenaApp.Instance.AdManager?.ShowAppOpenAd();
});
```
