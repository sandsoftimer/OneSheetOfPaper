using System;
using UnityEngine;

[Serializable]
public class GameplayData
{
    public int gameScore;
    public int currentLevelNumber;

    public float gameStartTime, gameEndTime;
    public float totalLevelCompletedTime;

    public bool isGameoverSuccess;

    public GameplayData()
    {
        gameScore = 0;
        currentLevelNumber = 0;

        gameStartTime = 0f;
        gameEndTime = 0f;
        totalLevelCompletedTime = 0f;

        isGameoverSuccess = false;
    }
}

public class EditorButtonStyle
{
    public Color buttonColor;
    public Color buttonTextColor;
}