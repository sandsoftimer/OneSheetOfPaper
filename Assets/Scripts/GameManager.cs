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

public class GameManager : APManager
{
    public override void Awake()
    {
        base.Awake();
        ChangeGameState(GameState.NONE);
    }

    public override void None()
    {
        base.None();
        ChangeGameState(GameState.GAME_DATA_LOADED);
    }

    public override void GameDataLoad()
    {
        base.GameDataLoad();
        ChangeGameState(GameState.GAME_INITIALIZED);

        gameStartingUI.SetBool("Hide", false);
    }

    public override void GameStart()
    {
        base.GameStart();

        gameStartingUI.SetBool("Hide", true);
        gamePlayUI.SetBool("Hide", false);

#if AP_GAMEANALYTICS_SDK_INSTALLED
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "World01", "Level " + (gameplayData.currentLevelNumber + 1));
#endif
#if AP_LIONSTUDIO_SDK_INSTALLED
        PlayerPrefs.SetInt((gameplayData.currentLevelNumber + 1).ToString(), PlayerPrefs.GetInt((gameplayData.currentLevelNumber + 1).ToString(), 0) + 1);
        LionAnalytics.LevelStart(gameplayData.currentLevelNumber + 1, PlayerPrefs.GetInt((gameplayData.currentLevelNumber + 1).ToString()));
#endif

    }

    public override void GameOver()
    {
        base.GameOver();

        if (gameplayData.isGameoverSuccess)
        {
            gameplayData.currentLevelNumber++;
            gameSuccessUI.SetBool("Hide", false);
            gameOverEffect.Play();
            PlayThisSoundEffect(gameWinAudioClip);


#if AP_GAMEANALYTICS_SDK_INSTALLED
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "World01", "Level " + gameplayData.currentLevelNumber);
#endif
#if AP_LIONSTUDIO_SDK_INSTALLED
            LionAnalytics.LevelComplete(gameplayData.currentLevelNumber, 1);
#endif
        }
        else
        {
            gameFaildUI.SetBool("Hide", false);
            PlayThisSoundEffect(gameLoseFailAudioClip);


#if AP_GAMEANALYTICS_SDK_INSTALLED
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "World01", "Level " + (gameplayData.currentLevelNumber + 1));
#endif
#if AP_LIONSTUDIO_SDK_INSTALLED
            LionAnalytics.LevelFail(gameplayData.currentLevelNumber + 1, PlayerPrefs.GetInt((gameplayData.currentLevelNumber + 1).ToString()));
#endif
        }

    }

    public override void ReloadLevel()
    {
        base.ReloadLevel();

#if AP_LIONSTUDIO_SDK_INSTALLED
        LionAnalytics.LevelRestart(gameplayData.currentLevelNumber + 1, PlayerPrefs.GetInt((gameplayData.currentLevelNumber + 1).ToString()));
#endif
    }
}
