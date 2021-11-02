using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : APBehaviour
{
    #region ALL UNITY FUNCTIONS

    // Awake is called before Start
    public override void Awake()
    {
        base.Awake();

        foreach (Transform item in transform)
        {
            item.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (gameState.Equals(GameState.GAME_INITIALIZED) && Input.GetMouseButtonDown(0))
        {
            gameManager.ChangeGameState(GameState.GAME_PLAY_STARTED);
            gameState = GameState.GAME_PLAY_STARTED;
        }

        if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
            return;

    }

    void FixedUpdate()
    {
        if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
            return;

    }

    void LateUpdate()
    {
        if (!gameState.Equals(GameState.GAME_PLAY_STARTED))
            return;

    }

    #endregion ALL UNITY FUNCTIONS
    //=================================   
    #region ALL OVERRIDING FUNCTIONS

    public override void OnGameDataLoad()
    {
        base.OnGameDataLoad();
        int index = 0;
        foreach (Transform item in transform)
        {
            item.gameObject.SetActive(index == gameManager.GetModedLevelNumber());
            index++;
        }
    }

    public override void OnGameInitializing()
    {
        base.OnGameInitializing();
        
    }

    #endregion ALL OVERRIDING FUNCTIONS
    //=================================
    #region ALL SELF DECLEAR FUNCTIONS


    #endregion ALL SELF DECLEAR FUNCTIONS

}
