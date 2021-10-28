using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if AP_GAMEANALYTICS_SDK_INSTALLED
using GameAnalyticsSDK;
#endif
#if AP_LIONSTUDIO_SDK_INSTALLED
using LionStudios.Suite.Analytics;
using LionStudios.Suite.Debugging;
#endif

public class BootManager : APManager
{
    public override void Awake()
    {
        base.Awake();
        ChangeGameState(GameState.GAME_DATA_LOADED);

#if AP_GAMEANALYTICS_SDK_INSTALLED
        GameAnalytics.Initialize();
#endif
#if AP_LIONSTUDIO_SDK_INSTALLED
        LionAnalytics.GameStart();
        LionDebugger.Hide();
#endif
    }

    public override void GameDataLoad()
    {
        base.GameDataLoad();

        APTools.functionManager.ExecuteAfterSecond(2.5f, () => {

            APTools.sceneManager.LoadLevel(GetValidLevelIndex());
        });
    }
}
