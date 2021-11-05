#if AP_FACEBOOK_SDK_INSTALLED_OBSULETE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookAnalyticsManager : MonoBehaviour
{
    public static FacebookAnalyticsManager instance;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

#region Public Callback

    public void FBALevelStart(int currentLevelNo)
    {
        var fbEventParameter = new Dictionary<string, object>();
        fbEventParameter["Level_Info"] = "" + currentLevelNo;
        FB.LogAppEvent(
            "Level_Start",
            null,
            fbEventParameter
        );

    }

    public void FBALevelComplete(int currentLevelNo)
    {
        var fbEventParameter = new Dictionary<string, object>();
        fbEventParameter["Level_Info"] = "" + currentLevelNo;
        FB.LogAppEvent(
            "Level_Complete",
            null,
            fbEventParameter
        );

    }

    public void FBALevelFailed(int currentLevelNo)
    {
        var fbEventParameter = new Dictionary<string, object>();
        fbEventParameter["Level_Info"] = "" + currentLevelNo;
        FB.LogAppEvent(
            "Level_Failed",
            null,
            fbEventParameter
        );

    }

    public void FBALevelRestart(int currentLevelNo)
    {
        var fbEventParameter = new Dictionary<string, object>();
        fbEventParameter["Level_Info"] = "" + currentLevelNo;
        FB.LogAppEvent(
            "Level_Restart",
            null,
            fbEventParameter
        );

    }

    public void FBRewardedVideoAd(string ads)
    {
        var fbEventParameter = new Dictionary<string, object>();
        fbEventParameter["Level_Info"] = "" + ads;
        FB.LogAppEvent(
            "RewardedVideoAd",
            null,
            fbEventParameter
        );
    }

#endregion
}
#endif