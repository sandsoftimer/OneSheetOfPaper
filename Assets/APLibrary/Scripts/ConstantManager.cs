using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(ConstantManagerOrder)]
public class ConstantManager
{
    #region Custom attributes of this game


    #endregion Custom attributes of this game

    public const string SOUND = "SOUND";
    public const string MUSIC = "MUSIC";
    public const string CURRENT_SCORE = "CURRENT_SCORE";
    public const string HIGH_SCORE = "HIGH_SCORE";
    public const string HIDE = "Hide";

    //Gameplay Data Veriables StringName
    public const string gameScore = "gameScore";
    public const string currentLevelNumber = "currentLevelNumber";
    public const string gameStartTime = "gameStartTime";
    public const string gameEndTime = "gameEndTime";
    public const string totalLevelCompletedTime = "totalLevelCompletedTime";
    public const string isGameoverSuccess = "isGameoverSuccess";

    // RuntimeExecution Order of APLibray Scripts.
    public const int ConstantManagerOrder = -30;
    public const int APToolOrder = -20;
    public const int APBehaviourOrder = -10;
    public const int APManagerOrder = -2;
    public const int TOTAL_GAME_LEVELS = 5;

    public const float DEFAULT_ANIMATION_TIME = 1f;
    public const float DRAGGING_THRESHOLD = 0.1f;
    public const float TAP_N_HOLD_THRESHOLD = 0.1f;
    public const float SWIPPING_THRESHOLD = 0.5f;

    /// <summary>
    /// Create an integer variable for script use
    /// and add that name to dictonary below as well
    /// APConfigarator.cs will use this to modify Layer System.
    /// </summary>
    #region Collision Layer Setup
    // Collision Layer ids. 
    // These layer's order must be maintained in Bottom Layer Array
    public const int DEFAULT_LAYER = 0;
    public const int TRANSPARENT_FX_LAYER = 1;
    public const int IGNORE_RAYCAST_LAYER = 2;
    public const int WATER_LAYER = 4;
    public const int UI_LAYER = 5;
    public const int PLAYER_LAYER = 8;
    public const int ENEMY_LAYER = 9;
    public const int GROUND_LAYER = 10;
    public const int BOUNDARY_LAYER = 11;
    public const int PICKUPS_LAYER = 12;
    public const int DESTINATION_LAYER = 13;
    public const int PLAYER_WEAPON = 14;
    public const int ENEMY_WEAPON = 15;
    public const int SENSOR = 16;
    public const int NON_RENDERING_LAYER = 17;
    public const int PAPER_DRAG_PART = 18;

    public static readonly Dictionary<int, string> layerNames = new Dictionary<int, string>
    {
        { 0, "Default" },
        { 1, "TransparentFX" },
        { 2, "Ignore Raycast" },
        { 3, "" },
        { 4, "Water" },
        { 5, "UI" },
        { 6, "" },
        { 7, "" },
        { 8, "PLAYER_LAYER" },
        { 9, "ENEMY_LAYER" },
        { 10, "GROUND_LAYER" },
        { 11, "BOUNDARY_LAYER" },
        { 12, "PICKUPS_LAYER" },
        { 13, "DESTINATION_LAYER" },
        { 14, "PLAYER_WEAPON" },
        { 15, "ENEMY_WEAPON" },
        { 16, "SENSOR" },
        { 17, "NON_RENDERING_LAYER" },
        { 18, "PAPER_DRAG_PART" },
        { 19, "" },
        { 20, "" },
        { 21, "" },
        { 22, "" },
        { 23, "" },
        { 24, "" },
        { 25, "" },
        { 26, "" },
        { 27, "" },
        { 28, "" },
        { 29, "" },
        { 30, "" },
        { 31, "" },
    };
    #endregion Collision Layer Setup
}
